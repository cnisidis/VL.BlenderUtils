using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VL.BlenderUtils.Parser.DNA;

namespace VL.BlenderUtils.Parser.Managed
{
    public class SceneObejct:IBlenderObject
    {

        private Scene _native;
        public CameraObject Camera;


        public SceneObejct(Scene native)
        {
            _native = native;


        }

        public ID GetID()
        {
            return _native.id;
        }
    }
}
