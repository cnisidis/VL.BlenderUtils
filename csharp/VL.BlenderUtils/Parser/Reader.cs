using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser
{
    public static class Reader
    {


        public static dynamic ReadBlock(BinaryReader reader, string classType, BlendFile.Header header, ulong fromPointer = 0)
        {
            var position = reader.BaseStream.Position;
            dynamic block = null;




            reader.BaseStream.Seek(position, SeekOrigin.Begin);
            return block;
        }
        public static string ReadString(BinaryReader reader, int length = 0)
        {
            if (length != 0)
            {
                var result = Encoding.UTF8.GetString(reader.ReadBytes(length));
                return result;

            }


            else
            {
                string result = string.Empty;
                string s = ReadString(reader, 1);

                while (s[0] != '\0')
                {
                    result += s;
                    s = ReadString(reader, 1);
                }

                return result;
            }

        }
        public static dynamic Read(ReaderType type, BinaryReader reader, BlendFile.Header header)
        {

            if (type == ReaderType.US)
                return reader.ReadUInt16();

            else if (type == ReaderType.S)
                return reader.ReadInt16();

            else if (type == ReaderType.UI)
                return reader.ReadUInt32();

            else if (type == ReaderType.I)
                return reader.ReadInt32();

            else if (type == ReaderType.F)
                return reader.ReadSingle();

            else if (type == ReaderType.UL)
                return reader.ReadUInt64();

            else if (type == ReaderType.L)
                return reader.ReadInt64();

            else if (type == ReaderType.P)
            {


                if (header.LittleEndianess)
                    return reader.ReadUInt64();
                else
                    return (ulong)reader.ReadUInt32();
            }

            throw new NotImplementedException();


        }

        public static dynamic ReadBytes(BinaryReader reader, int Size)
        {
            return reader.ReadBytes(Size);
        }

        public static void Align(BinaryReader reader)
        {
            var offset = reader.BaseStream.Position;

            long alignedOffset = offset + 3 & ~3U;

            reader.BaseStream.Position = alignedOffset;



            Console.WriteLine(" Offset->{0:G} | Alligned->{1:G}", offset, alignedOffset);
        }

        //In summary, alignment needs to be relative to the starting offset of SDNA, so it seems that this script used to work merely by coincidence.
        //https://stackoverflow.com/questions/79561711/alignment-is-wrong-when-trying-to-get-offset-while-parsing-blender-file-v4-4/79563536#79563536
        public static void AlignAlt(BinaryReader reader, long startOffset)
        {
            var offset = reader.BaseStream.Position - startOffset;

            var trim = offset % 4;
            if (trim != 0)
                reader.BaseStream.Seek(4 - trim, SeekOrigin.Current);

        }

        public static long Mod(long a, long b)
        {
            long remainder = a % b;
            return remainder >= 0 ? remainder : remainder + b;
        }

    }


}
