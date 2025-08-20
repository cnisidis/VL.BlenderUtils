
using System.Runtime.InteropServices;
using System;
using System.Text;
using VL.BlenderUtils.Parser.DNA;
using System.Reflection;

namespace VL.BlenderUtils.Parser.Native
{

    //https://github.com/blender/blender/blob/main/source/blender/makesdna/DNA_ID.h#L400
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ID
    {
        
        public IntPtr next;
        public IntPtr prev;
        public IntPtr newid;
        public IntPtr lib;
        // C++: struct AssetMetaData *asset_data;
        public IntPtr asset_data;
        // C++: char name[258];
        // Fixed-size C-style string.
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 66)]
        public string name;
        public short flag;
        public int tag;
        public int us;
        public int icon_id;
        public uint recalc;
        public uint recalc_up_to_undo_push;
        public uint recalc_after_undo_push;
        public uint session_uid;
        public IntPtr properties;
        public IntPtr system_properties;
        public IntPtr _pad1;
        public IntPtr override_library;       
        public IntPtr orig_id;
        public IntPtr py_instance;
        public IntPtr library_weak_reference;

        // C++: struct ID_Runtime runtime;
        // Note: You would need to define this struct as well.
        public ID_Runtime runtime;

        public string GetName()
        {
            return this.name;
        }

    }

    // C++: typedef struct ID_Runtime
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ID_Runtime
    {
        // C++: ID_Runtime_Remap remap;
        // Nested struct definition.
        public ID_Runtime_Remap remap;
        // C++: struct Depsgraph *depsgraph;
        // Pointer to another struct, represented as an IntPtr.
        public IntPtr depsgraph;
        // C++: struct ID_Readfile_Data *readfile_data;
        // Pointer to another struct, represented as an IntPtr.
        public IntPtr readfile_data;

        public ID_Runtime()
        {

        }
    }

    // C++: typedef struct ID_Runtime_Remap
    [StructLayout(LayoutKind.Sequential, Pack =1)]
    public struct ID_Runtime_Remap
    {
        
        public int status;
        public int skipped_refcounted;
        public int skipped_direct;
        public int skipped_indirect;

        public ID_Runtime_Remap()
        {

        }
    }

    

    [System.Flags]
    public enum IDFlags : ushort
    {
        
        /// <summary>
        /// Don't delete the data-block even if unused.
        /// </summary>
        ID_FLAG_FAKEUSER = 1 << 9,

        /// <summary>
        /// The data-block is a sub-data of another one.
        /// Direct persistent references are not allowed.
        /// </summary>
        ID_FLAG_EMBEDDED_DATA = 1 << 10,

        /// <summary>
        /// Data-block is from a library and linked indirectly, with ID_TAG_INDIRECT
        /// tag set. But the current .blend file also has a weak pointer to it that
        /// we want to restore if possible, and silently drop if it's missing.
        /// </summary>
        ID_FLAG_INDIRECT_WEAK_LINK = 1 << 11,

        /// <summary>
        /// The data-block is a sub-data of another one, which is an override.
        /// Note that this also applies to shape-keys, even though they are not 100% embedded data.
        /// </summary>
        ID_FLAG_EMBEDDED_DATA_LIB_OVERRIDE = 1 << 12,

        /// <summary>
        /// The override data-block appears to not be needed anymore after resync with linked data, but it
        /// was kept around (because e.g. detected as user-edited).
        /// </summary>
        ID_FLAG_LIB_OVERRIDE_RESYNC_LEFTOVER = 1 << 13,

        /// <summary>
        /// This id was explicitly copied as part of a clipboard copy operation.
        /// When reading the clipboard back, this can be used to check which ID's are
        /// intended to be part of the clipboard, compared with ID's that were indirectly referenced.
        ///
        /// While the flag is typically cleared, a saved file may have this set for some data-blocks,
        /// so it must be treated as dirty.
        /// </summary>
        ID_FLAG_CLIPBOARD_MARK = 1 << 14,

        ID_FLAG_RESERVED = 0xFFFF
    }

}
