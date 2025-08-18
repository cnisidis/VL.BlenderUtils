

//https://github.com/blender/blender/blob/main/doc/blender_file_format/BlendFileReader.py


using System.Runtime.InteropServices;
using VL.BlenderUtils.Parser.Native;
using VL.BlenderUtils.Parser.Managed;
using VL.Lib.Collections;
using VL.BlenderUtils.Parser.DNA;
using System.CodeDom;
using System.Text;

namespace VL.BlenderUtils.Parser
{
    public class BlendFile : IDisposable
    {


        public Header header { get; set; }
        //List<FileBlock> blocks { get; set; }
        public List<BlenderObjectBase> Objects { get; set; }
        public List<Managed.Scene> Scenes { get; set; }
        

        private bool FoundDnaBlock = false;
        DNACatalog Catalog;

        private FileStream _fileStream;
        private BinaryReader _handle;
        private Dictionary<IntPtr, FileBlock> Blocks;

        
        public BlendFile()
        {
            //blocks = new List<FileBlock>();
            
            Objects = new List<BlenderObjectBase>();
            Blocks = new Dictionary<IntPtr, FileBlock>();
            Scenes = new();
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

                    //blocks.Add(fileBlock);
                    try
                    {
                        if(fileBlock.Header.Code.Contains("CA")) { Console.WriteLine($"{fileBlock.Header.FileOffset} {fileBlock.Header.OldAddress}"); }
                        Blocks.Add(new IntPtr((long)fileBlock.Header.OldAddress), fileBlock);
                    }
                    catch
                    {
                        Console.WriteLine(fileBlock.Header.OldAddress);
                    }
                    fileBlock = new FileBlock(_handle, this);


                }
                //END FileBlock

                Blocks.Add(new IntPtr((long)fileBlock.Header.OldAddress), fileBlock);
            }
            else
            {
                throw new NotImplementedException();
            }
            {
                
                Map();
                
            }
            Console.WriteLine(Marshal.SizeOf<ID>().ToString());
            Dispose();
            
        }

        public void Map()
        {
            
            var _scnenes = Blocks.Values.ToList().FindAll(x=>x.Header.Code == "SC");
            
            foreach(var _scn in _scnenes)
            {
                var offset = 0;
                for (int i = 0; i < _scn.Header.Count; i++)
                {
                    var depth = 0;
                    var bytes = GetFileBlockBytesByOffset(_scn.Header);
                    //var sc = new Managed.Scene(Helpers.BytesToStruct<Native.Scene>(bytes, 0), this);
                    
                    var _sc = ReadFromBytes<Native.Scene>(bytes.Skip(offset).ToArray(), Catalog, ref depth);
                    var sc = new Managed.Scene(_sc, this);
                    Console.WriteLine(_sc.camera);
                    Scenes.Add(sc);
                    //offset = GetDNACatalog().Structures.Find(x => x.TypeName == "Scene").Size;
                }
            }
            
            _fileStream.Seek(0, SeekOrigin.Begin);
            
            var _objs = Blocks.Values.ToList().FindAll(x => x.Header.Code == "OB");
            foreach (var _obj in _objs)
            {
                var nativeObj = ResolvePtr<Native.Object>(new IntPtr((long)_obj.Header.OldAddress));
                var obj = (BlenderObjectBase)CreateManagedObject(nativeObj);
                Objects.Add(obj);
            }

            _fileStream.Seek(0, SeekOrigin.Begin);
           
        }

        public dynamic CreateManagedObject(Native.Object nativeObject)
        {
            var objectType = (ObjectType)nativeObject.type;
            switch (objectType)
            {
                case ObjectType.OB_CAMERA:
                    return new BlenderObject<Camera>(nativeObject, this);
                
                default:
                    return null ;
                    //throw new InvalidOperationException($"Cannot create a managed object for unknown type: {objectType}");
            }
        }

        public byte[] GetFileBlockBytesByOffset(FileBlockHeader header) 
        {
            var size = header.Size;
            var offset = header.FileOffset;
            var bytes = new byte[size];
            _fileStream.Position = offset;
            _fileStream.Read(bytes, 0, (int)size);
            _fileStream.Seek(0, SeekOrigin.Begin);
            return bytes;

        }
        /*
                 * Example --> first field in native class is ID
                 * we lookup in the DNACatalog.Structures to find the ID,
                 * we get the Size of the ID and read the bytes with a Helper Function in order to return an ID,
                 * the offset must be set then to the size of the initial object.
        */
        public static T ReadFromBytes<T>(byte[] bytes, DNACatalog DNACat, ref int depth) where T : new()
        {
            
            //Get the DNA structure which correspond to the native class/struct
            var nativeClassName = typeof(T).Name;
            Console.WriteLine(nativeClassName);
            DNAStructure dnaStruct = DNACat.Structures.Find(x => x.TypeName == nativeClassName);
            var dnaStructFields = dnaStruct.Fields;

            //Create new istance of the output class/struct
            T result = new T();
            var offset = 0;
            var size = dnaStruct.Size;
            
            //Iterate through all the Fields of the native struct
            foreach (var field in typeof(T).GetFields())
            {
                //Lookup the dna structures collection and locate the exact dna field that matches the exact name of the native class fields
                var dnaEquivalentField = dnaStruct.Fields.Find(x => x.GetShortName() == field.Name);
                //if the dna equivalent is not null then
                //1. Construct the object
                //  a. Get the size of the dna equivalent (Calculated Size)
                //  b. Set the size of the bytes.Take(..)
                //  c. Return the Object 
                //  d. proceed next

                
                if (dnaEquivalentField != null)
                {
                    /*
                    Console.WriteLine($"{dnaEquivalentField.GetShortName()} " + $"--> {field.Name} of type {field.FieldType} " +
                        $"with size of {dnaEquivalentField.CalculatedSize}");
                    */
                    
                    var fieldSize = dnaEquivalentField.CalculatedSize;
                    var type = dnaEquivalentField.SystemType;
                    dynamic value = null;
                    switch (dnaEquivalentField.InnerType)
                    {
                        case FieldType.Pointer:
                            value=ReadPrimitiveType(bytes, offset, type, fieldSize);
                            
                            break;
                        case FieldType.ValueType:
                            value=ReadPrimitiveType(bytes, offset, field.FieldType, fieldSize);
                            break;

                        case FieldType.Array:
                            if (dnaEquivalentField.SystemType == typeof(string)) value = ReadPrimitiveType(bytes, offset, field.FieldType, fieldSize);
                            
                                break;

                        case FieldType.StructType:
                            try
                            {
                                if (field.FieldType == typeof(ID)) 
                                { 
                                    value = (ID)ReadFromBytes<ID>(bytes.Skip(offset).Take(dnaEquivalentField.CalculatedSize).ToArray(), DNACat, ref depth);
                                }
                                else if (field.FieldType == typeof(ID_Runtime))
                                {
                                    value = (ID_Runtime)ReadFromBytes<Native.ID_Runtime>(bytes.Skip(offset).Take(dnaEquivalentField.CalculatedSize).ToArray(), DNACat, ref depth);
                                }
                            }
                            catch (Exception ex) 
                            {
                                Console.WriteLine(ex.Message);
                            }
                            
                            break;

                    }
                    offset += dnaEquivalentField.Size;


                    try
                    {
                        Console.WriteLine($"D:{depth} :: {field.Name} -> {value.ToString()}");
                        field.SetValue(result, value);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine($"D:{depth} :: {field.Name} {field.FieldType} {dnaEquivalentField.Name} {dnaEquivalentField.InnerType} {dnaEquivalentField.SystemType}");

                    }

                }

                else
                {
                    Console.WriteLine($"D:{depth} !!! ---> '{field.Name}' can not be matched!");
                }

            }
            Console.WriteLine($"Native Auto Generated Class {result.GetType()}");
            depth += 1;
            return result;

        }

        private static dynamic ReadPrimitiveType(byte[] bytes, int offset, System.Type type, int size)
        {
            if (type == typeof(int) || type == typeof(Int32)) return BitConverter.ToInt32(bytes, offset);
            else if (type == typeof(uint)) return BitConverter.ToUInt32(bytes, offset);
            else if(type == typeof(float) || type == typeof(Single)) return BitConverter.ToSingle(bytes, offset);
            else if(type == typeof(short)) return BitConverter.ToInt16(bytes, offset);
            else if(type == typeof(ushort)) return BitConverter.ToUInt16(bytes, offset);
            else if(type == typeof(ulong)) return BitConverter.ToUInt64(bytes, offset);
            else if(type == typeof(long)) return BitConverter.ToInt64(bytes, offset);
            else if(type == typeof(IntPtr)) return (IntPtr)BitConverter.ToInt64(bytes, offset);
            else if(type == typeof(string)) return Encoding.UTF8.GetString(bytes, offset, size).TrimEnd('\0');

            else
                // Add more primitive types as needed.
                return null;
        }

        

        /// <summary>
        /// Lazily resolves a pointer from a data block and deserializes the
        /// corresponding data into a new struct of type T.
        /// </summary>
        /// <typeparam name="T">The type of struct to deserialize the data into.</typeparam>
        /// <param name="oldAddress">The old memory address (the pointer) found in a data block.</param>
        /// <returns>A new instance of T with the data, or null if the pointer is invalid or not found.</returns>
        public T ResolvePtr<T>(IntPtr oldAddress) where T : struct
        {
            // Pointers can be null (IntPtr.Zero). We should handle this gracefully.
            if (oldAddress == IntPtr.Zero)
            {
                return default(T);
            }

            // Look up the pointer's corresponding FileBlock in our pre-parsed index.
            if (!Blocks.TryGetValue(oldAddress, out var fileBlock))
            {
                Console.WriteLine($"Warning: Pointer {oldAddress} not found in the map.");
                return default(T);
            }

            // Get the size of the data block from its header.
            long size = fileBlock.Header.Size;

            // Get the file offset (position) from the header.
            long offset = fileBlock.Header.FileOffset;

            // CRITICAL: Set the file stream's position to the start of the data block.
            _fileStream.Position = offset;

            // Create a byte array to hold the data.
            var bytes = new byte[size];

            // Read the exact number of bytes for the data block.
            var totalBytesRead = _handle.Read(bytes, 0, (int)size);
            if (totalBytesRead != size)
            {
                Console.WriteLine($"Warning: Expected to read {size} bytes but only read {totalBytesRead} for pointer {oldAddress}.");
            }

            // Marshal the byte array to the specified generic struct type.
            // This is a common helper method you likely have.
            // For demonstration, let's include a simple version of it.
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T result = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return result;
        }

        

        public Spread<FileBlock> GetFileBlocks()
        {
            return Blocks.Values.ToSpread();
        }

        public DNACatalog GetDNACatalog()
        {
            if (this != null)
                return this.Catalog;
            else
                return null;
        }
        public IEnumerable<BlenderObjectBase> GetObjects()
        {
            return this.Objects;
        }
        public Spread<Managed.Scene> GetScenes()
        {
            return this.Scenes.ToSpread();
        }

        public Spread<BlenderObject<Camera>> GetCameras()
        {
            return Objects.OfType<BlenderObject<Camera>>().ToSpread();
        }

        public void Dispose()
        {
            _fileStream.Dispose();
            _handle.Dispose();
            Console.WriteLine("--Parser Disposed--");
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
            public ulong OldAddress;
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

                //Console.WriteLine("Found blend-file-block-fileheader {0:G} {1:G}", Code, FileOffset);
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


    }
}