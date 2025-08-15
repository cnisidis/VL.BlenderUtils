using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.DNA
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CustomData_MeshMasks
    {
        public ulong vmask;
        public ulong emask;
        public ulong fmask;
        public ulong pmask;
        public ulong lmask;
    }
}
