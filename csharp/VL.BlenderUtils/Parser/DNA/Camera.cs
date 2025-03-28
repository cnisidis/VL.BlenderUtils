using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VL.Lib.IO.Notifications;
using Stride.Core.Mathematics;
using VL.Lib.Collections;
namespace VL.BlenderUtils.Parser.DNA
{
    //https://github.com/blender/blender/blob/main/source/blender/makesdna/DNA_camera_types.h
    public class Camera
    {
        ID id;

        ulong adt;

        char type;

        char dtx;
        short flag;
        float passepartalpha;
        float clip_start, clip_end;
        float lens, ortho_scale, drawsize;
        float sensor_x, sensor_y;
        float shiftx, shifty;
        float dof_distance; //DEPRECATED

        char sensor_fit;
        char panorama_type;
        char[] _pad = new char[2];

        /* Fish-eye properties. */
        float fisheye_fov;
        float fisheye_lens;
        float latitude_min, latitude_max;
        float longitude_min, longitude_max;
        float fisheye_polynomial_k0;
        float fisheye_polynomial_k1;
        float fisheye_polynomial_k2;
        float fisheye_polynomial_k3;
        float fisheye_polynomial_k4;

        /* Central cylindrical range properties. */
        float central_cylindrical_range_u_min;
        float central_cylindrical_range_u_max;
        float central_cylindrical_range_v_min;
        float central_cylindrical_range_v_max;
        float central_cylindrical_radius;
        float _pad2;

        /** Old animation system, deprecated for 2.5. */
        ulong ipo;// DNA_DEPRECATED;

        ulong dof_ob;// DNA_DEPRECATED;
        object gpu_dof;//GPUDOFSettings
        object dof;//CameraDOFSettings

        /* CameraBGImage reference images */
        ListBase bg_images;

        object stereo; //CameraStereoSettings
        object runtime; //CameraRuntime

        //Camera_Runtime runtime;

        public Camera()
        {

        }

        public Camera(IEnumerable<byte> _bytes, bool Bits64=true)
        {
            var index = 0;
            this.id = new ID(_bytes);
            index += 208;
            this.adt = Helpers.POINTER(_bytes, ref index, Bits64);
            this.type = (char)_bytes.ToArray()[index];
            index += 1;
            this.dtx = (char)_bytes.ToArray()[index];
            index += 1;
            this.flag = BitConverter.ToInt16(_bytes.Skip(index).ToArray());
            index += 2;
            this.passepartalpha = Helpers.FLOAT(_bytes, ref index);
            this.clip_start = Helpers.FLOAT(_bytes, ref index);
            this.clip_end = Helpers.FLOAT(_bytes, ref index);
            this.lens = Helpers.FLOAT(_bytes, ref index);
            this.ortho_scale = Helpers.FLOAT(_bytes, ref index);
            this.drawsize = Helpers.FLOAT(_bytes, ref index);
            this.sensor_x = Helpers.FLOAT(_bytes, ref index);
            this.sensor_y = Helpers.FLOAT(_bytes, ref index);
            this.shiftx = Helpers.FLOAT(_bytes, ref index); 
            this.shifty = Helpers.FLOAT(_bytes, ref index);
            this.dof_distance = Helpers.FLOAT(_bytes, ref index);

            this.sensor_fit = Helpers.CHAR(_bytes,ref index);
            this.panorama_type = Helpers.CHAR(_bytes, ref index);
            this._pad = Helpers.PAD(_bytes, ref index, 2);

            this.fisheye_fov = Helpers.FLOAT(_bytes, ref index);
            this.fisheye_lens = Helpers.FLOAT(_bytes, ref index);
            this.latitude_min = Helpers.FLOAT(_bytes, ref index);
            this.latitude_max = Helpers.FLOAT(_bytes, ref index);
            this.longitude_min = Helpers.FLOAT(_bytes, ref index);
            this.longitude_max= Helpers.FLOAT(_bytes, ref index);
            this.fisheye_polynomial_k0 = Helpers.FLOAT(_bytes, ref index);
            this.fisheye_polynomial_k1 = Helpers.FLOAT(_bytes, ref index);
            this.fisheye_polynomial_k2 = Helpers.FLOAT(_bytes, ref index);
            this.fisheye_polynomial_k3 = Helpers.FLOAT(_bytes, ref index);
            this.fisheye_polynomial_k4= Helpers.FLOAT(_bytes, ref index);


            this.central_cylindrical_range_u_min= Helpers.FLOAT(_bytes, ref index);
            this.central_cylindrical_range_u_max = Helpers.FLOAT(_bytes, ref index);
            this.central_cylindrical_range_v_min= Helpers.FLOAT(_bytes, ref index);
            this.central_cylindrical_range_v_max= Helpers.FLOAT(_bytes, ref index);
            this.central_cylindrical_radius= Helpers.FLOAT(_bytes, ref index);
            this._pad2= Helpers.FLOAT(_bytes, ref index);

            this.ipo = Helpers.POINTER(_bytes, ref index, Bits64);
            this.dof_ob = Helpers.POINTER(_bytes, ref index, Bits64);

            this.gpu_dof = Helpers.BYTEARRAY(_bytes, ref index, 32);
            this.dof = Helpers.BYTEARRAY(_bytes, ref index, 96);
            this.bg_images = ListBase.FromBytes(_bytes, ref index);
            this.stereo = Helpers.BYTEARRAY(_bytes, ref index, 24);
            this.runtime = Helpers.BYTEARRAY(_bytes, ref index, 216);



        }

        public void Split(out ID Id, out CameraType Type, out Stride.Core.Mathematics.Vector2 Cliping, out float Lens, out Stride.Core.Mathematics.Vector2 Sensor, out Stride.Core.Mathematics.Vector2 Shift)
        {
            Id = this.id;
            Type = (CameraType)this.type;
            Cliping = new Stride.Core.Mathematics.Vector2(this.clip_start, this.clip_end);
            Lens = this.lens;
            Sensor = new Stride.Core.Mathematics.Vector2(this.sensor_x, this.sensor_y);
            Shift = new Stride.Core.Mathematics.Vector2(this.shiftx, this.shifty);
        }

    }

    public enum CameraType
    {
        Perspective = 0,
        Ortho = 1,
        Panoramic = 2
    }

    public enum dtx
    {
        Center = 1 << 0,
        CenterDiag = 1 << 1,
        Thirds = 1 << 2,
        Golden = 1 << 3,
        GoldenTriA = 1 << 4,
        GoldenTriB = 1 << 5,
        HarmonyTriA = 1 << 6,
        HarmonyTriB = 1 << 7,

    }


    public enum CameraFlag
    {
        ShowLimits = 1 << 0,
        ShowMist = 1 << 1,
        ShowPassepartout = 1 << 2
    }

    public enum SensorFit
    {
        Auto = 0,
        Horizontal = 1,
        Vertical = 2
    }

    public class CameraRuntime()
    {
        float[][][] drw_corners;
    }
}
