using Stride.Core.Mathematics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VL.BlenderUtils.Parser.Native;

namespace VL.BlenderUtils.Parser.Managed
{
    public class Object
    {
        public int Id;
        /// <summary>
        /// Internal Blender Type CA, OB, SC etc
        /// </summary>
        private string Type;
        /// <summary>
        /// Object Name
        /// </summary>
        public string Name;
        /// <summary>
        /// Object Type : Mesh, Camera, Lamp etc
        /// </summary>
        public Native.ObjectType ObjectType; 
        public object Data { private set; get; }
        /// <summary>
        /// Object's Location
        /// </summary>
        Stride.Core.Mathematics.Vector3 Location;
        /// <summary>
        /// Object's Rotation
        /// Roations originally are in Radians
        /// </summary>
        Stride.Core.Mathematics.Vector3 Rotation;
        /// <summary>
        /// Objsect's Size
        /// </summary>
        Stride.Core.Mathematics.Vector3 Size;
        //Deltas
        /// <summary>
        /// Location Delta
        /// </summary>
        Stride.Core.Mathematics.Vector3 LocationDelta;
        /// <summary>
        /// Rotation Delta
        /// </summary>
        Stride.Core.Mathematics.Vector3 RotationDelta;
        /// <summary>
        /// Scale Delta
        /// </summary>
        Stride.Core.Mathematics.Vector3 ScaleDelta;

        Stride.Core.Mathematics.Quaternion Quaternion;

        bool isEmpty;

        public static Managed.Object FromDummyObject(Patcher.DNADummyObject dobj)
        {
            var obj = new Managed.Object();

            var fullName = (string)dobj.GetObject("id").GetValue("name");
            obj.Type = fullName.Substring(0, 2);
            obj.Name = fullName.Substring(2);
            obj.Id = (int)dobj.GetObject("id").GetValue("session_uid");

            var t = (ObjectType)dobj.GetValue("type");
            var d = dobj.GetObject("data");
            object data = null;
            switch(t)
            {
                case ObjectType.OB_CAMERA:
                    data = Managed.Camera.FromDummyObject(d);
                    break;
            }
            
            obj.ObjectType = t;
            obj.Data = (object)data;
            
            var loc = (float[])dobj.GetValue("loc");
            obj.Location = new Stride.Core.Mathematics.Vector3((float)loc[0], (float)loc[1], (float)loc[2]);
            var rot = (float[])dobj.GetValue("rot");
            obj.Rotation = new Stride.Core.Mathematics.Vector3((float)rot[0], (float)rot[1], (float)rot[2]);
            var sca = (float[])dobj.GetValue("size");
            obj.Size = new Stride.Core.Mathematics.Vector3((float)sca[0], (float)sca[1], (float)sca[2]);

            return obj;
        }
        /// <summary>
        /// Blender Location, Rotation and Size
        /// </summary>
        /// <param name="Translation"></param>
        /// <param name="Rotation"></param>
        /// <param name="Scale"></param>
        public void GetTransformations(out Stride.Core.Mathematics.Vector3 Translation, out Stride.Core.Mathematics.Vector3 Rotation, out Stride.Core.Mathematics.Vector3 Scale)
        {
            Translation = Location;
            Rotation = this.Rotation;
            Scale = this.Size; 
        }
        /// <summary>
        /// Stride Transformations
        /// </summary>
        /// <returns></returns>
        public Matrix GetTransformations()
        {
            var matrix = new Stride.Core.Mathematics.Matrix();
            Matrix.Scaling(Size.X, Size.Z, Size.Y,out matrix);
            Matrix.RotationYawPitchRoll(Rotation.Z, Rotation.X, Rotation.Y, out matrix);
            Matrix.Translation(Location.X, Location.Z, Location.Y,out  matrix);

            return matrix;
        }
        /// <summary>
        /// Split Object
        /// </summary>
        /// <param name="Name">The name of the Object</param>
        /// <param name="uid">The Sessions Unique ID</param>
        /// <param name="ObjectType">Type of the Object</param>
        public void Split(out string Name, out int uid, out ObjectType ObjectType)
        {
            Name = this.Name;
            uid = this.Id;
            ObjectType = this.ObjectType;
        }
    }
}
