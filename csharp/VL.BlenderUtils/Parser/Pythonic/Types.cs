using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.Pythonic
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

    public class TypeCode
    {

    }

    public class Field
    {
        int bitPos;
        int enumVal;
        string Name;
        bool artificial;
        bool isBaseClass;
        int bitSize;
        Type type;
        Type parentType;
    }
}
