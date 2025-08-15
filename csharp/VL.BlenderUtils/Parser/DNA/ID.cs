
using System.Runtime.InteropServices;


namespace VL.BlenderUtils.Parser.DNA
{

    //https://github.com/blender/blender/blob/main/source/blender/makesdna/DNA_ID.h#L400
    
    [StructLayout(LayoutKind.Sequential)]
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
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 258)]
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
    }

    // C++: typedef struct ID_Runtime
    [StructLayout(LayoutKind.Sequential)]
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
    }

    // C++: typedef struct ID_Runtime_Remap
    [StructLayout(LayoutKind.Sequential)]
    public struct ID_Runtime_Remap
    {
        
        public int status;
        public int skipped_refcounted;
        public int skipped_direct;
        public int skipped_indirect;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class Library
    {
        public ID id { get; set; }
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        string filepath; // = new char[1024];

        public Library()
        {
                
        }

        public static Library Read(BinaryReader handle)
        {
            Library lib = new Library();
            lib.filepath = Reader.ReadString(handle, 1024).Replace('\x00', ' ').Trim(); ;
            return lib;
        }

        public string GetFilePath()
        {
            return this.filepath;
        }
    }

}
