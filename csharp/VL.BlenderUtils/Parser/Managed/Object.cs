using Stride.Core.Mathematics;
using System;

using VL.BlenderUtils.Parser.Native;
using VL.Lib.IO;
using VL.Lib.Mathematics;

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
        Stride.Core.Mathematics.Vector3 Location = new();
        /// <summary>
        /// Object's Rotation
        /// Roations originally are in Radians
        /// </summary>
        Stride.Core.Mathematics.Vector3 Rotation = new();
        /// <summary>
        /// Objsect's Size
        /// </summary>
        Stride.Core.Mathematics.Vector3 Size = new();
        //Deltas
        /// <summary>
        /// Location Delta
        /// </summary>
        Stride.Core.Mathematics.Vector3 LocationDelta = new();
        /// <summary>
        /// Rotation Delta
        /// </summary>
        Stride.Core.Mathematics.Vector3 RotationDelta;
        /// <summary>
        /// Scale Delta
        /// </summary>
        Stride.Core.Mathematics.Vector3 ScaleDelta;

        Stride.Core.Mathematics.Quaternion Quaternion = new();

        Matrix WorldMatrix = new Matrix();
        Matrix LocalMatrix = new Matrix();

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
            var quat = (float[])dobj.GetValue("quat");
            Console.WriteLine($"Quat: {quat[0]} {quat[1]} {quat[2]} {quat[3]}");
            obj.Quaternion = new Stride.Core.Mathematics.Quaternion((float)quat[0], (float)quat[1], (float)quat[2], (float)quat[3]);

            foreach(var q in quat)
            {
                Console.WriteLine(q.ToString());
            }

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
            
            
            var sTranslation = new Stride.Core.Mathematics.Vector3(Location.X, -Location.Z, Location.Y);
            var sScale = new Stride.Core.Mathematics.Vector3(Size.X, Size.Z, Size.Y);
            var initMatrix = Matrix.Identity;

            //Matrix.RotationYawPitchRoll(Rotation.Z, Rotation.X, Rotation.Y, out matrix);
            
            var rotMatrix = Matrix.RotationYawPitchRoll(Rotation.Z+ (float)(-Math.PI / 2), Rotation.X, Rotation.Y);
            var sclMatrix = Matrix.Scaling(sScale);
            var trsMatrix = Matrix.Translation(sTranslation);
            
            
            initMatrix = Matrix.Multiply(initMatrix, sclMatrix);
            initMatrix = Matrix.Multiply(initMatrix, rotMatrix); 
            initMatrix = Matrix.Multiply(initMatrix, trsMatrix);

            var rotMatrixToBlender = Matrix.RotationYawPitchRoll((float)(-Math.PI / 2), 0, 0);
            initMatrix = Matrix.Multiply(initMatrix, rotMatrixToBlender);

            return initMatrix;



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
