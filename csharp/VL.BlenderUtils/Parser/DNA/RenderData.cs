using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.DNA
{
    public class RenderData
    {
        byte[] im_format; //ImageFormatData

        object _pad;
        byte[] ffcodecdata; //FFMpegCodecData

        int cfra, sfra, efra;
        float subframe;
        /** Start+end frames of preview range. */
        int psfra, pefra;

        int images, framapto;
        short flag, threads;

        float framelen;
        
        /** Frames to jump during render/playback. */
        int frame_step;

        /** For the dimensions presets menu. */
        short dimensionspreset;

        /** Size in %. */
        short size;

        /* From buttons: */
        /**
         * The desired number of pixels in the x direction
         */
        int xsch;
        /**
         * The desired number of pixels in the y direction
         */
        int ysch;

        /**
         * render tile dimensions
         */
        int tilex; //DNA_DEPRECATED;
        int tiley; //DNA_DEPRECATED;

        short planes; //DNA_DEPRECATED;
        short imtype; //DNA_DEPRECATED;
        short subimtype; //DNA_DEPRECATED;
        short quality; //DNA_DEPRECATED;

        char use_lock_interface;
        char[] _pad7 = new char[3];
        /**
       * Flags for render settings. Use bit-masking to access the settings.
       */
        int scemode;

        /**
         * Flags for render settings. Use bit-masking to access the settings.
         */
        int mode;

        short frs_sec;

        /**
         * What to do with the sky/background.
         * Picks sky/pre-multiply blending for the background.
         */
        char alphamode;

        char[] _pad0 = new char[1];

        /**
   * Adjustment factors for the aspect ratio in the x direction, was a short in 2.45
   */
        float xasp, yasp;

        float frs_sec_base;

        /**
         * Value used to define filter size for all filter options.
         */
        float gauss;

        /** Color management settings - color profiles, gamma correction, etc. */
        int color_mgt_flag;

        /** Dither noise intensity. */
        float dither_intensity;

        /* Bake Render options. */
        short bake_mode, bake_flag;
        short bake_margin, bake_samples;
        short bake_margin_type;
        char[] _pad9 = new char[6];
        float bake_biasdist, bake_user_scale;

        /* Path to render output. */
        /** 1024 = FILE_MAX. */
        /* NOTE: Excluded from `BKE_bpath_foreach_path_` / `scene_foreach_path` code. */
        char[] pic = new char[1024];

        /** Stamps flags. */
        int stamp;
        /** Select one of blenders bitmap fonts. */
        short stamp_font_id;
        char[] _pad3 = new char[2];

        /** Stamp info user data. */
        char[] stamp_udata = new char[768];

        /* Foreground/background color. */
        float fg_stamp;
        float bg_stamp;

        /** Sequencer options. */
        char seq_prev_type;
        /** UNUSED. */
        char seq_rend_type;
        /** Flag use for sequence render/draw. */
        char seq_flag;
        char[] _pad5 = new char[3];

        /* Render simplify. */
        short simplify_subsurf;
        short simplify_subsurf_render;
        short simplify_gpencil;
        float simplify_particles;
        float simplify_particles_render;
        float simplify_volumes;

        /** Freestyle line thickness options. */
        int line_thickness_mode;
        /** In pixels. */
        float unit_line_thickness;

        /** Render engine. */
        string engine; // [32];
        char[] _pad2 = new char[2];

        /** Performance Options. */
        short perf_flag;

        /** Cycles baking. */
        byte[] bake; //BakeData

        int _pad8;
        short preview_pixel_size;

        short _pad4;

        /* MultiView. */
        /** SceneRenderView. */
        ListBase views;
        short actview;
        short views_format;

        /* Hair Display. */
        short hair_type, hair_subdiv;

        /** Motion blur */
        float motion_blur_shutter;
        int motion_blur_position;
        byte[] mblur_shutter_curve; //CurveMapping

        /** Device to use for compositor engine. */
        int compositor_device; /* eCompositorDevice */

        /** Precision used by the GPU execution of the compositor tree. */
        int compositor_precision; /* eCompositorPrecision */

        /** Global configuration for denoise compositor nodes. */
        int compositor_denoise_preview_quality; /* eCompositorDenoiseQaulity */
        int compositor_denoise_final_quality;   /* eCompositorDenoiseQaulity */


        public RenderData()
        {

        }

        public RenderData(IEnumerable<byte> _bytes)
        {
            var bytes = _bytes.ToArray();
            var index = 0;

            

            im_format = bytes.Take(336).ToArray();
            index += 336;

            //_pad void
            index += IntPtr.Size;

            ffcodecdata = bytes.Skip(index).Take(80).ToArray();
            index += 80;
            
            cfra = BitConverter.ToInt32(bytes.Skip(index).ToArray());
            index += 4;
            sfra = (int)BitConverter.ToInt32(bytes.Skip(index).ToArray());
            index += 4;
            efra = (int)BitConverter.ToInt32(bytes.Skip(index).ToArray());
            index += 4;

            subframe = BitConverter.ToSingle(bytes.Skip(index).ToArray());
            index += 4;

            psfra = BitConverter.ToInt32(bytes.Skip(index).ToArray());
            index += 4;
            pefra = BitConverter.ToInt32(bytes.Skip(index).ToArray());
            index += 4;
            images = BitConverter.ToInt32(bytes.Skip(index).ToArray());
            index += 4;
            framapto = BitConverter.ToInt32(bytes.Skip(index).ToArray());
            index += 4;
            
            flag = BitConverter.ToInt16(bytes.Skip(index).ToArray());
            index += 2;
            threads = BitConverter.ToInt16(bytes.Skip(index).ToArray());
            index += 2;

            framelen = BitConverter.ToSingle(bytes.Skip(index).ToArray());
            index += 4;

            frame_step = BitConverter.ToInt32(bytes.Skip(index).ToArray());
            index += 4;

            dimensionspreset = BitConverter.ToInt16(bytes.Skip(index).ToArray());
            index += 2;
            size = BitConverter.ToInt16(bytes.Skip(index).ToArray());
            index += 2;

            xsch = BitConverter.ToInt32(bytes.Skip(index).ToArray());
            index += 4;
            ysch = BitConverter.ToInt32(bytes.Skip(index).ToArray());
            index += 4;

            tilex = BitConverter.ToInt32(bytes.Skip(index).ToArray());
            index += 4;
            tiley = BitConverter.ToInt32(bytes.Skip(index).ToArray());
            index += 4;

            planes = BitConverter.ToInt16(bytes.Skip(index).ToArray());
            index += 2;
            imtype = BitConverter.ToInt16(bytes.Skip(index).ToArray());
            index += 2;
            subimtype = BitConverter.ToInt16(bytes.Skip(index).ToArray());
            index += 2;
            quality = BitConverter.ToInt16(bytes.Skip(index).ToArray());
            index += 2;
            
            //use_lock_interface
            index += 1;
            //_pad7
            index += 3;

            scemode = BitConverter.ToInt32(bytes.Skip(index).ToArray());
            index += 4;
            mode = BitConverter.ToInt32(bytes.Skip(index).ToArray());
            index += 4;

            frs_sec = BitConverter.ToInt16(bytes.Skip(index).ToArray());
            index += 2;

            //alphamode
            index+= 1;

            //_pad0
            index += 1;
            //border
            index += 16;
            //layers
            index += 16;
            //actlay
            index += 16;
            //_pad1
            index += 2;
            xasp = BitConverter.ToSingle(bytes.Skip(index).ToArray());
            index += 4;
            yasp = BitConverter.ToSingle(bytes.Skip(index).ToArray());
            index += 4;

            frs_sec_base = BitConverter.ToSingle(bytes.Skip(index).ToArray());
            index += 4;
            gauss = BitConverter.ToSingle(bytes.Skip(index).ToArray());
            index += 4;



        }

        public void Split(out int Width, out int Height, out int Start, out int End, out float SubFrame)
        {
            Width = xsch;
            Height = ysch;
            Start = sfra;
            End = efra;
            SubFrame = subframe;
        }

    }
}
