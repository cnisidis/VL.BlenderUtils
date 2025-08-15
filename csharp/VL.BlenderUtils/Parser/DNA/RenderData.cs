using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.DNA
{
    public struct RenderData
    {
        public ImageFormatData im_format;
        public FFMpegCodecData ffcodecdata;

        public int cfra;
        public int sfra;
        public int efra;
        public float subframe;
        public int psfra;
        public int pefra;

        public int images;
        public int framapto;
        public short flag;
        public short threads;
        public float framelen;
        public int frame_step;
        public short dimensionspreset;
        public short size;

        public int xsch;
        public int ysch;

        public int tilex_deprecated;
        public int tiley_deprecated;

        public short planes_deprecated;
        public short imtype_deprecated;
        public short subimtype_deprecated;
        public short quality_deprecated;

        [MarshalAs(UnmanagedType.I1)]
        public byte use_lock_interface;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] _pad7;

        public int scemode;
        public int mode;
        public short frs_sec;

        [MarshalAs(UnmanagedType.I1)]
        public byte alphamode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] _pad0;

        public rctf border;

        public ListBase layers_deprecated;
        public short actlay_deprecated;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] _pad1;

        public float xasp;
        public float yasp;
        public float ppm_factor;
        public float ppm_base;
        public float frs_sec_base;
        public float gauss;

        public int color_mgt_flag;
        public float dither_intensity;

        public short bake_mode;
        public short bake_flag;
        public short bake_margin;
        public short bake_samples;
        public short bake_margin_type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] _pad9;
        public float bake_biasdist;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] _pad10;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        public string pic;

        public int stamp;
        public short stamp_font_id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] _pad3;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 768)]
        public byte[] stamp_udata;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] fg_stamp;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] bg_stamp;

        [MarshalAs(UnmanagedType.I1)]
        public byte seq_prev_type;
        [MarshalAs(UnmanagedType.I1)]
        public byte seq_rend_type;
        [MarshalAs(UnmanagedType.I1)]
        public byte seq_flag;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] _pad5;

        public short simplify_subsurf;
        public short simplify_subsurf_render;
        public short simplify_gpencil;
        public float simplify_particles;
        public float simplify_particles_render;
        public float simplify_volumes;

        public int line_thickness_mode;
        public float unit_line_thickness;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string engine;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] _pad2;

        public short perf_flag;
        public BakeData bake;
        public int _pad8;
        public short preview_pixel_size;
        public short _pad4;

        public ListBase views;
        public short actview;
        public short views_format;
        public short hair_type;
        public short hair_subdiv;

        public float motion_blur_shutter;
        public int motion_blur_position;
        public CurveMapping mblur_shutter_curve;

        public int compositor_device;
        public int compositor_precision;
        public int compositor_denoise_device;
        public int compositor_denoise_preview_quality;
        public int compositor_denoise_final_quality;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] _pad6;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageFormatData
    {
        // C++: char media_type;
        [MarshalAs(UnmanagedType.I1)]
        public byte media_type;
        [MarshalAs(UnmanagedType.I1)]
        public byte imtype;
        [MarshalAs(UnmanagedType.I1)]
        public byte depth;

        // C++: char planes;
        [MarshalAs(UnmanagedType.I1)]
        public byte planes;
        [MarshalAs(UnmanagedType.I1)]
        public byte flag;
        [MarshalAs(UnmanagedType.I1)]
        public byte quality;

        // C++: char compress;
        [MarshalAs(UnmanagedType.I1)]
        public byte compress;
        [MarshalAs(UnmanagedType.I1)]
        public byte exr_codec;
        [MarshalAs(UnmanagedType.I1)]
        public byte jp2_flag;
        [MarshalAs(UnmanagedType.I1)]
        public byte jp2_codec;

        // C++: char tiff_codec;
        [MarshalAs(UnmanagedType.I1)]
        public byte tiff_codec;

        // C++: char cineon_flag;
        [MarshalAs(UnmanagedType.I1)]
        public byte cineon_flag;
        public short cineon_white;
        public short cineon_black;
        public float cineon_gamma;

        // C++: char _pad[3];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] _pad;
        [MarshalAs(UnmanagedType.I1)]
        public byte views_format;
        public Stereo3dFormat stereo3d_format;
        [MarshalAs(UnmanagedType.I1)]
        public byte color_management;

        // C++: char _pad1[7];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public byte[] _pad1;
        public ColorManagedViewSettings view_settings;
        public ColorManagedDisplaySettings display_settings;
        public ColorManagedColorspaceSettings linear_colorspace_settings;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Stereo3dFormat
    {
        
        public short flag;
        public byte display_mode;
        public byte anaglyph_type;
        public byte interlace_type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] _pad;
    }

}
