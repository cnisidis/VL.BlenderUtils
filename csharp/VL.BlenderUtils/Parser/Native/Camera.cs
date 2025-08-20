
using System.Runtime.InteropServices;
using System.Threading.Tasks;
namespace VL.BlenderUtils.Parser.Native
{
    //https://github.com/blender/blender/blob/main/source/blender/makesdna/DNA_camera_types.h


    // This is crucial. It ensures the C# struct's memory layout matches
    // the C++ struct's memory layout. `LayoutKind.Sequential`
    // means fields are laid out in the order they are declared.
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Camera
    {
        
        public ID id;
        public IntPtr adt;
        [MarshalAs(UnmanagedType.I1)]    
        public CameraType type;
        
        public byte dtx;
        
        public CameraFlags flag;
        public float passepartalpha;
        public float clip_start, clip_end;
       
        public float lens;
        public float ortho_scale;
        public float drawsize;
        public float sensor_x;
        public float sensor_y;
        public float shiftx;
        public float shifty;
        [DNA_DEPRECATED]
        public float dof_distance;
        [MarshalAs(UnmanagedType.I1)]
        public SensorFit sensor_fit;
        [MarshalAs(UnmanagedType.I1)]
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
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] custom_bytecode_hash;
        public IntPtr custom_bytecode;
        public int custom_mode;
        public int _pad3;
        public IntPtr ipo;
        public IntPtr dof_ob;
        public GPUDOFSettings gpu_dof;
        // This is now a concrete struct, not a placeholder
        public CameraDOFSettings dof;
        // This is now a concrete struct, not a placeholder
        public ListBase bg_images;
        // This is now a concrete struct, not a placeholder
        public CameraStereoSettings stereo;
        // This is now a concrete struct, not a placeholder
        public Camera_Runtime runtime;
    }

    

    [StructLayout(LayoutKind.Sequential)]
    public struct CameraStereoSettings
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
    [StructLayout(LayoutKind.Sequential)]
    public struct CameraBGImage
    {
        // C++: struct CameraBGImage *next, *prev;
        // Pointers to other structs are represented as IntPtr.
        public IntPtr next;
        public IntPtr prev;

        // C++: struct Image *ima;
        public IntPtr ima;
        // C++: struct ImageUser iuser;
        public ImageUser iuser;
        // C++: struct MovieClip *clip;
        public IntPtr clip;
        // C++: struct MovieClipUser cuser;
        public MovieClipUser cuser;

        // C++: float offset[2], scale, rotation;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] offset;
        public float scale;
        public float rotation;

        // C++: float alpha;
        public float alpha;
        // C++: short flag;
        public short flag;
        // C++: short source;
        public short source;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CameraDOFSettings
    {
        // C++: struct Object *focus_object;
        public IntPtr focus_object;

        // C++: char focus_subtarget[64];
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string focus_subtarget;

        public float focus_distance;
        public float aperture_fstop;
        public float aperture_rotation;
        public float aperture_ratio;
        public int aperture_blades;
        public short flag;

        // C++: char _pad[2];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] _pad;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Camera_Runtime
    {
        // C++: float drw_corners[2][4][2];
        // A 3D array in C++. We flatten it into a 1D array in C#.
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2 * 4 * 2)]
        public float[] drw_corners;

        // C++: float drw_tria[2][2];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2 * 2)]
        public float[] drw_tria;

        // C++: float drw_depth[2];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] drw_depth;

        // C++: float drw_focusmat[4][4];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4 * 4)]
        public float[] drw_focusmat;

        // C++: float drw_normalmat[4][4];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4 * 4)]
        public float[] drw_normalmat;
    }
    public enum CameraType : byte
    {
        Perspective = 0,
        Ortho = 1,
        Panoramic = 2, 
        Custom  = 3,
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

    public enum SensorFit:byte
    {
        Auto = 0,
        Horizontal = 1,
        Vertical = 2
    }

    [Flags]
    public enum CameraFlags : short
    {
        CAM_SHOWLIMITS = (1 << 0),
        CAM_SHOWMIST = (1 << 1),
        CAM_SHOWPASSEPARTOUT = (1 << 2),
        CAM_SHOW_SAFE_MARGINS = (1 << 3),
        CAM_SHOWNAME = (1 << 4),
        CAM_ANGLETOGGLE = (1 << 5),
        CAM_DS_EXPAND = (1 << 6),
        // CAM_PANORAMA = (1 << 7), // Deprecated flag from older DNA versions.
        CAM_SHOWSENSOR = (1 << 8),
        CAM_SHOW_SAFE_CENTER = (1 << 9),
        CAM_SHOW_BG_IMAGE = (1 << 10),
    }

}
