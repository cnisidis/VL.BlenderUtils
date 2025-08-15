using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.DNA
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ImageUser
    {
        // C++: struct Scene *scene;
        // A C++ pointer to a Scene struct, represented as an IntPtr in C#.
        public IntPtr scene;

        // C++: int framenr;
        public int framenr;

        // C++: int frames;
        public int frames;

        // C++: int offset, sfra;
        public int offset;
        public int sfra;

        // C++: char cycl;
        // C++ 'char' is a single byte. We use 'byte' in C#.
        public byte cycl;

        // C++: char multiview_eye;
        public byte multiview_eye;

        // C++: short pass;
        public short pass;

        // C++: int tile;
        public int tile;

        // C++: short multi_index, view, layer;
        public short multi_index;
        public short view;
        public short layer;

        // C++: short flag;
        public short flag;
    }
}
