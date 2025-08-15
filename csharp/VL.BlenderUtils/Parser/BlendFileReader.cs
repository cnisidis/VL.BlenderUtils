

//https://github.com/blender/blender/blob/main/doc/blender_file_format/BlendFileReader.py

using Stride.Core.Extensions;
using System.Text.RegularExpressions;

using VL.Lib.Collections;

namespace VL.BlenderUtils.Parser
{
    public class BlendFile : IDisposable
    {


        public Header header { get; set; }
        List<FileBlock> blocks { get; set; }

        private bool FoundDnaBlock = false;
        DNACatalog Catalog;


        private FileStream _fileStream;
        private BinaryReader _handle;
        public BlendFile()
        {
            blocks = new List<FileBlock>();

        }

        public void OpenBlendFile(string blendFile)
        {
            _fileStream = new FileStream(blendFile, FileMode.Open, FileAccess.Read, FileShare.None);
            _handle = new BinaryReader(_fileStream) ;
           
            var magic = Reader.ReadString(_handle, 7);
            if (magic.Contains("BLENDER") || magic.Contains("BULLETf"))
            {
                Console.WriteLine("Normal blendfile detected");
                _handle.BaseStream.Seek(0, SeekOrigin.Begin);

                header = new Header(_handle);

                Console.WriteLine("Version: {0:G} | LittleEndianess: {1:G} | Pointer Size: {2:G}", header.Version, header.LittleEndianess, header.PointerSize);

                var fileBlock = new FileBlock(_handle, this);

                while (!FoundDnaBlock)
                {
                    if (fileBlock.Header.Code.Contains("DNA1") || fileBlock.Header.Code.Contains("SDNA"))
                    {
                        Catalog = new DNACatalog(header, _handle);
                        FoundDnaBlock = true;
                    }
                    else
                        fileBlock.Header.Skip(_handle);

                    blocks.Add(fileBlock);
                    fileBlock = new FileBlock(_handle, this);


                }

                blocks.Add(fileBlock);





            }
            else
            {
                throw new NotImplementedException();
            }
            {
                Map();
                
            }

            Dispose();
            
        }

        public void Map()
        {
            
        }

        public Spread<FileBlock> GetFileBlocks()
        {
            return blocks.ToSpread();
        }

        public DNACatalog GetDNACatalog()
        {
            if (this != null)
                return this.Catalog;
            else
                return null;
        }

        

        public void Dispose()
        {
            _fileStream.Dispose();
            _handle.Dispose();
            Console.WriteLine("--Parser Was Propserly Disposed");
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


            public byte[] Parse(BinaryReader handle, int Size = 0)
            {
                var dnaIndex = this.Header.SDNAIndex;
                handle.BaseStream.Seek(this.Header.FileOffset, SeekOrigin.Begin);
                int s = Size == 0 ? (int)this.Header.Size : (int)Size;
                return Reader.ReadBytes(handle, s);

            }


        }
        /// <summary>
        /// FileBlockHeader contains the information in a file-block-header.
        /// The class is needed for searching to the correct file-block (containing Code: DNA1)
        /// </summary>
        public partial class FileBlockHeader
        {

            public string Code;
            public uint Size { get; }
            ulong OldAddress;
            public uint SDNAIndex;
            public uint Count;
            public long FileOffset;

            public FileBlockHeader(BinaryReader handle, Header FileHeader)
            {
                this.Code = Reader.ReadString(handle, 4).Replace('\x00', ' ').Trim();
                if (Code != "ENDB")
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

            public void Split(out string Code, out int Size, out int Index, out int Count, out long FileOffset)
            {
                Code = this.Code;
                Size = (int)this.Size;
                Index = (int)this.SDNAIndex;
                Count = (int)this.Count;
                FileOffset = this.FileOffset;
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
                if (SDNA != "SDNA")
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

                for (int i = 0; i < numberOfNames; i++)
                {
                    var name = Reader.ReadString(handle);

                    if (name.IndexOf("[") == 0 && Regex.IsMatch(name, @"(\[+\d+\])"))
                    {
                        Console.WriteLine("{0:G} is not parsed properly", name);
                        var newName = Names[i - 1];
                        name = newName + name;

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
                if (STRC != "STRC")
                {
                    return;
                }
                var numberOfStructs = Reader.Read(ReaderType.UI, handle, header);
                Console.WriteLine("Building {0:G} STRUCTS", numberOfStructs);


                for (int structureIndex = 0; structureIndex < numberOfStructs; structureIndex++)
                {

                    var type = Reader.Read(ReaderType.US, handle, header);
                    string typeName = Types[type].Name;

                    DNAStructure structure = new DNAStructure
                    {
                        TypeName = typeName
                    };

                    var numberOfFields = Reader.Read(ReaderType.US, handle, header);


                    for (int fieldIndex = 0; fieldIndex < numberOfFields; fieldIndex++)
                    {
                        var fTypeIndex = Reader.Read(ReaderType.US, handle, header);

                        var fNameIndex = Reader.Read(ReaderType.US, handle, header);

                        var fType = Types[fTypeIndex];
                        var fName = Names[fNameIndex];



                        var field = new DNAField(fType.Name, fName);
                        structure.Fields.Add(field);

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
                string result=string.Empty;

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
                if (index != -1)
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

                while (idx != -1)
                {
                    var idx2 = tmp.IndexOf(']');
                    var mult = int.Parse(new string((tmp.Skip(idx + 1).Take(idx2).ToArray())));
                    result *= mult;
                    tmp = new string(tmp.Skip(idx2 + 1).ToArray());
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



        public class DNAStructure
        {

            public string TypeName;
            public List<DNAField> Fields { get; set; } = new();


            public void ToString(out string Result)
            {
                Result = this.Fields.Count().ToString();
            }

            public void Split(out string Name, out Spread<DNAField> Fields)
            {
                Name = TypeName;
                Fields = this.Fields.ToSpread();
            }
        }

        /// <summary>
        /// DNAField is a coupled DNAType and DNAName.
        /// </summary>
        public class DNAField
        {
            public string Type;
            public string Name;


            public DNAField(string Type, string Name)
            {
                this.Type = Type;
                this.Name = Name;

            }


            public string ToString()
            {
                return this.Name + this.Type;
            }


            public void Split(out string Name, out string Type)
            {
                Name = this.Name;
                Type = this.Type;
            }
        }

    }
}