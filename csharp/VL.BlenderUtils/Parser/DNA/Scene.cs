using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.DNA
{
    //https://github.com/blender/blender/blob/main/source/blender/makesdna/DNA_scene_types.h#L2004
    public class Scene
    {
        ID id;
        IntPtr ptrAnimData; //AnimData;
        IntPtr ptrCamera; //Camera
        IntPtr ptrWorld; //World
        IntPtr ptrSet; //Scene

        ListBase Base; //ListBase DNA_DEPRECATED;

        IntPtr ptrBaseact;


        byte[] cursor; //View3DCursor

        int lay;

        int layact;

        char[] _pad2 = new char[4];

        short flag;

        char use_nodes;

        char[] _pad3 = new char[1];

        IntPtr ptrBNodeTree;

        IntPtr ptrEd;
        IntPtr ptrToolSettings;
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

        public Scene(IEnumerable<byte> bytes)
        {
            var index = 0;
            var _bytes = bytes.ToArray();
            
            id = new ID(_bytes);
            index += id.Size;
            
            ptrAnimData = (int)BitConverter.ToUInt64(_bytes.Skip(index).ToArray());
            index += IntPtr.Size;

            //Drawdata not presented in c file
            index += 16;

            ptrCamera = (IntPtr)BitConverter.ToUInt64(_bytes.Skip(index).ToArray());
            index += IntPtr.Size;
            
            ptrWorld = (IntPtr)BitConverter.ToUInt64(_bytes.Skip(index).ToArray());
            index += IntPtr.Size;

            ptrSet = (IntPtr)BitConverter.ToUInt64(_bytes.Skip(index).ToArray());
            index += IntPtr.Size;

            Base = new ListBase( _bytes.Skip(index).Take(16));
            index += 16;

            ptrBaseact = (int)BitConverter.ToUInt64(_bytes.Skip(index).ToArray());
            index += IntPtr.Size;

            //_pad1 void
            index += IntPtr.Size;

            cursor = _bytes.Skip(index).Take(64).ToArray();
            index += 64;
            
            lay = BitConverter.ToInt32(_bytes.Skip(index).ToArray());
            index += 4;
            layact = (int)BitConverter.ToInt32(_bytes.Skip(index).ToArray());
            index += 4;
            
            _pad2 = Encoding.UTF8.GetString( _bytes.Skip(index).Take(4).ToArray()).ToCharArray();
            index += 4;
            
            flag = BitConverter.ToInt16(_bytes.Skip(index).ToArray());
            index += 2;
            use_nodes = (char)_bytes[index];
            index += 1;
            
            _pad3 = Encoding.UTF8.GetString(_bytes.Skip(index).Take(1).ToArray()).ToCharArray();
            index += 1;
            
            ptrBNodeTree = (IntPtr)BitConverter.ToUInt64(_bytes.Skip(index).ToArray());
            index += IntPtr.Size;

            ptrEd = (IntPtr)BitConverter.ToUInt32(_bytes.Skip(index).ToArray());
            index += IntPtr.Size;

            ptrToolSettings = (int)BitConverter.ToUInt32(_bytes.Skip(index).ToArray());
            index += IntPtr.Size;

            //_pad4 void
            index += IntPtr.Size;

            safe_areas = _bytes.Skip(index).Take(32).ToArray();
            index += 32;

            r = new RenderData(_bytes.Skip(index));
            index += 4376;

        }

        public void Split(out ID Id, out IntPtr Camera, out RenderData RenderData)
        {
            Id = this.id;
            Camera = this.ptrCamera;
            RenderData = this.r;
        }


    }
}
