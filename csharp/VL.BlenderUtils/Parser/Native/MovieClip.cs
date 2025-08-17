using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.Native
{
    // C++: typedef struct MovieClipUser
    [StructLayout(LayoutKind.Sequential)]
    public struct MovieClipUser
    {
        // C++: int framenr;
        public int framenr;

        // C++: short render_size, render_flag;
        public short render_size;
        public short render_flag;
    }
}
