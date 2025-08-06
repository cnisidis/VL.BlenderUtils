using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VL.BlenderUtils.Parser.Pythonic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VL.BlenderUtils.Parser.DNA
{

    //https://github.com/blender/blender/blob/main/source/blender/makesdna/DNA_ID.h#L400
    [StructLayout(LayoutKind.Sequential)]
    public class ID
    {
        IntPtr next { get; set; }
        IntPtr prev { get; set; }

        IntPtr newID;

        IntPtr library; //Library

        IntPtr ptrAsset_data; //AssetMetaData
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 258)]
        string name; //max 66 chars long

        short flag;
        int tag;
        int us;
        int icon_id;
        uint recalc;

        uint recalc_up_to_undo_push;
        uint recalc_after_undo_push;

        uint session_uid;

        IntPtr properties; //IDProperty 

        IntPtr system_properties; //IDOverrideLibrary

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] _pad;

        IntPtr override_library;

        IntPtr orig_id; //ID


        IntPtr py_instance;

        IntPtr library_weak_reference; //LibraryWeakReference

        IntPtr runtime;//ID_Runtime

        

        public ID()
        {
        
        }

       
        public static ID Read(byte[] data)
        {

            if (data == null || data.Length == 0)
                throw new ArgumentException("Camera data is empty.");

            int size = 80;

            if (data.Length < size)
                throw new ArgumentException($"Data length {data.Length} is smaller than expected Camera struct size {size}.");

            IntPtr ptr = IntPtr.Zero;

            try
            {
                // Allocate unmanaged memory
                ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(data, 0, ptr, size);

                // Marshal bytes into Camera object
                return Marshal.PtrToStructure<ID>(ptr);
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                    Marshal.FreeHGlobal(ptr);
            }
        }

        public void Split(out string Name,  out int Tag, out uint SessionUID, out IntPtr Library)
        {
            Name = this.name;
            
            Tag = this.tag;
            SessionUID = this.session_uid;
            Library = this.library;
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
}
