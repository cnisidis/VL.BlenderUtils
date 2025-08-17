using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct rctf
    {
        public float xmin;
        public float xmax;
        public float ymin;
        public float ymax;
    }
}
