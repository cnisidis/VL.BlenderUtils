using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VL.BlenderUtils.Parser.DNA;
using VL.Core.Import;

namespace VL.BlenderUtils.Parser.Managed
{

    public class SceneObejct : IBlenderObject
    {

        private Scene _native;
        public Parser.DNA.Object Camera;
        public string Type;
        public string Name { get; set; }
        

        public SceneObejct(Scene native, BlendFile blendFile)
        {
            _native = native;
            if (this._native.camera != IntPtr.Zero)
            {
                this.Camera = blendFile.ResolvePtr<DNA.Object>(this._native.camera);
            }
            this.Type = this._native.id.name.Substring(0,2);
            this.Name = this._native.id.name.Substring(2, native.id.name.Length - 2);
            

        }

        public string GetType() => this.Type;

        public void Split(out string Name)
        {
            Name = this.Name;
        }

        public ID GetID() => this._native.id;
       
    }
}
