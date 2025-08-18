using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VL.BlenderUtils.Parser.Native;
using VL.Core.Import;

namespace VL.BlenderUtils.Parser.Managed
{

    public class Scene
    {

        private Native.Scene _native;
        public BlenderObject<Native.Camera> Camera;
        public string Type;
        public string Name { get; set; }


        public Scene(Native.Scene native, BlendFile blendFile)
        {
            this._native = native;
            /*
            if (this._native.camera != IntPtr.Zero || this._native.camera!=null)
            {
                var _camObj = blendFile.ResolvePtr<Native.Object>(_native.camera);
                this.Camera = blendFile.CreateManagedObject(_camObj);
            }
            */
            //this.Type = this._native.id.name.Substring(0,2);
            //this.Name = this._native.id.name.Substring(2, native.id.name.Length - 2);
            

        }

        

        public void Split(out string Name)
        {
            Name = this._native.id.name;
        }

       
       
    }
}
