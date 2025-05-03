using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using VL.BlenderUtils.Parser.Pythonic;

namespace VL.BlenderUtils.Parser.DNA
{
    //https://github.com/blender/blender/blob/main/source/blender/makesdna/DNA_scene_types.h#L2004
    public class Scene
    {
        ID id;
        ulong ptrAnimData; //AnimData;
        ulong ptrCamera; //Camera
        ulong ptrWorld; //World
        ulong ptrSet; //Scene

        ListBase Base; //ListBase DNA_DEPRECATED;

        ulong ptrBaseact;


        byte[] cursor; //View3DCursor

        int lay;

        int layact;

        char[] _pad2 = new char[4];

        short flag;

        char use_nodes;

        char[] _pad3 = new char[1];

        ulong ptrBNodeTree;

        ulong ptrEd;
        ulong ptrToolSettings;
        object _pad4;

        byte[] safe_areas; //DisplaySafeAreas

        RenderData r; //RenderData

        byte[] Audio;

        byte[] markers; //ListBase
        byte[] transform_space; //ListBase

        byte[] orientation_slots; //TransformOrientationSlot

        byte[] ptrSound_scene;
        byte[] ptrPlayback_handle;

        byte[] sound_scrub_handle;
        byte[] speaker_handles;

        byte[] fps_info;

        int ptrDepsgraph_hash;//GHash

        char[] _pad7 = new char[4];

        int active_keyingsets;

        byte[] keyingsets; //ListBase;

        byte[] unit; //UnitSettings;

        int ptrGpd;  //bGPData

        int ptrClip; //MovieClip

        byte[] PhysicsSettings;

        byte[] _pad8;

        byte[] customdata_mask; //CustomData_MeshMasks
        byte[] customdata_mask_model; //CustomData_MeshMasks

        byte[] view_settings; //ColorManagedViewSettings;
        byte[] display_settings; //ColorManagedDisplaySettings
        byte[] sequencer_colorspace_settings; //ColorManagedColorSpaceSettings

        int ptrRigidbody_world; //rigidBodyWorld

        int ptrPreview;

        byte[] view_layer; //ListBase
        int ptrMaster_collection; //Collection

        int ptrLayer_properties; //IDProperty

        int simulation_frame_start;
        int simulation_frame_end;

        byte[] display; //SceneDisplay
        byte[] eevee; //SceneEEVEE
        byte[] grease_pencil_settings; //SceneGpencil
        byte[] hydra; // SceneHydra

        int ptrRuntime; //SceneRuntimeHandle;

        int _pad9;

        public Scene()
        {

        }

        


        public static Scene Read(BinaryReader handle, Pythonic.BlendFile.Header header)
        {
            Scene scn = new Scene();
            Console.WriteLine(handle.BaseStream.Position.ToString());
            scn.id = Reader.ReadBlock(handle, "ID", header);
            Console.WriteLine(handle.BaseStream.Position.ToString());
            var animdata = Reader.Read(ReaderType.P, handle, header);
            scn.ptrAnimData = animdata;
            
            scn.ptrCamera = Reader.Read(ReaderType.P, handle, header);
            scn.ptrWorld = Reader.Read(ReaderType.P, handle, header);
            scn.ptrSet = Reader.Read(ReaderType.P, handle, header);
            scn.Base = Reader.ReadBlock(handle, "ListBase", header);
            Console.WriteLine(animdata);
            return scn;

        }

        public void Split(out ID Id, out ulong Camera, out RenderData RenderData)
        {
            Id = this.id;
            Camera = this.ptrCamera;
            RenderData = this.r;
        }


    }
}
