using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VL.BlenderUtils.Parser.DNA;
using VL.BlenderUtils.Parser.Native;
using VL.Core.Import;

namespace VL.BlenderUtils.Parser.Managed
{

    public class Scene
    {

        private Native.Scene _native;
        public readonly BlenderObject<Native.Camera> Camera;
        public string Type;
        public string Name { get; set; }


        public Scene(Native.Scene native, BlendFile blendFile)
        {
            
            
                this._native = native;
                this.Type = this._native.id.name.Substring(0, 2);
                this.Name = !string.IsNullOrEmpty(this.Name) ?  "" :  this._native.id.name.Substring(2, native.id.name.Length - 2) ;
            
                if (this._native.camera != IntPtr.Zero)
                {
                    var _camera = blendFile.ResolvePtr<Native.Object>(this._native.camera);
                    this.Camera = blendFile.CreateManagedObject(_camera);
                }
            
            

        }

        

        public void Split(out string Name, out string Type)
        {
            Name = this.Name;
            Type = this.Type;
        }

       
       
    }
}
