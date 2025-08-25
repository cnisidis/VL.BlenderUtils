using Stride.Core.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using VL.BlenderUtils.Parser.DNA;
using VL.BlenderUtils.Parser.Native;
using VL.Core.Import;

namespace VL.BlenderUtils.Parser.Managed
{
    public class Scene
    {

        public int uuid {  get; set; }
        public string Name { get; set; }
        private string Type { get; set; }
        private int SelectedCamera { get; set; }
        public bool HasCamera { private set; get; }
        public RenderData RenderData { get; set; }
        public Scene()
        {
            RenderData = new RenderData();
        }

        public void SetSelectedCamera(int uuid)
        {
            this.SelectedCamera = uuid;
        }
        public void Split(out string Name, out int uuid, out RenderData RenderData)
        {
            Name = this.Name;
            uuid = this.uuid;
            RenderData = this.RenderData;
        }
        public static Scene FromDummyObject(Patcher.DNADummyObject dobj)
        {
            var scn = new Scene();
            var fullName = (string)dobj.GetObject("id").GetValue("name");
            var fullNameLength = fullName.Length;
            scn.Type = fullName.Substring(0,2);
            scn.Name = fullName.Substring(2);
            scn.uuid = (int)dobj.GetObject("id").GetValue("session_uid");
            var cam = dobj.GetObject("camera");
            if (cam !=null)
            {
                scn.SelectedCamera = (int)dobj.GetObject("camera").GetObject("id").GetValue("session_uid");
                scn.HasCamera = true;
            }

            scn.RenderData = RenderData.FromDummyObject(dobj.GetObject("r"));
            return scn;
        }

        public int GetSelectedCamera()
        {
            return this.SelectedCamera;
        }
    }
    
    public partial class RenderData
    {
        public Int2 Resolution { get; set; }
        public int CurrentFrame { get; set; }
        public Int2 StartEndFrames { get; set; }
        public Vector2 Aspect { get; set; }
        public string RenderEngine { get; set; }

        public static RenderData FromDummyObject(Patcher.DNADummyObject dobj)
        {
            var renderData = new RenderData();
            renderData.CurrentFrame = (int)dobj.GetValue("sfra");
            var sfra = (int)dobj.GetValue("sfra");
            var efra = (int)dobj.GetValue("efra");
            renderData.StartEndFrames = (new Int2(sfra, efra));

            var xasp = (float)dobj.GetValue("xasp");
            var yasp = (float)dobj.GetValue("yasp");
            renderData.Aspect = new Stride.Core.Mathematics.Vector2(xasp,yasp);

            var xsch = (int)dobj.GetValue("xsch");
            var ysch = (int)dobj.GetValue("ysch");
            renderData.Resolution = new Int2 (xsch,ysch);   

            renderData.RenderEngine = (string)dobj.GetValue("engine");

            return renderData;
        }

        public void Split(out string Engine, out Int2 StartEnd, out int CurrentFrame, out Int2 Resolution, out Vector2 Aspect)
        {
            StartEnd = this.StartEndFrames;
            CurrentFrame = this.CurrentFrame;
            Resolution = this.Resolution;
            Aspect = this.Aspect;
            Engine = this.RenderEngine;
        }
    }
}
