using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.DNA
{

    //https://github.com/blender/blender/blob/main/source/blender/makesdna/DNA_ID.h#L400
    public class ID
    {
        object next { get; set; }
        object prev { get; set; }

        IntPtr ptrNewId; //ID

        IntPtr ptrLib; //Library

        IntPtr ptrAsset_data; //AssetMetaData

        string name; //max 66 chars long

        short flag;
        int tag;
        int us;
        int icon_id;
        int recalc;

        int recalc_up_to_undo_push;
        int recalc_after_undo_push;

        uint session_uid;

        IntPtr ptrProperties; //IDProperty 

        IntPtr ptrOverride_library; //IDOverrideLibrary

        IntPtr ptrOrigin; //ID


        object py_instance;

        IntPtr library_weak_reference; //LibraryWeakReference

        byte[] runtime;//ID_Runtime

        public int Size;

        public ID()
        {
            Size = 208;
        }

        public ID(IEnumerable<byte> _bytes)
        {
            var bytes = _bytes.ToArray();
            var index = 0;

            index += IntPtr.Size;
            index += IntPtr.Size;
            
            ptrNewId = (IntPtr)BitConverter.ToUInt32(bytes);
            index += IntPtr.Size;
            ptrLib = (IntPtr)BitConverter.ToUInt32(bytes.Skip(index).ToArray());
            index += IntPtr.Size;
            ptrAsset_data = (IntPtr)BitConverter.ToUInt32(bytes.Skip(index).ToArray());
            index += IntPtr.Size;
            
            name = Encoding.UTF8.GetString( bytes.Skip(index).Take(66).ToArray() ).Replace('\x00', ' ').Trim();
            index += 66;
            
            flag = (short)BitConverter.ToInt16(bytes.Skip(index).ToArray());
            index += 2;
            
            tag = (int)BitConverter.ToUInt32(bytes.Skip(index).ToArray());
            index += 4;
            us = (int)BitConverter.ToUInt32(bytes.Skip(index).ToArray());
            index += 4;
            icon_id = (int)BitConverter.ToUInt32(bytes.Skip(index).ToArray());
            index += 4;
            recalc = (int)BitConverter.ToUInt32(bytes.Skip(index).ToArray());
            index += 4;
            recalc_up_to_undo_push = (int)BitConverter.ToUInt32(bytes.Skip(index).ToArray());
            index += 4;
            recalc_after_undo_push = (int)BitConverter.ToUInt32(bytes.Skip(index).ToArray());
            index += 4;
            
            session_uid = BitConverter.ToUInt32(bytes.Skip(index).ToArray());
            index += 4;
            
            ptrProperties = (IntPtr)BitConverter.ToUInt64(bytes.Skip(index).ToArray());
            index += IntPtr.Size;
            ptrOverride_library = (IntPtr)BitConverter.ToUInt64(bytes.Skip(index).ToArray());
            index += IntPtr.Size;
            ptrOrigin = (IntPtr)BitConverter.ToUInt32(bytes.Skip(index).ToArray());
            index += IntPtr.Size;

            //py_istance
            index += IntPtr.Size;

            library_weak_reference = (IntPtr)BitConverter.ToUInt64(bytes.Skip(index).ToArray());
            index += IntPtr.Size;
            runtime = bytes.Skip(index).ToArray();
            index += 32;

            
            Size = index;

        }

        public void Split(out string Name, out int Size, out int Tag, out uint SessionUID)
        {
            Name = this.name;
            Size = this.Size;
            Tag = this.tag;
            SessionUID = this.session_uid;
        }


        internal struct Library
        {
            ID id;

            char[] filepath = new char[1024];

            public Library(byte[] bytes)
            {
                
            }
        }

    }
}
