using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.Managed
{
    public class Object
    {
        public int uuid;
        
        private string Type;
        public string Name;
        public Native.ObjectType ObjectType; 
        public object Data { private set; get; }
        public static Managed.Object FromDummyObject(Patcher.DNADummyObject dobj)
        {
            var obj = new Managed.Object();
            /*var fullName = (string)dobj.GetObject("id").GetValue("name");
            var fullNameLength = fullName.Length;
            obj.Type = fullName.Substring(0, 2);
            obj.Name = fullName.Substring(2);
            obj.uuid = (int)dobj.GetObject("id").GetValue("session_uid");*/

            /*if(dobj.GetObject("data")!=null)
            {
                
            }*/
            
            return obj;
        }
    }
}
