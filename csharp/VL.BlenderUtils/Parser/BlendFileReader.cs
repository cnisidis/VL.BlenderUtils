

//https://github.com/blender/blender/blob/main/doc/blender_file_format/BlendFileReader.py


using CommunityToolkit.HighPerformance;
using System.CodeDom;
using System.Drawing;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using VL.BlenderUtils.Parser.DNA;
using VL.BlenderUtils.Parser.Managed;
using VL.BlenderUtils.Parser.Native;
using VL.Core;
using VL.Lib.Animation;
using VL.Lib.Collections;
using static VL.BlenderUtils.Parser.BlendFile;

namespace VL.BlenderUtils.Parser
{
    public class BlendFile : IDisposable
    {


        public Header header { get; set; }
        //List<FileBlock> blocks { get; set; }
        public List<BlenderObjectBase> Objects { get; set; }
        public List<Managed.Scene> Scenes { get; set; }

        public List<Camera> Cameras { get; set; }
        

        private bool FoundDnaBlock = false;
        DNACatalog Catalog;

        private FileStream _fileStream;
        private BinaryReader _handle;
        private Dictionary<IntPtr, FileBlock> Blocks;

        StringBuilder Logging = new StringBuilder();
        private string _logFilePath;
        private string _logFileName;
        public BlendFile()
        {
            //blocks = new List<FileBlock>();
            
            Objects = new List<BlenderObjectBase>();
            Blocks = new Dictionary<IntPtr, FileBlock>();
            Scenes = new();
            _logFilePath = Path.GetDirectoryName(AppHost.Current.AppPath);
            _logFileName = "log.txt";
            //Console.WriteLine(System.IO.Path.GetTempPath());
        }

        public void OpenBlendFile(string blendFile)
        {
            Logging.Clear();
            Blocks.Clear();
            
                File.WriteAllText(_logFilePath + "/" + _logFileName, String.Empty); 


                _fileStream = new FileStream(blendFile, FileMode.Open, FileAccess.Read, FileShare.None);
            _handle = new BinaryReader(_fileStream) ;
           
            var magic = Reader.ReadString(_handle, 7);
            if (magic.Contains("BLENDER") || magic.Contains("BULLETf"))
            {
                Logging.AppendLine("Normal blendfile detected");
                _handle.BaseStream.Seek(0, SeekOrigin.Begin);

                header = new Header(_handle);

                Logging.AppendLine($"Version: {header.Version} | LittleEndianess: {header.LittleEndianess} | Pointer Size: {header.PointerSize}");

                var fileBlock = new FileBlock(_handle, this);
                var isEnd = false; 
                while (!FoundDnaBlock)
                {
                    if (fileBlock.Header.Code.Contains("DNA1") || fileBlock.Header.Code.Contains("SDNA"))
                    {
                        Catalog = new DNACatalog(header, _handle);
                        FoundDnaBlock = true;
                    }
                    
                    
                    else
                        fileBlock.Header.Skip(_handle);

                    
                    try
                    {
                        Blocks.Add(new IntPtr((long)fileBlock.Header.OldAddress), fileBlock);
                    }
                    catch (Exception ex)
                    {
                        
                        Logging.AppendLine($"{ex.Message} Block:{fileBlock.Header.Code} Add: {fileBlock.Header.OldAddress} Offset:{fileBlock.Header.FileOffset} Size:{fileBlock.Header.Size}");
                        //Resolve duplicate:
                        FileBlock duplicate = null;
                        var found = Blocks.TryGetValue(new IntPtr((long)fileBlock.Header.OldAddress),out duplicate);
                        if (found)
                            Logging.AppendLine($"Block:{duplicate.Header.Code} Add: {duplicate.Header.OldAddress} Offset:{duplicate.Header.FileOffset} Size:{duplicate.Header.Size}");
                        else
                            Logging.AppendLine("Missing Block ???");
                    }
                    
                    fileBlock = new FileBlock(_handle, this);

                }
                //END FileBlock
                Console.WriteLine(fileBlock.Header.Code);
                Blocks.Add(new IntPtr((long)fileBlock.Header.OldAddress), fileBlock);
            }
            else
            {
                throw new NotImplementedException();
            }

            try
            {
                Map();
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.Message );
                this.Dispose();
            }
            
            Console.WriteLine(BlenderMarshal.SizeOf(typeof(Native.ID)));
            Dispose();
            
        }

        public void Map()
        {
            //Get all SC (Scene) blocks - if more than one
            //var blockBytes = ReadBytesFromPtr(new IntPtr((long)fBlock.Header.OldAddress), blockStructInDNA.Size, this);
            var _scenesBlocks = Blocks.Values.ToList().FindAll(x=>x.Header.Code == "SC");
            var blockStructInDNA = GetDNACatalog().Structures.Find(x => x.TypeName == "Scene");
            var scIndex = 0;
            foreach (var fBlock in _scenesBlocks)
            {
                ResolveStructFromBlocks(fBlock, "Scene", scIndex);
                scIndex++;
            }

                
            
            _fileStream.Seek(0, SeekOrigin.Begin);

        }

        public dynamic GetField(byte[] BlockBytes, DNAField Field)
        {
            dynamic value = null;
            if (Field.InnerType == FieldType.String ) return Encoding.ASCII.GetString(BlockBytes, Field.Offset, Field.CalculatedSize);
            else if (Field.InnerType == FieldType.ValueType)
            {
                return BitConverter.ToSingle(BlockBytes, Field.Offset);
            }
            
            return value;
        }

        public dynamic CreateManagedObject(Native.Object nativeObject)
        {
            var objectType = (ObjectType)nativeObject.type;
            switch (objectType)
            {
                case ObjectType.OB_CAMERA:
                    return new BlenderObject<Native.Camera>(nativeObject, this);
                
                default:
                    return null ;
                    //throw new InvalidOperationException($"Cannot create a managed object for unknown type: {objectType}");
            }
        }

        public byte[] GetFileBlockBytesByOffset<T>(FileBlockHeader header) 
        {
            var size = header.Size;
            //var size = Marshal.SizeOf(typeof(T));
            var offset = header.FileOffset;
            var bytes = new byte[size];
            _fileStream.Position = offset;
            _fileStream.Read(bytes, 0, (int)size);
            _fileStream.Seek(0, SeekOrigin.Begin);
            return bytes;

        }
        
        /*
        *  METHOD TO RESOLVE STRUCTS and Populate Classes
        */
        public void ResolveStructFromBlocks(FileBlock fBlock, string structType, int index =0, int depth=0 )
        {
            var blockStructInDNA = GetDNACatalog().Structures.Find(x => x.TypeName == structType);
            if (blockStructInDNA == null) return;

            Logging.AppendLine($"Resolving Block: {fBlock.Header.Code}");
            //Read Bytes of this File block (ie type: Scene)
            _handle.BaseStream.Position = fBlock.Header.FileOffset;
            var blockBytes = _handle.ReadBytes(blockStructInDNA.Size);
            //set currenct block Offset to add to the struct fields.
            var blockOffset = fBlock.Header.FileOffset;

            //Parse retreived Block and Build Struct [Embeded]
            if (blockBytes != null && blockBytes.Length > 0)
            {
                
                ResolveStructFromBytes(blockBytes, blockStructInDNA, (int)blockOffset, depth );

            }

            
        }

        public void ResolveStructFromBytes(IEnumerable<byte> bytes, DNAStructure dnaStrcture, int offset, int depth)
        {

            //Read Block with current DNA struct - fields
            //retreived data => read -> field.offset and field.calculated size
            /*
             * This is the Core function of Reading bytes and convert them to values according a struct DNA
             */
            depth++;
            foreach (var field in dnaStrcture.Fields)
            {
                var innerOffset = field.Offset + offset;
                if (field.InnerType == FieldType.Pointer )
                {
                    _handle.BaseStream.Position = innerOffset;
                    var valB = _handle.ReadBytes(field.CalculatedSize);
                    var val = new IntPtr(BitConverter.ToInt64(valB));
                    if (val != IntPtr.Zero)
                    {
                        //Get the File Block by IntPtr
                        Blocks.TryGetValue(val, out var fileBlock);
                        {
                            if (fileBlock != null)
                            {
                                
                                Logging.AppendLine($"{new string('\t', depth)} {dnaStrcture.TypeName}.{field.GetShortName()} => {fileBlock.Header.Code} {val} : {field.Type.Name}");

                                var embededStruct = GetDNACatalog().Structures.Find(x => x.TypeName == field.Type.Name);

                                if (embededStruct != null && (field.Type.Name == "Object"))
                                {
                                    Logging.AppendLine($"{new string('\t', depth)}--DATA--");
                                    _handle.BaseStream.Position=fileBlock.Header.FileOffset;
                                    var embededBytes = _handle.ReadBytes(embededStruct.Size);
                                    ResolveStructFromBytes(embededBytes, embededStruct, (int)fileBlock.Header.FileOffset, depth);
                                }

                            }
                                
                            //else Console.WriteLine($"{val} Not Found In Catalog Details {field.Name}.{field.Type.Name}");
                        }

                    }

                }
                else if (field.InnerType == FieldType.StructType) //&& field.Type.Name == "ID"
                {
                    //Here is the right place to patch - compare native with DNA derived fields.
                    //GetDNA -> Structs of the embeded DNA struct
                    var embededStruct = GetDNACatalog().Structures.Find(x => x.TypeName == field.Type.Name);
                    if (embededStruct != null)
                    {
                        Logging.AppendLine($"{new string('\t', depth)} {dnaStrcture.TypeName}.{field.Name} | {field.Type.Name}");
                        ResolveStructFromBytes(bytes, embededStruct, innerOffset, depth);

                    }


                }
                else if (field.InnerType == FieldType.Array)
                {
                    if (field.SystemType == typeof(string) && !field.IsMultiDimArray && !field.Name.Contains("_pad"))
                    {

                        _handle.BaseStream.Position = innerOffset;
                        var valB = _handle.ReadBytes(field.CalculatedSize);
                        //var val = Encoding.ASCII.GetString(valB).Trim('\0');
                        var val = BlenderMarshal.ReadString(valB, 0, field.CalculatedSize);
                        Logging.AppendLine($"{new string('\t', depth)} {dnaStrcture.TypeName}.{field.Name} => {val}");
                    }

                }
                else if (field.InnerType == FieldType.ValueType)
                {
                    object val = null;
                    _handle.BaseStream.Position = innerOffset;
                    if (field.SystemType == typeof(int)) val = _handle.ReadInt32();
                    if (field.SystemType == typeof(uint)) val = _handle.ReadUInt32();
                    if (field.SystemType == typeof(short)) val = _handle.ReadInt16();
                    if (field.SystemType == typeof(ushort)) val = _handle.ReadUInt16();
                    if (field.SystemType == typeof(float)) val = _handle.ReadSingle();
                    Logging.AppendLine($"{new string('\t', depth)} {dnaStrcture.TypeName}.{field.Name} => {val}");

                }
            }
            
        }
        private static int GetFieldSize(FieldInfo field)
        {
            var marshalAsAttribute = field.GetCustomAttribute<MarshalAsAttribute>();
            if (marshalAsAttribute != null && marshalAsAttribute.Value == UnmanagedType.ByValArray)
            {
                var elementType = field.FieldType.GetElementType();
                if (elementType == null)
                {
                    throw new InvalidOperationException("ByValArray attribute used on a non-array type.");
                }
                return marshalAsAttribute.SizeConst * Marshal.SizeOf(elementType);
            }

            if (field.FieldType == typeof(IntPtr))
            {
                return IntPtr.Size;
            }

            if (field.FieldType.IsValueType)
            {
                return Marshal.SizeOf(field.FieldType);
            }

            return 0;
        }

        

        /// <summary>
        /// Your original method, but with the final line replaced with our new parser.
        /// </summary>
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
            long size = Marshal.SizeOf<T>(); //fileBlock.Header.Size;

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


        public static byte[] ReadBytesFromPtr(IntPtr Pointer, int Size, BlendFile blendFile)
        {
            
            // Pointers can be null (IntPtr.Zero). We should handle this gracefully.
            if (Pointer == IntPtr.Zero)
            {
                
                return new byte[0];
            }

            // Look up the pointer's corresponding FileBlock in our pre-parsed index.
            if (!blendFile.Blocks.TryGetValue(Pointer, out var fileBlock))
            {
                Console.WriteLine($"Warning: Pointer {Pointer} not found in the map.");
                return new byte[0];
            }

            // Get the size of the data block from its header.
            long size = fileBlock.Header.Size; //fileBlock.Header.Size;

            // Get the file offset (position) from the header.
            long offset = fileBlock.Header.FileOffset;
            
            // CRITICAL: Set the file stream's position to the start of the data block.
            blendFile._fileStream.Position = offset;

            // Create a byte array to hold the data.
            var bytes = new byte[size];

            // Read the exact number of bytes for the data block.
            var totalBytesRead = blendFile._handle.Read(bytes, 0, (int)size);
            if (totalBytesRead != size)
            {
                Console.WriteLine($"Warning: Expected to read {size} bytes but only read {totalBytesRead} for pointer {Pointer}.");
            }

            return bytes;
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
            
            File.AppendAllText(_logFilePath +"/" + _logFileName, Logging.ToString());
            Logging.Clear();
            
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