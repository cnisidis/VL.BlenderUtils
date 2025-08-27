using Stride.Core.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.Managed
{
    public class Light
    {
        public int Id;
        public string Name;
        private string Type { set; get; }
        public LightType LightType { set; get; }
        public Stride.Core.Mathematics.Color4 Color;
        public float Energy;
        public float EnergyNew;
        public float Radius;
        public static Managed.Light FromDummyObject(Patcher.DNADummyObject dobj)
        {
            
            var obj = new Managed.Light();
            var fullName = (string)dobj.GetObject("id").GetValue("name");
            var fullNameLength = fullName.Length;
            obj.Type = fullName.Substring(0, 2);
            obj.Name = fullName.Substring(2);
            obj.Id = (int)dobj.GetObject("id").GetValue("session_uid");

            obj.LightType = (LightType)dobj.GetValue("type");
            obj.EnergyNew = (float)dobj.GetValue("energy_new");
            obj.Radius = (float)dobj.GetValue("radius");
            obj.Color = new Stride.Core.Mathematics.Color4(
                 (float)dobj.GetValue("r"),
                 (float)dobj.GetValue("g"),
                 (float)dobj.GetValue("b")

                );
            obj.Energy = (float)dobj.GetValue("energy");
            return obj;
        }

        public void Split(out LightType Type, out Color4 Color, out float Energy, out float Radius)
        {
            Type = this.LightType;
            Color = this.Color;
            Energy = this.EnergyNew;
            Radius = this.Radius;
        }
    }


    public enum LightType : short
    {
        LA_LOCAL = 0,
        LA_SUN = 1,
        LA_SPOT = 2,
        // LA_HEMI = 3, /* Deprecated. */
        LA_AREA = 4,
    };
}
