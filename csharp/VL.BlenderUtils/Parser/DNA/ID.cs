using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VL.BlenderUtils.Parser.Pythonic;

namespace VL.BlenderUtils.Parser.DNA
{

    //https://github.com/blender/blender/blob/main/source/blender/makesdna/DNA_ID.h#L400
    public class ID
    {
        ulong next { get; set; }
        ulong prev { get; set; }

        ID newID;

        Library library; //Library

        ulong ptrAsset_data; //AssetMetaData

        string name; //max 66 chars long

        short flag;
        int tag;
        int us;
        int icon_id;
        int recalc;

        int recalc_up_to_undo_push;
        int recalc_after_undo_push;

        uint session_uid;

        ulong ptrProperties; //IDProperty 

        ulong ptrOverride_library; //IDOverrideLibrary

        ulong ptrOrigin; //ID


        object py_instance;

        ulong library_weak_reference; //LibraryWeakReference

        byte[] runtime;//ID_Runtime

        public int Size;

        public ID()
        {
            Size = 208;
        }

       
        public static ID Read(BinaryReader handle, Pythonic.BlendFile.Header header)
        {
            ID id = new ID();
            id.next = Reader.Read(ReaderType.P, handle, header);
            id.prev = Reader.Read(ReaderType.P, handle, header);
            var ptrNewId = Reader.Read(ReaderType.P, handle, header);
            Console.WriteLine(ptrNewId);
            id.newID = new ID();
            var ptrLib = Reader.Read(ReaderType.P, handle, header);
            Console.WriteLine(ptrLib);
            id.library = Reader.ReadBlock(handle, "Library", header, ptrLib);
            var ptrAssetData = Reader.Read(ReaderType.P, handle, header);
            //id.ptrAsset_data = Reader.Read(ReaderType.P, handle, header);
            id.name = Reader.ReadString(handle, 66).Replace('\x00', ' ').Trim();
            id.flag = Reader.Read(ReaderType.S, handle, header);
            id.tag = Reader.Read(ReaderType.I, handle, header);
            id.us = Reader.Read(ReaderType.I, handle, header);
            id.icon_id = Reader.Read(ReaderType.I, handle, header);
            id.recalc = Reader.Read(ReaderType.I, handle, header);
            id.recalc_up_to_undo_push = Reader.Read(ReaderType.I, handle, header);
            id.recalc_after_undo_push = Reader.Read(ReaderType.I, handle, header);
            id.session_uid = Reader.Read(ReaderType.UI, handle, header);
            id.ptrProperties = Reader.Read(ReaderType.P, handle, header);
            id.ptrOverride_library = Reader.Read(ReaderType.P, handle, header);
            id.ptrOrigin = Reader.Read(ReaderType.P, handle, header);
            id.py_instance = (object)Reader.Read(ReaderType.P, handle, header);
            id.library_weak_reference = Reader.Read(ReaderType.P, handle, header);
            id.runtime = Reader.ReadBytes(handle, 32);

            return id;
        }

        public void Split(out string Name, out int Size, out int Tag, out uint SessionUID, out Library Library)
        {
            Name = this.name;
            Size = this.Size;
            Tag = this.tag;
            SessionUID = this.session_uid;
            Library = this.library;
        }


        public class Library
        {
            public ID id { get; set; }

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
