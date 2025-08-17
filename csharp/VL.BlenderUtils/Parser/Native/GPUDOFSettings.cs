using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.Native
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct GPUDOFSettings
    {
        public float focus_distance;
        public float fstop;
        public float focal_length;
        public float sensor;
        public float rotation;
        public float ratio;
        public int num_blades;
        public int high_quality;
    }
}
