
//https://github.com/blender/blender/blob/main/doc/blender_file_format/BlendFileReader.py
using System.Runtime.InteropServices;
using VL.BlenderUtils.Parser.DNA;
using VL.BlenderUtils.Parser.Native;
using VL.Lib.Collections;
using VL.BlenderUtils.Parser.Logger;

namespace VL.BlenderUtils.Parser
{
    public class BlendFile : IDisposable
    {
        private Logger.Logger Log;
        public Header header { get; set; }
        //List<FileBlock> blocks { get; set; }
        public List<Managed.Object> Objects { get; set; }
        public List<Managed.Scene> Scenes { get; set; }
        public Patcher Patcher { get; set; }
        private bool FoundDnaBlock = false;
        DNACatalog Catalog;
        private FileStream _fileStream;
        private BinaryReader _handle;
        private Dictionary<IntPtr, FileBlock> Blocks;
       
        public BlendFile()
        {
            Objects = new List<Managed.Object>();
            Blocks = new Dictionary<IntPtr, FileBlock>();
            Scenes = new();
            Log = new Logger.Logger();
        }

        public void OpenBlendFile(string blendFile)
        {
            Blocks.Clear();

                _fileStream = new FileStream(blendFile, FileMode.Open, FileAccess.Read, FileShare.None);
            _handle = new BinaryReader(_fileStream) ;
           
            var magic = Reader.ReadString(_handle, 7);
            if (magic.Contains("BLENDER") || magic.Contains("BULLETf"))
            {
                Log.Add("Normal blendfile detected");
                _handle.BaseStream.Seek(0, SeekOrigin.Begin);
                
                header = new Header(_handle);

                Log.Add($"Version: {header.Version} | LittleEndianess: {header.LittleEndianess} | Pointer Size: {header.PointerSize}");

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
                        
                        Log.Add($"{ex.Message} Block:{fileBlock.Header.Code} Add: {fileBlock.Header.OldAddress} Offset:{fileBlock.Header.FileOffset} Size:{fileBlock.Header.Size}", LogType.error);
                        //Resolve duplicate:
                        FileBlock duplicate = null;
                        var found = Blocks.TryGetValue(new IntPtr((long)fileBlock.Header.OldAddress),out duplicate);
                        if (found)
                            Log.Add($"Block:{duplicate.Header.Code} Add: {duplicate.Header.OldAddress} Offset:{duplicate.Header.FileOffset} Size:{duplicate.Header.Size}", LogType.error);
                        else
                            Log.Add("Missing Block ???", LogType.error);
                    }
                    
                    fileBlock = new FileBlock(_handle, this);

                }
                //END FileBlock
                Console.WriteLine(fileBlock.Header.Code);
                Log.Add("--File Loaded");
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
            //Initialize Patcher [Patcher is responsible to collect all valeus according to the BlendFile's DNA ]
            Patcher = new Patcher();
            var _camBlocks = Blocks.Values.ToList().FindAll(x => x.Header.Code == "CA");
            var camIndex = 0;

            foreach (var fBlock in _camBlocks)
            {
                Patcher.Objects.Add(ResolveStructFromBlock(fBlock, "Camera", camIndex));
                camIndex++;
            }

            var _objBlocks = Blocks.Values.ToList().FindAll(x => x.Header.Code == "OB");
            var obIndex = 0;

            foreach (var fBlock in _objBlocks)
            {
                Patcher.Objects.Add(ResolveStructFromBlock(fBlock, "Object", obIndex));
                obIndex++;
            }

            var _scenesBlocks = Blocks.Values.ToList().FindAll(x=>x.Header.Code == "SC");
            var blockStructInDNA = GetDNACatalog().Structures.Find(x => x.TypeName == "Scene");
            var scIndex = 0;

            foreach (var fBlock in _scenesBlocks)
            {
                Patcher.Objects.Add(ResolveStructFromBlock(fBlock, "Scene", scIndex));
                scIndex++;
            }

            if (Patcher.Objects != null && Patcher.Objects.Count > 0)
            {
                foreach (var obj in Patcher.Objects)
                {
                    var type = obj.GetValue("structType");
                    var id = obj.GetObject("id");
                    var name = id.GetValue("name");
                    var uuid = id.GetValue("session_uid");

                    Console.WriteLine($"{uuid} {type} {name}");

                    if (type.ToString() == "Object")
                    {
                        if(obj.GetObject("data") != null)
                        {
                            var dataId = obj.GetObject("data").GetObject("id").GetValue("session_uid");
                            var dataName = obj.GetObject("data").GetObject("id").GetValue("name");
                            var dataType = (ObjectType)obj.GetValue("type");
                            Console.WriteLine($"\t{dataId} {dataName} {dataType} ");
                        }
                        
                        Objects.Add(Managed.Object.FromDummyObject(obj));
                    }
                    else if (type.ToString() == "Scene")
                    {

                        var scene = Managed.Scene.FromDummyObject(obj);
                        var camId = obj.GetObject("camera") == null ? 0 : obj.GetObject("camera").GetObject("id").GetValue("session_uid");
                        Console.Write($"Camera Obj Id:{camId}\t");
                        Scenes.Add(scene);
                    }
                }
            }

            _fileStream.Seek(0, SeekOrigin.Begin);
            Log.Add("--Mapping Completed--");
        }

        /*
        *  METHOD TO RESOLVE STRUCTS and Populate Classes
        */
        public Patcher.DNADummyObject ResolveStructFromBlock(FileBlock fBlock, string structType, int index =0, int depth=0)
        {
            Patcher.DNADummyObject dummy = null;
            var blockStructInDNA = GetDNACatalog().Structures.Find(x => x.TypeName == structType);
            if (blockStructInDNA == null) return dummy;
            depth++;

            Log.Add($"{new string('\t', depth)} --- Resolving Block: {fBlock.Header.Code}");
            //Read Bytes of this File block (ie type: Scene)
            _handle.BaseStream.Position = fBlock.Header.FileOffset;
            var blockBytes = _handle.ReadBytes(blockStructInDNA.Size);
            //set currenct block Offset to add to the struct fields.
            var blockOffset = fBlock.Header.FileOffset;

            //Parse retreived Block and Build Struct [Embeded]
            if (blockBytes != null && blockBytes.Length > 0)
            {
                dummy = new(blockStructInDNA.TypeName);
                dummy.AddField("structType", structType);
                ResolveStructFromBytes(blockBytes, blockStructInDNA, (int)blockOffset, depth, ref dummy );
            }
            
            return dummy;
        }

        public void ResolveStructFromBytes(IEnumerable<byte> bytes, DNAStructure dnaStrcture, int offset, int depth,ref Patcher.DNADummyObject parentDummy)
        {
            
            //Read Block with current DNA struct - fields
            //retreived data => read -> field.offset and field.calculated size
            /*
             * This is the Core function of Reading bytes and convert them to values according a struct DNA
             */
            //Patcher.DNADummyObject dummy = new Patcher.DNADummyObject(dnaStrcture.TypeName);
            depth++;
            foreach (var field in dnaStrcture.Fields)
            {
                var innerOffset = field.Offset + offset;
                object dummyFieldVal = null;
                if (field.InnerType == FieldType.Pointer || field.InnerType == FieldType.Void)
                {
                    _handle.BaseStream.Position = innerOffset;
                    var valB = _handle.ReadBytes(field.CalculatedSize);
                    var val = new IntPtr(BitConverter.ToInt64(valB));
                    if ((IntPtr)val != IntPtr.Zero)
                    {
                        //Get the File Block by IntPtr
                        Blocks.TryGetValue((IntPtr)val, out var fileBlock);
                        {
                            if (fileBlock != null)
                            {
                                
                                Log.Add($"{new string('\t', depth)} {dnaStrcture.TypeName}.{field.GetShortName()}:{field.InnerType} => {fileBlock.Header.Code} {val} : {field.Type.Name}");

                                var fTypeName = field.Type.Name;
                                if (fTypeName == "void" && field.GetShortName()=="data" && dnaStrcture.TypeName == "Object")
                                {

                                    var type = (ObjectType)parentDummy.GetValue("type");
                                    Log.Add($"{new string('\t', depth)} maybe data of type {type}");
                                    if (type != null)
                                    {
                                        if(type == ObjectType.OB_CAMERA)
                                        {
                                            Log.Add($"{new string('\t', depth)} Look for IntPtr {val}");
                                            GetDNACatalog().Structures.Find(x=>x.TypeName == "Camera");
                                            Blocks.TryGetValue((IntPtr)val, out var cameraBlock);
                                            if(cameraBlock != null)
                                            {
                                                
                                                var cam = ResolveStructFromBlock(cameraBlock, "Camera", 0, depth);
                                                dummyFieldVal = cam;
                                            }
                                        }
                                        else if(type == ObjectType.OB_MESH)
                                        {
                                            Log.Add($"{new string('\t', depth)} Look for IntPtr {val}");
                                            GetDNACatalog().Structures.Find(x => x.TypeName == "Mesh");
                                            Blocks.TryGetValue((IntPtr)val, out var meshBlock);
                                            if (meshBlock != null)
                                            {

                                                var mesh = ResolveStructFromBlock(meshBlock, "Mesh", 0, depth);
                                                dummyFieldVal = mesh;
                                            }
                                        }
                                        else
                                        {
                                            Log.Add($"{new string('\t', depth)} maybe data of type {type}");
                                        }
                                    }
                                      
                                }
                                else if(field.GetShortName() == "data")
                                {
                                    Log.Add( $"{new string('\t', depth)} SOME DATA OBVIOUSLY {field.Type.Name}");
                                }

                                var embededStruct = GetDNACatalog().Structures.Find(x => x.TypeName == fTypeName);
                                if (embededStruct != null && (field.Type.Name == "Object"))
                                {
                                    var subDummy = new Patcher.DNADummyObject(field.GetShortName());
                                    _handle.BaseStream.Position=fileBlock.Header.FileOffset;
                                    var embededBytes = _handle.ReadBytes(embededStruct.Size);
                                    ResolveStructFromBytes(embededBytes, embededStruct, (int)fileBlock.Header.FileOffset, depth, ref subDummy);
                                    dummyFieldVal = subDummy;
                                }
                                else if(embededStruct != null && embededStruct.TypeName == "Mesh")
                                {
                                    Log.Add($"{new string('\t', depth)} NOT an Object {field.Type.Name}");
                                    if(true)
                                    {
                                        Log.Add($"{new string('\t', depth)} A CUSTOM DATA LAYER");
                                        var subDummy = new Patcher.DNADummyObject(field.GetShortName());
                                        _handle.BaseStream.Position = fileBlock.Header.FileOffset;
                                        var embededBytes = _handle.ReadBytes(embededStruct.Size);
                                        ResolveStructFromBytes(embededBytes, embededStruct, (int)fileBlock.Header.FileOffset, depth, ref subDummy);
                                        dummyFieldVal = subDummy;
                                    }
                                }
                            }                          
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
                        var subDummy = new Patcher.DNADummyObject(field.GetShortName());
                        Log.Add($"{new string('\t', depth)} {field.InnerType}:{dnaStrcture.TypeName}.{field.Name} | {field.Type.Name}");
                        ResolveStructFromBytes(bytes, embededStruct, innerOffset, depth, ref subDummy);
                        dummyFieldVal= subDummy;
                        
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
                        Log.Add($"{new string('\t', depth)} {dnaStrcture.TypeName}.{field.Name} => {val} : {field.SystemType}");
                        dummyFieldVal = val;
                    }
                    else if (!field.IsMultiDimArray && !field.Name.Contains("_pad"))
                    {
                        _handle.BaseStream.Position = innerOffset;
                        var valB = _handle.ReadBytes(field.CalculatedSize);
                        if(field.SystemType == typeof(float[]))
                        {
                            var val = (float[])BlenderMarshal.ReadArray<float>(valB, 0, field.CalculatedSize);
                            dummyFieldVal = val;
                        }
                            
                        Log.Add($"{new string('\t', depth)} {dnaStrcture.TypeName}.{field.Name} =>  : {field.SystemType}[{field.ArraySizes.FirstOrDefault()}]");
                    }
                    else if(field.IsMultiDimArray && !field.Name.Contains("_pad"))
                    {
                        _handle.BaseStream.Position = innerOffset;
                        var valB = _handle.ReadBytes(field.CalculatedSize);
                        Log.Add($"{new string('\t', depth)} {dnaStrcture.TypeName}.{field.Name} =>  : Multi -> {field.SystemType} {string.Join(",", field.ArraySizes.SelectMany(x=>x.ToString()))}");
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
                    if (field.SystemType == typeof(char)) val = _handle.ReadByte();
                    if (field.SystemType == typeof(byte)) val = _handle.ReadByte();
                    Log.Add($"{new string('\t', depth)} {dnaStrcture.TypeName}.{field.Name} => {val} : {field.SystemType}");
                    dummyFieldVal = val;    

                }

                parentDummy.AddField(field.GetShortName(), dummyFieldVal);
            }
                  
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
        public Spread<Managed.Object> GetObjects()
        {
            return this.Objects.ToSpread();
        }
        public Spread<Managed.Scene> GetScenes()
        {
            return this.Scenes.ToSpread();
        }

        public Spread<Managed.Camera> GetCameras()
        {
            return Objects.OfType<Managed.Camera>().ToSpread();
        }

        public void Dispose()
        {
            _fileStream.Dispose();
            _handle.Dispose();
            Console.WriteLine("--Parser Disposed--");
                
            Log.Add("--Disposed--");
            Log.ToFile();
            Log.Dispose();
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