using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser
{
    public class Field
    {
        int bitPos;
        int enumVal;
        string name;
        bool artificial;
        bool isBaseClass;
        int bitsize;
        Type type;
        Type parentType;

    }

    public class Value
    {
        Type type;
        Value Address;
        bool isOptimizedOutput;
        Type dynamicType;
        bool isLazy;

        public void Dereference()
        {

        }

        public void ReferencedValue()
        {

        }

        public void ReferenceValue()
        {

        }

        public void ConstValue()
        {

        }


    }
}
