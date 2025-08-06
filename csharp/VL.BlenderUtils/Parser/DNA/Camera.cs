
using System.Runtime.InteropServices;
namespace VL.BlenderUtils.Parser.DNA
{
    //https://github.com/blender/blender/blob/main/source/blender/makesdna/DNA_camera_types.h
    [StructLayout(LayoutKind.Sequential)]
    public class Camera
    {
        public ID id;
        public IntPtr adt;

        public byte type;
        public byte dtx;
        public short flag;

        public float passepartalpha;
        public float clip_start;
        public float clip_end;
        public float lens;
        public float ortho_scale;
        public float drawsize;
        public float sensor_x;
        public float sensor_y;
        public float shiftx;
        public float shifty;
        public float dof_distance; // deprecated

        public byte sensor_fit;
        public byte panorama_type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] _pad;

        public float fisheye_fov;
        public float fisheye_lens;
        public float latitude_min;
        public float latitude_max;
        public float longitude_min;
        public float longitude_max;
        public float fisheye_polynomial_k0;
        public float fisheye_polynomial_k1;
        public float fisheye_polynomial_k2;
        public float fisheye_polynomial_k3;
        public float fisheye_polynomial_k4;

        public float central_cylindrical_range_u_min;
        public float central_cylindrical_range_u_max;
        public float central_cylindrical_range_v_min;
        public float central_cylindrical_range_v_max;
        public float central_cylindrical_radius;
        public float _pad2;

        public IntPtr custom_shader;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        public string custom_filepath;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string custom_bytecode_hash;

        public IntPtr custom_bytecode;
        public int custom_mode;
        public int _pad3;

        public IntPtr ipo; // deprecated
        public IntPtr dof_ob; // deprecated
        public GPUDOFSettings gpu_dof; // deprecated
        public CameraDOFSettings dof;

        public ListBase bg_images;
        public CameraStereoSettings stereo;

        public Camera_Runtime runtime;

        public Camera()
        {

        }
        public static Camera ReadCamera(byte[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Camera data is empty.");

            int size = Marshal.SizeOf<Camera>();

            if (data.Length < size)
                throw new ArgumentException($"Data length {data.Length} is smaller than expected Camera struct size {size}.");

            IntPtr ptr = IntPtr.Zero;

            try
            {
                // Allocate unmanaged memory
                ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(data, 0, ptr, size);

                // Marshal bytes into Camera object
                return Marshal.PtrToStructure<Camera>(ptr);
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                    Marshal.FreeHGlobal(ptr);
            }
        }

        public static Camera ParseCamera(byte[] data)
        {
            var index = 0;
            return new Camera()
            {
                

            };
        }


        public void Split(out ID Id, out CameraType Type, out Stride.Core.Mathematics.Vector2 Cliping, out float Lens, out Stride.Core.Mathematics.Vector2 Sensor, out Stride.Core.Mathematics.Vector2 Shift)
        {
            Id = this.id;
            Type = (CameraType)this.type;
            Cliping = new Stride.Core.Mathematics.Vector2(this.clip_start, this.clip_end);
            Lens = this.lens;
            Sensor = new Stride.Core.Mathematics.Vector2(this.sensor_x, this.sensor_y);
            Shift = new Stride.Core.Mathematics.Vector2(this.shiftx, this.shifty);
        }

    }

    public enum CameraType
    {
        Perspective = 0,
        Ortho = 1,
        Panoramic = 2
    }

    public enum dtx
    {
        Center = 1 << 0,
        CenterDiag = 1 << 1,
        Thirds = 1 << 2,
        Golden = 1 << 3,
        GoldenTriA = 1 << 4,
        GoldenTriB = 1 << 5,
        HarmonyTriA = 1 << 6,
        HarmonyTriB = 1 << 7,

    }


    public enum CameraFlag
    {
        ShowLimits = 1 << 0,
        ShowMist = 1 << 1,
        ShowPassepartout = 1 << 2
    }

    public enum SensorFit
    {
        Auto = 0,
        Horizontal = 1,
        Vertical = 2
    }

    [StructLayout(LayoutKind.Sequential)]
    public class Camera_Runtime
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2 * 4 * 2)]
        public float[] drw_corners;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2 * 2)]
        public float[] drw_tria;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] drw_depth;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public float[] drw_focusmat;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public float[] drw_normalmat;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class CameraDOFSettings
    {
        public IntPtr focus_object; // Object*
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string focus_subtarget;
        public float focus_distance;
        public float aperture_fstop;
        public float aperture_rotation;
        public float aperture_ratio;
        public int aperture_blades;
        public short flag;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] _pad;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class CameraBGImage
    {
        public IntPtr next;
        public IntPtr prev;

        public IntPtr ima; // Image*
        //public ImageUser iuser;
        public IntPtr clip; // MovieClip*
        //public MovieClipUser cuser;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] offset;
        public float scale;
        public float rotation;
        public float alpha;
        public short flag;
        public short source;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class CameraStereoSettings
    {
        public float interocular_distance;
        public float convergence_distance;
        public short convergence_mode;
        public short pivot;
        public short flag;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] _pad;
        public float pole_merge_angle_from;
        public float pole_merge_angle_to;
    }
}
