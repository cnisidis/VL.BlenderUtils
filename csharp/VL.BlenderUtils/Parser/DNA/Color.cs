using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.DNA
{
    // C++: typedef struct ColorManagedViewSettings
    [StructLayout(LayoutKind.Sequential)]
    public struct ColorManagedViewSettings
    {
        // C++: int flag;
        public int flag;
        // C++: char _pad[4];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] _pad;
        // C++: char look[64];
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string look;
        // C++: char view_transform[64];
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string view_transform;
        public float exposure;
        public float gamma;
        public float temperature;
        public float tint;
        public IntPtr curve_mapping;
        public IntPtr _pad2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CurveMapping
    {
        public float rangex, rangey;
        public rctf rect;
        public ListBase points;
        public int flag;
        public int cur;
        public int totpoint;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ColorManagedDisplaySettings
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string display_device;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct ColorManagedColorspaceSettings
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string name;
    }
}
