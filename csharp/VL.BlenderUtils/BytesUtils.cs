

namespace VL.BlenderUtils
{
    public enum Endianess
    {
        B,
        L
    }

    
    public static class BytesUtils
    {
        public static IEnumerable<byte> SwapBytes(IEnumerable<byte> bytes)
        {
            return bytes.Reverse();
        }
        public static float ReadFloat(IEnumerable<byte> bytes, Endianess endianess)
        {
            if(endianess == Endianess.L)
            {
                return BitConverter.ToSingle(bytes.ToArray());
            }
            else
            {
                return BitConverter.ToSingle(SwapBytes(bytes).ToArray());
            }
        }

        public static UInt32 ReadUInt32(IEnumerable<byte> bytes, Endianess endianess)
        {
            if (endianess == Endianess.L)
            {
                return BitConverter.ToUInt32(bytes.ToArray());
            }
            else
            {
                return BitConverter.ToUInt32(SwapBytes(bytes).ToArray());
            }
        }

        public static Int32 ReadInt32(IEnumerable<byte> bytes, Endianess endianess)
        {
            if (endianess == Endianess.L)
            {
                return BitConverter.ToInt32(bytes.ToArray());
            }
            else
            {
                return BitConverter.ToInt32(SwapBytes(bytes).ToArray());
            }
        }

        public static UInt16 ReadUInt16(IEnumerable<byte> bytes, Endianess endianess)
        {
            if (endianess == Endianess.L)
            {
                return BitConverter.ToUInt16(bytes.ToArray());
            }
            else
            {
                return BitConverter.ToUInt16(SwapBytes(bytes).ToArray());
            }
        }

        public static Int16 ReadInt16(IEnumerable<byte> bytes, Endianess endianess)
        {
            if (endianess == Endianess.L)
            {
                return BitConverter.ToInt16(bytes.ToArray());
            }
            else
            {
                return BitConverter.ToInt16(SwapBytes(bytes).ToArray());
            }
        }
    }
}
