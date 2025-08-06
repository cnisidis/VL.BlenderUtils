using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.DNA
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GPUDOFSettings
    {
        public float focus_distance;
        public float fstop;
        public float focal_length;
        public float sensor;
        public float rotation;
        public float ratio;
        public int num_blades;
        public int high_quality;

        public static GPUDOFSettings ReadGPUDOFSettings(byte[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("GPUDOFSettings data is empty.");

            int size = Marshal.SizeOf<GPUDOFSettings>();

            if (data.Length < size)
                throw new ArgumentException($"Data length {data.Length} is smaller than expected struct size {size}.");

            IntPtr ptr = IntPtr.Zero;

            try
            {
                ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(data, 0, ptr, size);
                return Marshal.PtrToStructure<GPUDOFSettings>(ptr);
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                    Marshal.FreeHGlobal(ptr);
            }
        }

        // Optional: Offset-based slice
        public static GPUDOFSettings ReadGPUDOFSettingsFromOffset(byte[] blockData, int offset)
        {
            int size = Marshal.SizeOf<GPUDOFSettings>();
            byte[] slice = new byte[size];
            Buffer.BlockCopy(blockData, offset, slice, 0, size);
            return ReadGPUDOFSettings(slice);
        }
    }
}
