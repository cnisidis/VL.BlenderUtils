using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VL.Lib.Collections;

namespace VL.BlenderUtils.Parser
{
    

    public class BlendFileParser
    {
        
        private List<FileBlock> _fileBlocks = new List<FileBlock>();
        public BlendFileHeader Header { get; }
        public List<DNA1Block> _dnaBlocks = new List<DNA1Block>();

        public string FilePath;

        public enum PtrSize
        { 
            Bit64 = 45,
            Bit32 = 95
        }


        public BlendFileParser(VL.Lib.IO.Path BlendFile)
        {
            if(BlendFile.Exists)
            {

                var directory = "";
                var filename = "";
                var extension = "";
                
                

                FilePath = BlendFile.ToString();
                

                using (var stream = new FileStream(BlendFile, FileMode.Open, FileAccess.Read))
                using (var reader = new BinaryReader(stream))
                {
                    //Extract and Assign Header to the BlendFileParser
                    Header = new BlendFileHeader(reader);

                    //Parse all FileBlocks
                    ParseFileBlocks(reader);

                }
            }
        }
        /// <summary>
        /// Returns the Absolute Path, the Directory, the Filename and its Extension.
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="Directory"></param>
        /// <param name="FileName"></param>
        /// <param name="Extension"></param>
        public void GetFilePath(out VL.Lib.IO.Path FilePath, out string Directory, out string FileName, out string Extension)
        {
            FilePath = (VL.Lib.IO.Path)this.FilePath;

            FilePath.Filename(out Directory, out FileName, out Extension);
            
        }

        /// <summary>
        /// Parse all FilaBlocks
        /// </summary>
        /// <param name="reader"></param>
        private void ParseFileBlocks(BinaryReader reader)
        {
            

            //Read till the end of file and create FileBlocks - Look for DNA1

            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                
                //Read all FileBlocks

                var fb = FileBlock.Read(this, reader);
                
                //Add FileBlocks to List
                _fileBlocks.Add(fb);
            }
            

        }

        /// <summary>
        /// Return all FileBlocks found in blend file
        /// </summary>
        /// <returns></returns>
        public Spread<FileBlock> GetFileBlocks()
        {
            return _fileBlocks.ToSpread();
        }

        /// <summary>
        /// Get only the DNA1 blocks (normally there must be only one)
        /// </summary>
        /// <returns></returns>
        public Spread<DNA1Block> GetDNABlocks()
        {
            return _dnaBlocks.ToSpread();
        }

        /// <summary>
        /// Helper Function to retrieve specific FileBlocks (by name)
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="ToUpper"></param>
        /// <returns></returns>
        public Spread<FileBlock> FindBlocks(string Code, bool ToUpper=true)
        {
            var lookFor = ToUpper ? Code.ToUpper() : Code;
            return _fileBlocks.FindAll(x=> x.code.Replace('\x00', ' ').Trim() == lookFor).ToSpread();
        }

        /// <summary>
        /// Blend File Header
        /// </summary>
        public partial class BlendFileHeader
        {
            /// <summary>
            /// Identifier
            /// </summary>
            public char[] Identifier { get; set; } // Should be "BLENDER"
            public byte PointerSize { get; set; }  // '_' (4 bytes) or '-' (8 bytes)
            public byte Endianness { get; set; }   // 'v' (little-endian) or 'V' (big-endian)
            public char[] Version { get; set; }   // Blender version (e.g., "293")


            public bool isLE { get; set; }
            public bool ptrSizeBits64 { get; set; }
            /// <summary>
            /// Empty Constructor
            /// </summary>
            public BlendFileHeader()
            {

            }
            /// <summary>
            /// Constructor from stream.
            /// </summary>
            /// <param name="reader"></param>
            public BlendFileHeader(BinaryReader reader)
            {
                Identifier = reader.ReadChars(7);
                PointerSize = reader.ReadByte();
                Endianness = reader.ReadByte();
                Version = reader.ReadChars(3);

                if (this.Endianness == 'v') this.isLE = true;
                if (this.PointerSize == '-') this.ptrSizeBits64 = true;

                // Validate the header
                if (new string(this.Identifier) != "BLENDER")
                {
                    throw new InvalidDataException("Not a valid .blend file.");
                }
            }
            /// <summary>
            /// Static Function, returns header, reads from stream.
            /// </summary>
            /// <param name="reader"></param>
            /// <returns></returns>
            /// <exception cref="InvalidDataException"></exception>
            public static BlendFileHeader Read(BinaryReader reader)
            {
                var header = new BlendFileHeader
                {
                    Identifier = reader.ReadChars(7),
                    PointerSize = reader.ReadByte(),
                    Endianness = reader.ReadByte(),
                    Version = reader.ReadChars(3)
                };

                if (header.Endianness == 'v') header.isLE = true;
                if (header.PointerSize == '-') header.ptrSizeBits64 = true;

                // Validate the header
                if (new string(header.Identifier) != "BLENDER")
                {
                    throw new InvalidDataException("Not a valid .blend file.");
                }

                return header;
            }

            public void Split(out string Identifier, out string PointerSize, out string Endianess, out string Version)
            {
                Identifier = new string(this.Identifier);
                PointerSize = new string([(char)this.PointerSize]);
                Endianess = new string([(char)this.Endianness]);
                Version = new string(this.Version);

            }
        }

        public partial class FileBlock
        {
            public string code { get; set; }
            public uint Length { get; set; }
            object memAddr { get; set; }

            uint SDNAIndex { get; set; }
            uint count { get; set; }

            BlendFileHeader Header;

            object body { get; set; }

            byte[] _rawBody { get; set; }

            public FileBlock(BlendFileHeader Header)
            {

            }

            public static FileBlock Read(BlendFileParser Parser, BinaryReader reader)
            {
                var Header = Parser.Header;

                var Fblock = new FileBlock(Header)
                {
                    code = new string(reader.ReadChars(4)),
                    Length = reader.ReadUInt32(),
                    memAddr = Header.ptrSizeBits64 ? reader.ReadUInt64() : reader.ReadUInt32(),
                    SDNAIndex = reader.ReadUInt32(),
                    count = reader.ReadUInt32(),

                };

                Fblock.ReadBody(Parser, reader);

                return Fblock;

            }
            /// <summary>
            /// Get the Raw data of the current FileBlock (body)
            /// </summary>
            /// <returns></returns>
            public Spread<byte> GetRaw()
            {
                return this._rawBody.ToSpread();
            }


            public void ReadBody(BlendFileParser Parser, BinaryReader reader)
            {

                switch (this.code)
                {
                    case "DNA1":
                        this.body = DNA1Block.FromBytes(reader, Parser);
                        Parser._dnaBlocks.Add((DNA1Block)body);
                        break;

                    default:
                        this._rawBody = reader.ReadBytes((int)this.Length);
                        this.body = _rawBody;
                        break;
                }



            }

            public void Split(out string Code, out uint Length, out uint SDNAIndex, out uint Count, out object MemoryAddress)
            {
                Code = this.code;
                Length = this.Length;
                SDNAIndex = this.SDNAIndex;
                Count = this.count;
                MemoryAddress = (dynamic)this.memAddr;



            }
        }
    }

    

   

   

    public class DNA1Block
    {
        private byte[] _id;
        private byte[] _nameMagick;
        private uint _numNames;

        private List<string> _names;
        private byte[] paddding;
        
        private byte[] _typeMagic;
        private uint _numTypes;
        private List<string> _types;

        private byte[] _tlenMagic;
        private List<ushort> _lengths;

        private byte[] _strcMagic;
        private uint _numStructs;
        private List<DNAStruct> _structs;

        public DNA1Block()
        {
            _names = new List<string>();
            _types = new List<string>();
            _lengths = new List<ushort>();
            _structs = new List<DNAStruct>();


        }
        public static DNA1Block FromBytes(BinaryReader reader, BlendFileParser root)
        {
            var DNA = new DNA1Block();
            var index = 0;

            DNA._id = reader.ReadBytes(4);
            
            DNA._nameMagick = reader.ReadBytes(4);
            
            DNA._numNames = reader.ReadUInt32();
            

            
            
            for (int i =0; i<DNA._numNames; i++)
            {
                DNA._names.Add(Helpers.ReadBytesTerm(reader,0,false, true));
            }

            var _padding1 = reader.ReadBytes( (int)Helpers.Mod(4 - reader.BaseStream.Position,4) );
            DNA._typeMagic = reader.ReadBytes(4);
            DNA._numTypes = reader.ReadUInt32();

            for (int i = 0; i < DNA._numTypes; i++)
            {
                DNA._types.Add(Helpers.ReadBytesTerm(reader, 0, false, true));
            }

            var _padding2 = reader.ReadBytes((int)Helpers.Mod(4 - reader.BaseStream.Position, 4));
            DNA._tlenMagic = reader.ReadBytes(4);

            for (int i = 0; i < DNA._numTypes; i++)
            {
                DNA._lengths.Add(reader.ReadUInt16());
            }

            var _padding3 = reader.ReadBytes((int)Helpers.Mod(4 - reader.BaseStream.Position, 4));
            DNA._strcMagic = reader.ReadBytes(4);

            DNA._numStructs = reader.ReadUInt32();
            for (int i =0; i<DNA._numStructs; i++)
            {
                DNA._structs.Add(DNAStruct.Read(reader, DNA, root));
            }

            return DNA;
        }

        public void GetNamesTypesAndStructures(out Spread<string> Names, out Spread<string> Types, out Spread<ushort> Lengths, out Spread<DNAStruct> Structures)
        {
            Names = _names.ToSpread();
            Types = _types.ToSpread();
            Lengths = _lengths.ToSpread();
            Structures = _structs.ToSpread();
        }

        public void Split(out string Id, out string MagicName, out uint NamesCount, out string MagicType, out uint TypesCount, out string MagicTypesLength, out uint StructuresCount)
        {
            Id = new string( _id.Select(x=>(Char)x).ToArray() );
            MagicName = new string(_nameMagick.Select(x => (Char)x).ToArray());
            NamesCount = _numNames;
            
            MagicType = Encoding.UTF8.GetString(_typeMagic);
            TypesCount = _numTypes;
            MagicTypesLength = new string(_tlenMagic.Select(x => (Char)x).ToArray());
            
            StructuresCount = _numStructs;

        }

    }


    public partial class DNAStruct
    {
        

        private ushort _idxType;
        private ushort _numFields;
        private List<DNAField> _fields;

        public DNAStruct()
        {

        }

       
        public static DNAStruct FromBytes(byte[] bytes)
        {
            var sdna = new DNAStruct()
            {
                _idxType = BitConverter.ToUInt16(bytes),
                _numFields = BitConverter.ToUInt16(bytes, 2),
                
            };

            

            return sdna;
        }

        public static DNAStruct Read(BinaryReader reader, DNA1Block DNA, BlendFileParser root)
        {
            var dnaStruct = new DNAStruct();
            dnaStruct._idxType = reader.ReadUInt16();
            dnaStruct._numFields = reader.ReadUInt16();
            dnaStruct._fields = new List<DNAField>();

            for (var i=0; i< dnaStruct._numFields; i++)
            {
                dnaStruct._fields.Add(new DNAField(reader, dnaStruct, root));
            }

            return dnaStruct;
        }

        public void Split(out int IndexType, out int NumberOfFields, out Spread<DNAField> Fields)
        {
            IndexType = _idxType;
            NumberOfFields = _numFields;
            Fields = _fields.ToSpread();
        }
    }

    public partial class DNAField
    {
        private ushort _idxType;
        private ushort _idxName;
        private DNAStruct Parent;
        public DNAField()
        {

        }

        public DNAField(BinaryReader reader, DNAStruct sdna, BlendFileParser root)
        {
            Parent = sdna;
            this.Read(reader);
        }

        public void Type()
        {
        }

        private void Read(BinaryReader reader)
        {
            _idxName = reader.ReadUInt16();
            _idxName = reader.ReadUInt16();
        }

        public void Split(out int IndexType, out int IndexName, out DNAStruct Parent)
        {
            IndexType = this._idxType;
            IndexName = this._idxName;
            Parent = this.Parent;
        }

    }
    
}
