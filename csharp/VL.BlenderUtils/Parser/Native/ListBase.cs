

using System.Runtime.InteropServices;

namespace VL.BlenderUtils.Parser.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LinkData
    {
        public IntPtr data;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct ListBase
    {
        public IntPtr first;
        public IntPtr last;

    }  
}
