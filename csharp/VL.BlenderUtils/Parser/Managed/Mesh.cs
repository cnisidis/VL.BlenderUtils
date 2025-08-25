using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VL.BlenderUtils.Parser.Native;

namespace VL.BlenderUtils.Parser.Managed
{
    public class Mesh
    {
        private string Type;
        public string Name;
        public int Id;
        public static Managed.Mesh FromDummyObject(Patcher.DNADummyObject dobj)
        {
            var obj = new Managed.Mesh();
            var fullName = (string)dobj.GetObject("id").GetValue("name");
            var fullNameLength = fullName.Length;
            obj.Type = fullName.Substring(0, 2);
            obj.Name = fullName.Substring(2);
            obj.Id = (int)dobj.GetObject("id").GetValue("session_uid");

            return obj;
        }
    }
}
