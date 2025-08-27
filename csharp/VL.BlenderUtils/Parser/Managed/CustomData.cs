using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VL.BlenderUtils.Parser.Managed.CustomData;

namespace VL.BlenderUtils.Parser.Managed
{
    public class CustomData
    {

        public CustomDataLayer layers;
        public int typemap; //size 53
        /** Number of layers, size of layers array. */
        public int totlayer, maxlayer;
        /** In editmode, total size of all data layers. */
        public int totsize;

    }

    public class CustomDataLayer
    {
        /** Type of data in layer. */
        eCustomDataType type;
        /** In editmode, offset of layer in block. */
        public int offset;
        /** General purpose flag. */
        int flag;
        /** Number of the active layer of this type. */
        int active;
        /** Number of the layer to render. */
        int active_rnd;
        /** Number of the layer to render. */
        int active_clone;
        /** Number of the layer to render. */
        int active_mask;
        /** Shape key-block unique id reference. */
        public int uid;
        /** Layer name. */
        public string Name; //[/*MAX_CUSTOMDATA_LAYER_NAME*/ 68];
        
        /** Layer data. */
        public object data { private set; get; }
    }

    public enum eCustomDataType
    {
        /**
         * Used by GPU attributes in the cases when we don't know which layer
         * we are addressing in advance.
         */
        CD_AUTO_FROM_NAME = -1,
        CD_MVERT = 0,
        CD_MSTICKY = 1,
        CD_MDEFORMVERT = 2, /* Array of #MDeformVert. */
        CD_MEDGE = 3,
        CD_MFACE = 4,
        CD_MTFACE = 5,
        CD_MCOL = 6,
        CD_ORIGINDEX = 7,
        /**
         * Used as temporary storage for some areas that support interpolating custom normals.
         * Using a separate type from generic 3D vectors is a simple way of keeping values normalized.
         */
        CD_NORMAL = 8,
        CD_FACEMAP = 9,
        CD_PROP_FLOAT = 10,
        CD_PROP_INT32 = 11,
        CD_PROP_STRING = 12,
        CD_ORIGSPACE = 13, /* for modifier stack face location mapping */
        CD_ORCO = 14,      /* undeformed vertex coordinates, normalized to 0..1 range */
        CD_MTEXPOLY = 15,
        CD_MLOOPUV = 16,
        CD_PROP_BYTE_COLOR = 17,
        /**
         * Previously used for runtime corner tangent storage in mesh #CustomData. Currently only used
         * as an identifier to choose tangents in a few places.
         */
        CD_TANGENT = 18,
        CD_MDISPS = 19,
        CD_PROP_FLOAT4X4 = 20,
        /* CD_ID_MCOL = 21, */
        CD_PROP_INT16_2D = 22,
        CD_CLOTH_ORCO = 23,
        /* CD_RECAST = 24, */ /* UNUSED */

        CD_MPOLY = 25,
        CD_MLOOP = 26,
        CD_SHAPE_KEYINDEX = 27,
        CD_SHAPEKEY = 28,
        CD_BWEIGHT = 29,
        CD_CREASE = 30,
        CD_ORIGSPACE_MLOOP = 31,
        /* CD_PREVIEW_MLOOPCOL = 32, */ /* UNUSED */
        CD_BM_ELEM_PYPTR = 33,

        CD_PAINT_MASK = 34,
        CD_GRID_PAINT_MASK = 35,
        CD_MVERT_SKIN = 36,
        CD_FREESTYLE_EDGE = 37,
        CD_FREESTYLE_FACE = 38,
        CD_MLOOPTANGENT = 39,
        CD_TESSLOOPNORMAL = 40,
        CD_CUSTOMLOOPNORMAL = 41,
        CD_SCULPT_FACE_SETS = 42,

        /* CD_LOCATION = 43, */ /* UNUSED */
        /* CD_RADIUS = 44, */   /* UNUSED */
        CD_PROP_INT8 = 45,
        /* Two 32-bit signed integers. */
        CD_PROP_INT32_2D = 46,

        CD_PROP_COLOR = 47,
        CD_PROP_FLOAT3 = 48,
        CD_PROP_FLOAT2 = 49,
        CD_PROP_BOOL = 50,

        /* CD_HAIRLENGTH = 51, */ /* UNUSED */

        CD_PROP_QUATERNION = 52,

        CD_NUMTYPES = 53,
    }
    
}
