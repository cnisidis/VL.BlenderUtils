using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct View3DCursor
    {
        // C++: float location[3];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] location;

        // C++: float rotation_quaternion[4];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] rotation_quaternion;

        // C++: float rotation_euler[3];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] rotation_euler;

        // C++: float rotation_axis[3];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] rotation_axis;

        // C++: float rotation_angle;
        public float rotation_angle;

        // C++: short rotation_mode;
        public short rotation_mode;

        // C++: char _pad[6];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] _pad;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct View3DShading
    {
        public byte type;
        public byte prev_type;
        public byte prev_type_wire;
        public byte color_type;
        public short flag;
        public byte light;
        public byte background_type;
        public byte cavity_type;
        public byte wire_color_type;
        public byte use_compositor;
        public byte _pad;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string studio_light;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string lookdev_light;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string matcap;
        public float shadow_intensity;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] single_color;
        public float studiolight_rot_z;
        public float studiolight_background;
        public float studiolight_intensity;
        public float studiolight_blur;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] object_outline_color;
        public float xray_alpha;
        public float xray_alpha_wire;
        public float cavity_valley_factor;
        public float cavity_ridge_factor;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] background_color;
        public float curvature_ridge_factor;
        public float curvature_valley_factor;
        public int render_pass;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string aov_name;
        public IntPtr prop;
        public IntPtr _pad2;
    }
}
