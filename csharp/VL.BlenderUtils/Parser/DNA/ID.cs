using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.DNA
{

    //https://github.com/dfelinto/blender/blob/master/source/blender/makesdna/DNA_ID.h#L360
    public class ID
    {
        nint next { get; set; }
        nint prev { get; set; }

        ID newId;

        Library Lib;

        //missing AssetMetaData

        public string Name; //max 66 chars long

        short flag;
        int tag;
        int us;
        int icon_id;
        int recalc;

        int recalc_up_to_undo_push;
        int recalc_after_undo_push;

        uint session_uid;

        //IDProperty properties

        //IDOverrideLibrary override_library

        ID origin;


        nint py_instance;


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
