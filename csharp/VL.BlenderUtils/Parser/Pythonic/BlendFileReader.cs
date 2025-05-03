

//https://github.com/blender/blender/blob/main/doc/blender_file_format/BlendFileReader.py

using Stride.Core.Extensions;
using System.Dynamic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using VL.BlenderUtils.Parser.DNA;
using VL.Lib.Collections;

namespace VL.BlenderUtils.Parser.Pythonic
{
    public class BlendFile:IDisposable
    {


        public Header header { get; set; }
        List<FileBlock> blocks { get; set; }

        private bool FoundDnaBlock = false;
        DNACatalog Catalog;

        public List<Scene> Scenes;

        BinaryReader Handle;
        public BlendFile()
        {
            blocks = new List<FileBlock>();
            Scenes = new List<Scene>();
        }

        public void OpenBlendFile(string blendFile)
        {
            
            using (var stream = new FileStream(blendFile, FileMode.Open, FileAccess.Read, FileShare.None))
            using (var handle = new BinaryReader(stream))
            {
                Handle = handle;
                var magic = Reader.ReadString(handle, 7);
                if(magic.Contains("BLENDER") || magic.Contains("BULLETf"))
                {
                    Console.WriteLine("Normal blendfile detected");
                    handle.BaseStream.Seek(0, SeekOrigin.Begin);

                    header = new Header(handle);

                    Console.WriteLine("Version: {0:G} | LittleEndianess: {1:G} | Pointer Size: {2:G}", header.Version, header.LittleEndianess, header.PointerSize);

                    var fileBlock = new FileBlock(handle, this);

                    while(!FoundDnaBlock)
                    {
                        if (fileBlock.Header.Code.Contains("DNA1") || fileBlock.Header.Code.Contains("SDNA"))
                        {
                            Catalog = new DNACatalog(header, handle);
                            FoundDnaBlock = true;
                        }
                        else
                            fileBlock.Header.Skip(handle);

                        blocks.Add(fileBlock);
                        fileBlock = new FileBlock(handle, this);

                            
                    }

                    blocks.Add(fileBlock);


                    


                }
                else
                {
                    throw new NotImplementedException();
                }

                //Time to remap and create realise Structures ?
                foreach (var block in blocks)
                {
                    if (block.Header.Code == "SC")
                    {
                        block.Get(handle);
                        var count = block.Header.Count;
                        for (int i = 0; i < count; i++)
                        {
                            var scn = Scene.Read(handle, header);
                            Scenes.Add(scn);
                        }

                    }

                }
            }

        }

        public DNACatalog GetDNACatalog()
        {
            if (this != null)
                return this.Catalog;
            else
                return null;
        }

        public Spread<Scene> GetScenes()
        {
            

            return Scenes.ToSpread();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// BlendFileHeader allocates the first 12 bytes of a blend file.
        /// It contains information about the hardware architecture.
        /// Header example: BLENDER_v254
        /// </summary>

        public partial class Header
        {
            public string Magic { get; }
            public int PointerSize { get; }
            public bool LittleEndianess { get; }

            public int Version { set; get; }
            public Header(BinaryReader handle)
            {
                Magic = Reader.ReadString(handle, 7);
                var pointerSize = Reader.ReadString(handle, 1);
                if (pointerSize == "-")
                    this.PointerSize = 8;
                if (pointerSize == "_")
                    this.PointerSize = 4;

                var endianess = Reader.ReadString(handle, 1);
                if (endianess == "v")
                    LittleEndianess = true;
                if (endianess == "V")
                    LittleEndianess = false;

                var version = Reader.ReadString(handle, 3);
                this.Version = int.Parse(version);

            }

            public void Split(out string Magic, out int PointerSize, out bool LittleEndianess, out int Version)
            {

                Magic = this.Magic;
                PointerSize = this.PointerSize;
                LittleEndianess = this.LittleEndianess;
                Version = this.Version;
                
            }
        }
        /// <summary>
        /// BlendFileBlock.File
        /// BlendFileBlock.Header
        /// </summary>
        public partial class FileBlock
        {
            BlendFile File;
            public FileBlockHeader Header;

            public FileBlock(BinaryReader handle, BlendFile file)
            {
                this.File = file;
                this.Header = new FileBlockHeader(handle, file.header);
            }

            public void Get(BinaryReader handle)
            {
                var dnaIndex = this.Header.SDNAIndex;
                var dnaStruct = "";
                handle.BaseStream.Seek(this.Header.FileOffset, SeekOrigin.Begin);

            }

           
        }
        /// <summary>
        /// FileBlockHeader contains the information in a file-block-header.
        /// The class is needed for searching to the correct file-block (containing Code: DNA1)
        /// </summary>
        public partial class FileBlockHeader
        {

            public string Code;
            uint Size { get; }
            ulong OldAddress;
            public uint SDNAIndex;
            public uint Count;
            public long FileOffset;

            public FileBlockHeader(BinaryReader handle, Header FileHeader)
            {
                this.Code = Reader.ReadString(handle, 4).Replace('\x00', ' ').Trim();
                if(Code != "ENDB")
                {
                    this.Size = Reader.Read(ReaderType.UI, handle, FileHeader);
                    OldAddress = Reader.Read(ReaderType.P, handle, FileHeader);
                    //Console.WriteLine(OldAddress);
                    SDNAIndex = Reader.Read(ReaderType.UI, handle, FileHeader);
                    Count = Reader.Read(ReaderType.UI, handle, FileHeader);
                    FileOffset = handle.BaseStream.Position;

                    
                }
                else
                {
                    Size = Reader.Read(ReaderType.UI, handle, FileHeader);
                    OldAddress = 0;
                    SDNAIndex = 0;
                    Count = 0;
                    FileOffset = handle.BaseStream.Position;
                }

                Console.WriteLine("Found blend-file-block-fileheader {0:G} {1:G}", Code, FileOffset);
            }

            public void Skip(BinaryReader handle)
            {
                handle.ReadBytes((int)Size);
            }


        }

        /// <summary>
        /// DNACatalog is a catalog of all information in the DNA1 file-block

        /// </summary>
        public partial class DNACatalog
        {
            public List<string> Names;
            List<DNAType> Types;
            public List<DNAStructure> Structures;
            public DNACatalog(Header header, BinaryReader handle)
            {
                Names = new List<string>();
                Types = new List<DNAType>();
                Structures = new List<DNAStructure>();
                
                
                var startOffset = handle.BaseStream.Position;
                var SDNA = Reader.ReadString(handle, 4);
                if(SDNA != "SDNA")
                {
                    Console.WriteLine("SDNA tag is not parsed properly - abodring");
                    return;
                }
                
                //names
                var NAME = Reader.ReadString(handle, 4);
                if (NAME != "NAME")
                {
                    Console.WriteLine("NAME tag is not parsed properly - abodring");
                    return;
                }
                var numberOfNames = Reader.Read(ReaderType.UI, handle, header);
                Console.WriteLine("Building {0:G} NAMES", numberOfNames);

                //Potential reason that NAMES are not offseted properly -> https://github.com/blender/blender/blob/main/source/blender/makesdna/intern/dna_genfile.cc#L377C23-L377C28
                /* "float gravity [3]" was parsed wrong giving both "gravity" and
                 * "[3]"  members. we rename "[3]", and later set the type of
                 * "gravity" to "void" so the offsets work out correct */

                for (int i =0; i<numberOfNames; i++)
                {
                    var name = Reader.ReadString(handle);
                    
                    if(name.IndexOf("[") == 0 && Regex.IsMatch(name, @"(\[+\d+\])"))
                    {
                        Console.WriteLine("{0:G} is not parsed properly", name);
                        var newName = Names[i - 1];
                        name = newName+name;
                        
                    }
                    Names.Add(name);
                }

                Reader.AlignAlt(handle, startOffset);
                
                var TYPE = Reader.ReadString(handle, 4); 

                if (TYPE != "TYPE")
                {
                    Console.WriteLine("Error on Parsing Types - Alignment is wrong: {0:G}", TYPE);
                    return;
                }
                    
                
                
                var numberOfTypes = Reader.Read(ReaderType.UI, handle, header);
                Console.WriteLine("Building {0:G} TYPES", numberOfTypes);
                
                for (int i = 0; i < numberOfTypes; i++)
                {
                    var type = Reader.ReadString(handle);
                    //Create new DNAType
                    var dnaType = new DNAType(type);
                    Types.Add(dnaType);
                }
                Reader.AlignAlt(handle, startOffset);

                //types lengths
                var TLEN = Reader.ReadString(handle, 4);
                Console.WriteLine("Building {0:G} TYPE-LENGTHs", numberOfTypes);
                
                for (int i = 0; i < numberOfTypes; i++)
                {
                    var length = Reader.Read(ReaderType.US, handle, header);
                    //Get dnaType and set its size
                    Types[i].Size = length;
                    
                }
                Reader.AlignAlt(handle, startOffset);

                //structs
                var STRC = Reader.ReadString(handle, 4);
                if(STRC != "STRC")
                {
                    return;
                }
                var numberOfStructs = Reader.Read(ReaderType.UI, handle, header);
                Console.WriteLine("Building {0:G} STRUCTS", numberOfStructs);
                
                
                for (int structureIndex =0; structureIndex < numberOfStructs; structureIndex++)
                {
                    
                    var type = Reader.Read(ReaderType.US, handle, header);
                    DNAStructure structure = new DNAStructure();
                    
                    var numberOfFields = Reader.Read(ReaderType.US, handle, header);
                    

                    for (int fieldIndex=0; fieldIndex<numberOfFields; fieldIndex++)
                    {
                        var fTypeIndex = Reader.Read(ReaderType.US, handle, header);
                        
                        var fNameIndex = Reader.Read(ReaderType.US, handle, header);
                        
                        var fType = Types[fTypeIndex];
                        var fName = Names[fNameIndex];
                        //Console.WriteLine(fName);
                        //DNAField field = new DNAField(fType, fName);
                        //Console.WriteLine(field.ToString());
                        //structure.Fields.Add(field);
                        
                    }

                    Structures.Add(structure);
                }
                

            }

            public Spread<DNAType> GetTypes()
            {
                return Types.ToSpread();
            }

            public Spread<DNAStructure> GetStructures()
            {
                return Structures.ToSpread();
            }
        }

        public class DNAName
        {
            public string Name { get; }
            public DNAName(string name)
            {
                this.Name = name;
            }

            public string AsReference(string parent)
            {
                string result;

                if (parent == null)
                {
                    return "";
                }
                else return parent + ".";
            }

            public string ShortName()
            {
                var result = this.Name;
                result = result.Replace("*", "");
                result = result.Replace("(", "");
                result = result.Replace(")", "");
                var index = result.ToCharArray().IndexOf('[');
                if(index != -1)
                {
                    result = new string(result.ToCharArray().Take(index).ToArray());
                }

                return result;
            }

            public bool IsPointer()
            {
                return Name.IndexOf('*') > -1;     
            }

            public bool IsMethodPointer()
            {
                return Name.Contains("(*");
            }

            public int ArraySize()
            {
                var result = 1;

                var tmp = Name;
                var idx = Name.IndexOf('[');

                while(idx != -1)
                {
                    var idx2 = tmp.IndexOf(']');
                    var mult = int.Parse( new string ((tmp.Skip(idx + 1).Take(idx2).ToArray())));
                    result *= mult;
                    tmp = new string (tmp.Skip(idx2+1).ToArray());
                    idx = tmp.IndexOf('[');
                }


                return result;
            }

        }

        public class DNAType
        {
            public string Name;
            public int Size;
            public DNAStructure Structure;

            public DNAType(string name)
            {
                this.Name = name;
                this.Structure = null;
            }

            public void ToVLType()
            {

            }
        }


        public class BType
        {
            int alignOf;
            int sizeOf;
            

            public BType()
            {

            }
        }

        public class DNAStructure
        {

            DNAType Type;
            public List<DNAField> Fields;
            public DNAStructure()
            {
                //this.Type = aType;
                Fields = new List<DNAField>();
            }

            public dynamic GetField(Header header, BinaryReader handle, string path)
            {
                var splitted = path.Split(new string[] { "."} , StringSplitOptions.RemoveEmptyEntries);
                var name = splitted[0];
                var rest = splitted[2];
                var offset = 0;

                foreach(var field in this.Fields)
                {
                    if(field.Name.ShortName() == name)
                    {
                        Console.WriteLine("Found {0:G}@{1:G}", name, offset);
                        handle.BaseStream.Seek(offset, SeekOrigin.Current);
                        return field.DecodeField(header, handle, rest);

                    }
                    else
                    {
                        offset += field.Size(header);
                    }
                }

                return null;
            }

            public void ToString(out string Result)
            {
                Result = this.Fields.Count().ToString();
            }
        }

        /// <summary>
        /// DNAField is a coupled DNAType and DNAName.
        /// </summary>
        public class DNAField
        {
            public DNAType Type;
            public DNAName Name;
            public string sType;

            public DNAField(DNAType aType, DNAName aName)
            {
                this.Type = aType;
                this.Name = aName;

            }

            public DNAField(string sType, string aName)
            {
                this.sType = sType;
                this.Name = new DNAName(aName);
            }

            public int Size(Header header)
            {
                if(Name.IsPointer() || Name.IsMethodPointer())
                {
                    return header.PointerSize * Name.ArraySize();
                }
                else
                {
                    return Type.Size * Name.ArraySize();
                }
            }

            public dynamic DecodeField(Header header, BinaryReader handle, string path)
            {
                if(path == "")
                {
                    if (Name.IsPointer()) return Reader.Read(ReaderType.P, handle, header);
                    if (Type.Name == "int") return Reader.Read(ReaderType.I, handle, header);
                    if (Type.Name == "short") return Reader.Read(ReaderType.S, handle, header);
                    if (Type.Name == "float") return Reader.Read(ReaderType.F, handle, header);
                    if (Type.Name == "char") return Reader.ReadString(handle, Name.ArraySize());
                }
                else
                {
                    Type.Structure.GetField(header, handle, path);
                }


                return 0;
            }

            public string ToString()
            {
                return this.Name + this.Type.ToString();
            }
        }

    }
}
