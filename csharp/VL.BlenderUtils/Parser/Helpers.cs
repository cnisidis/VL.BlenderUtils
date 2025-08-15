using Stride.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VL.BlenderUtils.Parser.DNA;
using static VL.BlenderUtils.Parser.Pythonic.BlendFile;

namespace VL.BlenderUtils.Parser
{

    public enum ReaderType
    {
        US,
        S,
        UI,
        I,
        F,
        UL,
        L,
        P,
    }
    public static class Reader
    {


        public static dynamic ReadBlock(BinaryReader reader, string classType, Pythonic.BlendFile.Header header, ulong fromPointer = 0)
        {
            var position = reader.BaseStream.Position;
            dynamic block = null;
            

            

            reader.BaseStream.Seek(position, SeekOrigin.Begin);
            return block;
        }
        public static string ReadString(BinaryReader reader, int length=0)
        {
            if (length != 0)
            {
                var result = Encoding.UTF8.GetString(reader.ReadBytes(length));
                return result;

            }

             
            else
            {
                string result = string.Empty;
                string s = Reader.ReadString(reader, 1);

                while ( s[0] != '\0')
                {
                    result += s;
                    s = Reader.ReadString(reader, 1);
                }
                
                return result;
            }

        }
        public static dynamic Read(ReaderType type, BinaryReader reader, Pythonic.BlendFile.Header header)
        {

            if (type == ReaderType.US)
                return reader.ReadUInt16();

            else if (type == ReaderType.S)
                return reader.ReadInt16();

            else if(type == ReaderType.UI)
                return reader.ReadUInt32();
            
            else if(type == ReaderType.I)
                return reader.ReadInt32();
            
            else if(type == ReaderType.F)
                return reader.ReadSingle();
            
            else if(type == ReaderType.UL)
                return reader.ReadUInt64();
            
            else if(type == ReaderType.L)
                return reader.ReadInt64();
            
            else if(type == ReaderType.P)
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
            var offset = reader.BaseStream.Position ;
            
            long alignedOffset = (offset + 3) & ~3U;

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

    

    public static class Helpers
    {

        /// <summary>
        /// Marshals data from a byte array to a struct, starting at a specific offset.
        /// This method pins the byte array to avoid memory copying, making it efficient.
        /// </summary>
        /// <typeparam name="T">The type of the struct to marshal.</typeparam>
        /// <param name="byteArray">The source byte array.</param>
        /// <param name="offset">The offset in the array to begin reading from.</param>
        /// <returns>A new instance of the struct populated with the data.</returns>
        public static T BytesToStruct<T>(byte[] byteArray, int offset) where T : struct
        {
            // Allocate a GCHandle to pin the byte array in memory.
            GCHandle handle = GCHandle.Alloc(byteArray, GCHandleType.Pinned);
            try
            {
                // Get a pointer to the start of the pinned byte array.
                IntPtr pointer = handle.AddrOfPinnedObject();

                // Add the offset to the pointer to get the correct starting address for the struct.
                pointer = IntPtr.Add(pointer, offset);

                // Marshal the data from the pointer into the struct.
                return (T)Marshal.PtrToStructure(pointer, typeof(T));
            }
            finally
            {
                // IMPORTANT: Always free the handle in a finally block to unpin the array.
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }

        // The key pointer resolution method

        public static object BYTEARRAY(IEnumerable<byte> bytes, ref int index, int Size)
        {
            var idx = index;
            index += Size;
            return bytes.Skip(idx);
        }
        public static char[] PAD(IEnumerable<byte> bytes, ref int index, int size)
        {
            var idx = index;
            index += size;
            return bytes.Skip(idx).Take(size).Select(x=>(char)x).ToArray();
        }
        public static char CHAR(IEnumerable<byte> bytes, ref int index)
        {
            var idx = index;
            index += 1;
            return (char)bytes.ToArray()[idx];
        }
        
        public static float FLOAT(IEnumerable<byte> bytes, ref int index)
        {
            var idx = index;
            index += 4;
            return BitConverter.ToSingle(bytes.Skip(idx).ToArray());
        }

        public static IntPtr POINTER(IEnumerable<byte> bytes, ref int index, bool Bits64 = true)
        {
            var idx = index;

            if(Bits64)
            {
                
                index += 8;
                return (IntPtr)BitConverter.ToInt64(bytes.Skip(idx).ToArray());
            }
            else
            {
                index += 4;
                return (IntPtr)BitConverter.ToInt32(bytes.Skip(idx).ToArray());
            }
        }

        public static UInt32 SwapBytes(UInt32 value)
        {
            var bytes = BitConverter.GetBytes(value);
            return BitConverter.ToUInt32(bytes.Reverse().ToArray());
        }

        public static Int32 SwapBytes(Int32 value)
        {
            var bytes = BitConverter.GetBytes(value);
            return BitConverter.ToInt32(bytes.Reverse().ToArray());
        }

        public static UInt64 SwapBytes(UInt64 value)
        {
            var bytes = BitConverter.GetBytes(value);
            return BitConverter.ToUInt64(bytes.Reverse().ToArray());
        }

        public static UInt16 SwapBytes(UInt16 value)
        {
            var bytes = BitConverter.GetBytes(value);
            return BitConverter.ToUInt16(bytes.Reverse().ToArray());
        }

        public static string ReadBytesTerm(BinaryReader reader, byte term=0, bool IncludeTerm=false, bool ConsumeTerm=true)
        {
            List<byte> _bytes = new List<byte>();
            
            
            
            while(reader.BaseStream.Position < reader.BaseStream.Length)
            {
                byte b = reader.ReadByte();

                if (b == term)
                {
                    if (IncludeTerm) _bytes.Add(b);
                    if (!ConsumeTerm) reader.BaseStream.Position -=1;
                    break;
                }
                _bytes.Add(b);
                
            } 

            return Encoding.ASCII.GetString(_bytes.ToArray());
        }

        public static int Mod(int a, int b)
        {
            if (b <= 0) throw new ArgumentException("Divisor of mod operation must be greater than zero.", "b");
            int r = a % b;
            if (r < 0) r += b;
            return r;
        }

        public static long Mod(long a, long b)
        {
            if (b <= 0) throw new ArgumentException("Divisor of mod operation must be greater than zero.", "b");
            long r = a % b;
            if (r < 0) r += b;
            return r;
        }
    }
}
