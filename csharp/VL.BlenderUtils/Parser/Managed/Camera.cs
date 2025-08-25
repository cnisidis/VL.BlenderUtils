using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VL.BlenderUtils.Parser.Native;
using Stride.Core.Mathematics;

namespace VL.BlenderUtils.Parser.Managed
{
    public class Camera
    {
        public int Id;
        public string Name;
        private string Type {  set; get; }

        public float Lens;
        public SensorFit SensorFit {set; get; }
        public Vector2 SensorSize {set; get; }
        public Vector2 Clipping { get; set; }
        public Vector2 Shift { get; set; }
        public float OrthoScale { get; set; }
        
        public CameraType CameraType { set; get; }
        
        public static Managed.Camera FromDummyObject(Patcher.DNADummyObject dobj)
        {
            var obj = new Managed.Camera();
            var fullName = (string)dobj.GetObject("id").GetValue("name");
            var fullNameLength = fullName.Length;
            obj.Type = fullName.Substring(0, 2);
            obj.Name = fullName.Substring(2);
            obj.Id = (int)dobj.GetObject("id").GetValue("session_uid");
            obj.CameraType = (CameraType)dobj.GetValue("type");
            obj.Lens = (float)dobj.GetValue("lens");
            obj.Shift = new Vector2((float)dobj.GetValue("shiftx"), (float)dobj.GetValue("shifty"));
            obj.SensorSize = new Vector2((float)dobj.GetValue("sensor_x"), (float)dobj.GetValue("sensor_y"));

            return obj;
        }

        public void Split(out string Name, out int uid, out CameraType CameraType)
        {
            Name = this.Name;
            uid = this.Id;
            CameraType = this.CameraType;
        }

        public void GetSensor(out SensorFit Fit, out Vector2 Size)
        {
            Fit = this.SensorFit;
            Size = this.SensorSize;
        }
    }
}
