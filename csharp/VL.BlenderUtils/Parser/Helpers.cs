﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser
{
    public static class Helpers
    {
        

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
