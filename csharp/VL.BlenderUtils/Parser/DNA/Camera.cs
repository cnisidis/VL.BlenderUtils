using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.DNA
{
    //https://github.com/dfelinto/blender/blob/master/source/blender/makesdna/DNA_camera_types.h#L75
    public class Camera
    {
        ID id;

        //AnimData adt;

        char type;

        char dtx;
        short flag;
        float passepartalpha;
        float clip_start, clip_end;
        float lens, ortho_scale, drawsize;
        float sensor_x, sensor_y;
        float shiftx, shifty;
        float dof_distance;

        //Object dof_ob
        //GUDOFSettings gpu_dof
        //CameraDOFSettings dof;

        //ListBase bg_images;

        char sensor_fit;
        string _pad; //[7]

        //CameraStereoSettings stereo

        //Camera_Runtime runtime;



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
}
