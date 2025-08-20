using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.Native
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
        public short type;
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

        [DNA_DEPRECATED]
        public IntPtr proxy;
        [DNA_DEPRECATED]
        public IntPtr proxy_group;
        [DNA_DEPRECATED]
        public IntPtr proxy_from;
        [DNA_DEPRECATED]
        public IntPtr ipo;
        [DNA_DEPRECATED]
        public IntPtr action;
        [DNA_DEPRECATED]
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

        [DNA_DEPRECATED]
        public ListBase constraintChannels;
        [DNA_DEPRECATED]
        public ListBase effect;
        [DNA_DEPRECATED]
        public ListBase defbase;
        [DNA_DEPRECATED]
        public ListBase fmaps;
        
        public ListBase modifiers;
        public ListBase greasepencil_modifiers;
        public ListBase shader_fx;

        public int mode;
        public int restore_mode;
        // C: struct Material **mat; (A pointer to an array of pointers)
        public IntPtr mat;
        // C: char *matbits; (A pointer to a byte array)
        public IntPtr matbits;
        // C: int totcol;
        public int totcol;
        // C: int actcol;
        public int actcol;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] loc;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] dloc;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] scale;
        [DNA_DEPRECATED]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] dsize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] dscale;
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
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4*4)]
        public float[] parentinv;
        // C: float constinv[4][4]; (2D array, must be flattened to a 1D array)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4*4)]
        public float[] constinv;

        [DNA_DEPRECATED]
        public uint lay;
        /** Copy of Base. */
        public short flag;
        [DNA_DEPRECATED]
        public short colbits;

        /** Transformation settings and transform locks. */
        public short transflag;
        public short protectflag;
        public short trackflag;
        public short upflag;
        /** Used for DopeSheet filtering settings (expanded/collapsed). */
        public short nlaflag;

        
        public byte _pad1;
        // C: char duplicator_visibility_flag;
        public byte duplicator_visibility_flag;

        /* Depsgraph */
        /** Used by depsgraph, flushed from base. */
        public short base_flag;
        /** Used by viewport, synced from base. */
        public ushort base_local_view_bits;

        /** Collision mask settings */
        public ushort col_group;
        public ushort col_mask;

        /** Rotation mode - uses defines set out in DNA_action_types.h for PoseChannel rotations.... */
        public short rotmode;

        /** Bounding box use for drawing. */
        
        public char boundtype;
        /** Bounding box type used for collision. */
        [MarshalAs(UnmanagedType.I1)]
        public char collision_boundtype;

        /** Viewport draw extra settings. */
        public short dtx;
        /** Viewport draw type. */
        public byte dt;
        [MarshalAs(UnmanagedType.I1)]
        public byte empty_drawtype;
        
        public float empty_drawsize;
        /** Dupliface scale. */
        public float instance_faces_scale;

        // C: short index;
        public short index;
        [DNA_DEPRECATED]
        public ushort actdef;
        /** Current face map, NOTE: index starts at 1. */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] _pad2;
        /** Object color (in most cases the material color is used for drawing). */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] color;

        /** Softbody settings. */
        public short softflag;

        /** For restricting view, select, render etc. accessible in outliner. */
        public short visibility_flag;

        /** Current shape key for menu or pinned. */
        public short shapenr;
        /** Flag for pinning. */
        public byte shapeflag;

        
        public byte _pad3;

        /** Object constraints. */
        public ListBase constraints;
        [DNA_DEPRECATED ]
        public ListBase nlastrips;
        [DNA_DEPRECATED]
        public ListBase hooks;
        /** Particle systems. */
        public ListBase particlesystem;

        /** Particle deflector/attractor/collision data. */
        public IntPtr pd;
        /** If exists, saved in file. */
        public IntPtr soft;
        /** Object duplicator for group. */
        public IntPtr instance_collection;

        /** If fluidsim enabled, store additional settings. */
        [DNA_DEPRECATED]
        public IntPtr fluidsimSettings;

        // C: ListBase pc_ids;
        public ListBase pc_ids;

        /** Settings for Bullet rigid body. */
        public IntPtr rigidbody_object;
        /** Settings for Bullet constraint. */
        public IntPtr rigidbody_constraint;

        /** Offset for image empties. */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] ima_ofs;

        /** Must be non-null when object is an empty image. */
        public IntPtr iuser;
        public byte empty_image_visibility_flag;
        public byte empty_image_depth;
        public byte empty_image_flag;

        /** ObjectModifierFlag */
        public byte modifier_flag;

        
        public float shadow_terminator_normal_offset;
        public float shadow_terminator_geometry_offset;
        public float shadow_terminator_shading_offset;

        public IntPtr preview;
        public ObjectLineArt lineart;
        /** Light-group membership information. */
        public IntPtr lightgroup;
        /** Light linking information. */
        public IntPtr light_linking;

        /** Irradiance caches baked for this object (light-probes only). */
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
