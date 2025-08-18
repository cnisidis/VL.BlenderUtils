
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using VL.BlenderUtils.Parser.DNA;


namespace VL.BlenderUtils.Parser.Native
{
    //https://github.com/blender/blender/blob/main/source/blender/makesdna/DNA_scene_types.h#L2004
    public struct Scene
    {
        public ID id;
        public IntPtr adt;
        public IntPtr camera;
        public IntPtr world;
        public IntPtr set;

        public ListBase base_list_deprecated;
        public IntPtr basact_deprecated;
        public IntPtr _pad1;

        public View3DCursor cursor;

        public uint lay_deprecated;
        public int layact_deprecated;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] _pad2;

        public short flag;
        [MarshalAs(UnmanagedType.I1)]
        public byte use_nodes_deprecated;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] _pad3;

        public IntPtr nodetree_deprecated;
        public IntPtr compositing_node_group;
        public IntPtr ed;
        public IntPtr toolsettings;
        public IntPtr _pad4;

        public DisplaySafeAreas safe_areas;

        public RenderData r;
        public AudioData audio;

        public ListBase markers;
        public ListBase transform_spaces;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public TransformOrientationSlot[] orientation_slots;

        public IntPtr sound_scene;
        public IntPtr playback_handle;
        public IntPtr sound_scrub_handle;
        public IntPtr speaker_handles;
        public IntPtr fps_info;
        public IntPtr depsgraph_hash;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] _pad7;

        public int active_keyingset;
        public ListBase keyingsets;

        public UnitSettings unit;
        public IntPtr gpd;
        public IntPtr clip;

        public PhysicsSettings physics_settings;
        public IntPtr _pad8;

        public CustomData_MeshMasks customdata_mask;
        public CustomData_MeshMasks customdata_mask_modal;

        public ColorManagedViewSettings view_settings;
        public ColorManagedDisplaySettings display_settings;
        public ColorManagedColorspaceSettings sequencer_colorspace_settings;

        public IntPtr rigidbody_world;
        public IntPtr preview;

        public ListBase view_layers;
        public IntPtr master_collection;
        public IntPtr layer_properties;

        public int simulation_frame_start;
        public int simulation_frame_end;

        public SceneDisplay display;
        public SceneEEVEE eevee;
        public SceneGpencil grease_pencil_settings;
        public SceneHydra hydra;

        public IntPtr runtime;

        public static Scene ReadFromBytes(byte[] bytes, DNACatalog DNACat)
        {
            var offset = 0;
            //Get the DNA structure which correspond to the native class/struct
            var nativeClassName = typeof(Scene).Name;
            DNAStructure dnaStruct = DNACat.Structures.Find(x => x.TypeName == nativeClassName);
            var dnaStructFields = dnaStruct.Fields;
            
            //Create new istance of the output class/struct
            Scene scene = new Scene();

            //Iterate through all the Fields of the native struct
            foreach (var field in typeof(Native.Scene).GetFields())
            {
                //Lookup the dna structures collection and locate the exact dna field that matches the exact name of the native class fields
                var dnaEquivalentField = dnaStruct.Fields.Find(x=>x.GetShortName() == field.Name);
                //if the dna equivalent is not null then
                //1. Construct the object
                //  a. Get the size of the dna equivalent (Calculated Size)
                //  b. Set the size of the bytes.Take(..)
                //  c. Return the Object 
                //  d. proceed next

                /*
                 * Example --> first field in native class is ID
                 * we lookup in the DNACatalog.Structures to find the ID,
                 * we get the Size of the ID and read the bytes with a Helper Function in order to return an ID,
                 * the offset must be set then to the size of the initial object.
                 */
                if (dnaEquivalentField != null)
                {
                    Console.WriteLine($"{dnaEquivalentField.GetShortName()} " + $"--> {field.Name} of type {field.FieldType} " + 
                        $"with size of {dnaEquivalentField.CalculatedSize}");
                    //field.SetValue(scene, new ID());
                }

                else
                {
                    Console.WriteLine($"!!! ---> '{field.Name}' can not be matched!");
                }
                    
            }
            
            return scene;

        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DisplaySafeAreas
    {
        // C++: float title[2];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] title;

        // C++: float action[2];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] action;

        // C++: float title_center[2];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] title_center;

        // C++: float action_center[2];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] action_center;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FFMpegCodecData
    {
        public int type;
        public int codec;
        public int audio_codec;
        public int video_bitrate;
        public int audio_bitrate;
        public int audio_mixrate;
        public int audio_channels;
        public float audio_volume;
        public int gop_size;
        public int max_b_frames;
        public int flags;
        public int constant_rate_factor;
        public int ffmpeg_preset;
        public int ffmpeg_prores_profile;
        public int rc_min_rate;
        public int rc_max_rate;
        public int rc_buffer_size;
        public int mux_packet_size;
        public int mux_rate;
        public int video_hdr;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BakeData
    {
        public ImageFormatData im_format;
        // C++: char filepath[1024];
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        public string filepath;
        public short width;
        public short height;
        public short margin;
        public short flag;
        public float cage_extrusion;
        public float max_ray_distance;
        public int pass_filter;
        // C++: char normal_swizzle[3];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] normal_swizzle;
        public byte normal_space;
        public byte target;
        public byte save_mode;
        public byte margin_type;
        public byte view_from;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] _pad;
        public IntPtr cage_object;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AudioData
    {
        public int mixrate;
        public float main;
        public float speed_of_sound;
        public float doppler_factor;
        public int distance_model;
        public short flag;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] _pad;
        public float volume;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] _pad2;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct TransformOrientationSlot
    {
        public int type;
        public int index_custom;
        public byte flag;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public byte[] _pad0;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UnitSettings
    {
        public float scale_length;
        public byte system;
        public byte system_rotation;
        public short flag;
        public byte length_unit;
        public byte mass_unit;
        public byte time_unit;
        public byte temperature_unit;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] _pad;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PhysicsSettings
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] gravity;
        public int flag;
        public int quick_cache_step;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] _pad0;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SceneDisplay
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] light_direction;
        public float shadow_shift;
        public float shadow_focus;
        public float matcap_ssao_distance;
        public float matcap_ssao_attenuation;
        public int matcap_ssao_samples;
        public byte viewport_aa;
        public byte render_aa;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] _pad;
        public View3DShading shading;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SceneEEVEE
    {
        public int flag;
        public int gi_diffuse_bounces;
        public int gi_cubemap_resolution;
        public int gi_visibility_resolution;
        public float gi_glossy_clamp;
        public int gi_irradiance_pool_size;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] _pad0;
        public int taa_samples;
        public int taa_render_samples;
        public float volumetric_start;
        public float volumetric_end;
        public int volumetric_tile_size;
        public int volumetric_samples;
        public float volumetric_sample_distribution;
        public float volumetric_light_clamp;
        public int volumetric_shadow_samples;
        public int volumetric_ray_depth;
        public float gtao_distance_deprecated;
        public float gtao_thickness_deprecated;
        public float fast_gi_bias;
        public int fast_gi_resolution;
        public int fast_gi_step_count;
        public int fast_gi_ray_count;
        public float fast_gi_quality;
        public float fast_gi_distance;
        public float fast_gi_thickness_near;
        public float fast_gi_thickness_far;
        public byte fast_gi_method;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] _pad1;
        public float bokeh_overblur;
        public float bokeh_max_size;
        public float bokeh_threshold;
        public float bokeh_neighbor_max;
        public int motion_blur_samples_deprecated;
        public int motion_blur_max;
        public int motion_blur_steps;
        public int motion_blur_position_deprecated;
        public float motion_blur_shutter_deprecated;
        public float motion_blur_depth_scale;
        public int shadow_cube_size_deprecated;
        public int shadow_pool_size;
        public int shadow_ray_count;
        public int shadow_step_count;
        public float shadow_resolution_scale;
        public float clamp_surface_direct;
        public float clamp_surface_indirect;
        public float clamp_volume_direct;
        public float clamp_volume_indirect;
        public int ray_tracing_method;
        public RaytraceEEVEE ray_tracing_options;
        public float overscan;
        public float light_threshold;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SceneGpencil
    {
        public float smaa_threshold;
        public float smaa_threshold_render;
        public int aa_samples;
        public int motion_blur_steps;
    }
    

    [StructLayout(LayoutKind.Sequential)]
    public struct SceneHydra
    {
        public int export_method;
        public int _pad0;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RaytraceEEVEE
    {
        public float screen_trace_quality;
        public float screen_trace_thickness;
        public float trace_max_roughness;
        public int resolution_scale;
        public int flag;
        public int denoise_stages;
    }

}
