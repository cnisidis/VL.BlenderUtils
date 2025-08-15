using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser
{
    public class Type
    {
        int AlignOf;
        int SizeOf;
        TypeCode Code;
        bool Dynamic;
        string? Name;
        string? Tag;
        object ObjectFile;

        //public PemFields => new List<>

    }

   
}
