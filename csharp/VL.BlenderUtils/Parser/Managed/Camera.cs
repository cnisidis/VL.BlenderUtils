using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.Managed
{
    public class Camera
    {
        public int uuid;
        public string Name;
        private string Type {  set; get; }

        public float Lens;
        
        public static Managed.Camera FromDummyObject(Patcher.DNADummyObject dobj)
        {
            var obj = new Managed.Camera();
            var fullName = (string)dobj.GetObject("id").GetValue("name");
            var fullNameLength = fullName.Length;
            obj.Type = fullName.Substring(0, 2);
            obj.Name = fullName.Substring(2);
            obj.uuid = (int)dobj.GetObject("id").GetValue("session_uid");

            obj.Lens = (float)dobj.GetValue("lens");

            return obj;
        }
    }
}
