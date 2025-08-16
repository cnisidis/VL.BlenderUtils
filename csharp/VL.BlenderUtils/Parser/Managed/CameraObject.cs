
using Stride.Core.Mathematics;
using VL.BlenderUtils.Parser.DNA;

namespace VL.BlenderUtils.Parser.Managed
{
    

    public class CameraObject:IBlenderObject
    {

        private Camera _native;
        public string Type;
        public string Name;

        public CameraType CamType { get; set; }
        public Matrix Transformations { get; set; }
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }
        public float FoV { get; set; }
        public Vector2 Clipping { get; set; }
        public Vector2 Sensor { get; set; }
        public Vector2 Shift { get; set; }

        public float DoFDistance { get; set; }

        public SensorFit SensorFit { get; set; }

        public CameraObject(Camera native, BlendFile blendFile)
        {
            _native = native;
            
            this.Type = native.id.name.Substring(0, 2);
            this.Name = native.id.name.Substring(2, native.id.name.Length - 2);
            this.Sensor = new Vector2(native.sensor_x, native.sensor_y);
            this.Shift = new Vector2(native.shiftx, native.shifty);
            this.CamType = this._native.type;
        }



        public void Split(out string Name, out float FieldOfView, out CameraType Type)
        {
            Name = this.Name;
            FieldOfView = this.FoV;
            Type = this.CamType;
        }

        public ID GetID()
        {
            return this._native.id;
        }

        public string GetType() => this.Type;
    }


    public class Sensor
    {
        Vector2 Size { get; set; }
        SensorFit Fit { get; set; }

        public Sensor()
        {

        }
    }


    public enum SensorFit
    {

    }

   

}
