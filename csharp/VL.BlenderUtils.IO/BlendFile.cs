
using Stride.Core.IO;
using Stride.Core.Serialization.Serializers;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
///For examples, see:
///https://thegraybook.vvvv.org/reference/extending/writing-nodes.html#examples
/////https://formats.kaitai.io/blender_blend/blender_blend.svg
///https://wiki.blender.jp/Dev:Source/Architecture/File_Format
namespace VL.BlenderUtils;

public struct SDNA
{

}

public class BlendFile
{

    Stream blendStream;
    byte[] blendBytes;

    public List<SDNA> SDNAStructs;
    public List<FileBlock> fileBlocks;
    Header header { get; set; }

    public BlendFile() 
    { 
        header = new Header();  
        fileBlocks = new List<FileBlock>();
    }

    
    public void Read(string filepath)
    {
        FileStream filestream = new FileStream(filepath,FileMode.Open, FileAccess.Read);
        blendStream = filestream;

        this.header.magic = GetString(0,7);
        this.header.ptrSizeId = (byte)blendStream.ReadByte();
        this.header.endianess = (byte)blendStream.ReadByte();
        this.header.version = GetString(0,3);

        ReadFileBlocks();
        
    }

    public Header GetHeader()
    {
        return this.header;
    }

    public void ReadFileBlocks()
    {
        if(blendStream.CanRead)
        {
            FileBlock fileBlock = new FileBlock();
            fileBlock.code = GetString(0,4);
            fileBlock.len_body = blendStream.ReadUInt32();
            fileBlock.sdnaIndex = blendStream.ReadUInt32();
            fileBlock.count = blendStream.ReadUInt32();
            fileBlock.sdnaStruct = new SDNA();
            if(fileBlock.code == "DNA1")
            {

            }
            fileBlocks.Add(fileBlock);  
        }
    }

    

    public class Header
    {
        public string magic { get; set; }
        public byte ptrSizeId;
        public byte endianess;
        public string version;
        public int psize;

        internal Header() { }

        public void Split(out string magic, out string version, out int ptrSize, out char endianess)
        {
            magic = this.magic;
            version = this.version;
            ptrSize = (int)this.ptrSizeId;
            endianess = (char)this.endianess;

        }
    }


    public class FileBlock
    {
        public SDNA sdnaStruct;
        public string code;
        public uint len_body;
        public int memAddress;
        public uint sdnaIndex;
        public uint count;
        public string body;

        internal FileBlock()
        {

        }

        
    }

    public class DNABody
    {
        public string id;
        public string magic;
        public uint num_names;
    }
    
    public byte[] ReadStream(int offset, int count)
    {
        byte[] bytes = new byte[count];
        blendStream.Read(bytes, offset, count);
        return bytes;
    }

    public int GetInt32(int offset, bool littleEndian=false)
    {
        var bytes = ReadStream(offset, 4);
        if(littleEndian) 
        {
            bytes.Reverse();
        }
        return BitConverter.ToInt32(bytes);
    }
    public Single GetFloat(int offset, bool littleEndian = false)
    {
        var bytes = ReadStream(offset, 4);
        if (littleEndian)
        {
            bytes.Reverse();
        }
        return BitConverter.ToSingle(bytes);
    }

    public Byte GetByte(int offset)
    {
        var bytes = ReadStream(offset, 1); 
        return bytes[0];
    }

    public String GetString(int offset, int count)
    {
        var bytes = ReadStream(offset, count);
        
        return Encoding.UTF8.GetString(bytes);
    }

    
}