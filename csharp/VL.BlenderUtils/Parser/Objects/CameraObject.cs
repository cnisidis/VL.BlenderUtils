
using Stride.Core.Mathematics;
using VL.BlenderUtils.Parser.DNA;

namespace VL.BlenderUtils.Parser.Managed
{
    public enum SensorFit
    {

    }


    public class CameraObject
    {

        private Camera _native;
        public Matrix Transformations { get; set; }
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }

        public string Name;

        public float FoV { get; set; }
        public Vector2 Clipping { get; set; }
        public Vector2 Sensor { get; set; }
        public Vector2 Shift { get; set; }

        public float DoFDistance { get; set; }

        public CameraObject(Camera native)
        {
            _native = native;
        }
    }

    
}
