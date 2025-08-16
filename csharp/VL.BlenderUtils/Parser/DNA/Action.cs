using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.DNA
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct bAnimVizSettings
    {
        // C: short recalc;
        public short recalc;

        // C: short path_type;
        public short path_type;
        // C: short path_step;
        public short path_step;
        // C: short path_range;
        public short path_range;

        // C: short path_viewflag;
        public short path_viewflag;
        // C: short path_bakeflag;
        public short path_bakeflag;
        // C: char _pad[4];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] _pad;

        // C: int path_sf, path_ef;
        public int path_sf;
        public int path_ef;
        // C: int path_bc, path_ac;
        public int path_bc;
        public int path_ac;
    }
}
