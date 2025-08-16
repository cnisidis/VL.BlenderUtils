using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.DNA
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Object
    {
        // C: ID id;
        public ID id;

        // C: struct AnimData *adt;
        public IntPtr adt;
        // C: struct SculptSession *sculpt;
        public IntPtr sculpt;

        // C: short type;
        public ObjectType type;
        // C: short partype;
        public short partype;
        // C: int par1, par2, par3;
        public int par1;
        public int par2;
        public int par3;
        // C: char parsubstr[64];
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string parsubstr;
        // C: struct Object *parent, *track;
        public IntPtr parent;
        public IntPtr track;
        // C: struct Object *proxy DNA_DEPRECATED;
        public IntPtr proxy;
        // C: struct Object *proxy_group DNA_DEPRECATED;
        public IntPtr proxy_group;
        // C: struct Object *proxy_from DNA_DEPRECATED;
        public IntPtr proxy_from;
        // C: struct Ipo *ipo DNA_DEPRECATED;
        public IntPtr ipo;
        // C: struct bAction *action DNA_DEPRECATED;
        public IntPtr action;
        // C: struct bAction *poselib DNA_DEPRECATED;
        public IntPtr poselib;
        // C: struct bPose *pose;
        public IntPtr pose;
        // C: void *data; (This is a crucial pointer to the Object's data, like Mesh, Camera, Curve, etc.)
        public IntPtr data;
        // C: struct bGPdata *gpd DNA_DEPRECATED;
        public IntPtr gpd;

        // C: bAnimVizSettings avs; (You will need to define this struct)
        public bAnimVizSettings avs;
        // C: bMotionPath *mpath;
        public IntPtr mpath;
        // C: void *_pad0;
        public byte _pad0;

        // C: ListBase constraintChannels DNA_DEPRECATED;
        public ListBase constraintChannels;
        // C: ListBase effect DNA_DEPRECATED;
        public ListBase effect;
        // C: ListBase defbase DNA_DEPRECATED;
        public ListBase defbase;
        // C: ListBase fmaps DNA_DEPRECATED;
        public ListBase fmaps;
        // C: ListBase modifiers;
        public ListBase modifiers;
        // C: ListBase greasepencil_modifiers;
        public ListBase greasepencil_modifiers;
        // C: ListBase shader_fx;
        public ListBase shader_fx;

        // C: int mode;
        public int mode;
        // C: int restore_mode;
        public int restore_mode;

        // C: struct Material **mat; (A pointer to an array of pointers)
        public IntPtr mat;
        // C: char *matbits; (A pointer to a byte array)
        public IntPtr matbits;
        // C: int totcol;
        public int totcol;
        // C: int actcol;
        public int actcol;

        // C: float loc[3], dloc[3];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] loc;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] dloc;
        // C: float scale[3];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] scale;
        // C: float dsize[3] DNA_DEPRECATED;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] dsize;
        // C: float dscale[3];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] dscale;
        // C: float rot[3], drot[3];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] rot;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] drot;
        // C: float quat[4], dquat[4];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] quat;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] dquat;
        // C: float rotAxis[3], drotAxis[3];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] rotAxis;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] drotAxis;
        // C: float rotAngle, drotAngle;
        public float rotAngle;
        public float drotAngle;
        // C: float parentinv[4][4]; (2D array, must be flattened to a 1D array)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public float[] parentinv;
        // C: float constinv[4][4]; (2D array, must be flattened to a 1D array)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public float[] constinv;

        // C: unsigned int lay DNA_DEPRECATED;
        public uint lay;
        // C: short flag;
        public short flag;
        // C: short colbits DNA_DEPRECATED;
        public short colbits;

        // C: short transflag, protectflag;
        public short transflag;
        public short protectflag;
        // C: short trackflag, upflag;
        public short trackflag;
        public short upflag;
        // C: short nlaflag;
        public short nlaflag;

        // C: char _pad1;
        public byte _pad1;
        // C: char duplicator_visibility_flag;
        public byte duplicator_visibility_flag;

        // C: short base_flag;
        public short base_flag;
        // C: unsigned short base_local_view_bits;
        public ushort base_local_view_bits;

        // C: unsigned short col_group, col_mask;
        public ushort col_group;
        public ushort col_mask;

        // C: short rotmode;
        public short rotmode;

        // C: char boundtype;
        public byte boundtype;
        // C: char collision_boundtype;
        public byte collision_boundtype;

        // C: short dtx;
        public short dtx;
        // C: char dt;
        public byte dt;
        // C: char empty_drawtype;
        public byte empty_drawtype;
        // C: float empty_drawsize;
        public float empty_drawsize;
        // C: float instance_faces_scale;
        public float instance_faces_scale;

        // C: short index;
        public short index;
        // C: unsigned short actdef DNA_DEPRECATED;
        public ushort actdef;
        // C: char _pad2[4];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] _pad2;
        // C: float color[4];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] color;

        // C: short softflag;
        public short softflag;

        // C: short visibility_flag;
        public short visibility_flag;

        // C: short shapenr;
        public short shapenr;
        // C: char shapeflag;
        public byte shapeflag;

        // C: char _pad3[1];
        public byte _pad3;

        // C: ListBase constraints;
        public ListBase constraints;
        // C: ListBase nlastrips DNA_DEPRECATED;
        public ListBase nlastrips;
        // C: ListBase hooks DNA_DEPRECATED;
        public ListBase hooks;
        // C: ListBase particlesystem;
        public ListBase particlesystem;

        // C: struct PartDeflect *pd;
        public IntPtr pd;
        // C: struct SoftBody *soft;
        public IntPtr soft;
        // C: struct Collection *instance_collection;
        public IntPtr instance_collection;
        // C: struct FluidsimSettings *fluidsimSettings DNA_DEPRECATED;
        public IntPtr fluidsimSettings;

        // C: ListBase pc_ids;
        public ListBase pc_ids;

        // C: struct RigidBodyOb *rigidbody_object;
        public IntPtr rigidbody_object;
        // C: struct RigidBodyCon *rigidbody_constraint;
        public IntPtr rigidbody_constraint;

        // C: float ima_ofs[2];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] ima_ofs;
        // C: ImageUser *iuser;
        public IntPtr iuser;
        // C: char empty_image_visibility_flag;
        public byte empty_image_visibility_flag;
        // C: char empty_image_depth;
        public byte empty_image_depth;
        // C: char empty_image_flag;
        public byte empty_image_flag;

        // C: uint8_t modifier_flag;
        public byte modifier_flag;

        // C: float shadow_terminator_normal_offset;
        public float shadow_terminator_normal_offset;
        // C: float shadow_terminator_geometry_offset;
        public float shadow_terminator_geometry_offset;
        // C: float shadow_terminator_shading_offset;
        public float shadow_terminator_shading_offset;

        // C: struct PreviewImage *preview;
        public IntPtr preview;
        // C: ObjectLineArt lineart; (You will need to define this struct)
        public ObjectLineArt lineart;
        // C: struct LightgroupMembership *lightgroup;
        public IntPtr lightgroup;
        // C: LightLinking *light_linking;
        public IntPtr light_linking;
        // C: struct LightProbeObjectCache *lightprobe_cache;
        public IntPtr lightprobe_cache;

        // C: ObjectRuntimeHandle *runtime;
        public IntPtr runtime;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ObjectLineArt
    {
        // C: short usage;
        public short usage;
        // C: short flags;
        public short flags;

        // C: float crease_threshold;
        public float crease_threshold;

        // C: unsigned char intersection_priority;
        public byte intersection_priority;

        // C: char _pad[7];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public byte[] _pad;
    }

    public enum ObjectType:short
    {
        OB_EMPTY = 0,
        OB_MESH = 1,
        /** Curve object is still used but replaced by "Curves" for the future (see #95355). */
        OB_CURVES_LEGACY = 2,
        OB_SURF = 3,
        OB_FONT = 4,
        OB_MBALL = 5,

        OB_LAMP = 10,
        OB_CAMERA = 11,

        OB_SPEAKER = 12,
        OB_LIGHTPROBE = 13,

        OB_LATTICE = 22,

        OB_ARMATURE = 25,

        OB_GPENCIL_LEGACY = 26,

        OB_CURVES = 27,

        OB_POINTCLOUD = 28,

        OB_VOLUME = 29,

        OB_GREASE_PENCIL = 30,

        /* Keep last. */
        OB_TYPE_MAX,
    }
}
