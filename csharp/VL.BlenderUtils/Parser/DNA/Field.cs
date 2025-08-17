using Stride.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.DNA
{

    /// <summary>
    /// DNAField is a coupled DNAType and DNAName.
    /// </summary>
    public class DNAField
    {
        public DNAType Type;
        public string Name;
        public FieldType InnerType;
        public int Size;


        public DNAField(string Name, DNAType Type)
        {
            this.Type = Type;
            this.Name = Name;
            this.ResolveField();
        }


        public string ToString()
        {
            return this.Name + this.Type.ToString();
        }


        public void Split(out string Name, out DNAType Type)
        {
            Name = this.Name;
            Type = this.Type;
        }
        private void ResolveField()
        {

            //define type
            if (Name.IndexOf('*') > -1) InnerType = FieldType.Pointer ;
            else if (Name.Contains("(*")) InnerType = FieldType.Method;
            else if (Name.Contains("[")) 
            {
                InnerType = FieldType.Array;
                //check how many arrays it holds
                //check the DNAType.Name
            }
            else
            {
                if (Type.Name == "char") InnerType = FieldType.Byte;
                else if (Type.Name == "short" || Type.Name == "int") InnerType = FieldType.Int;
                else if (Type.Name == "float") InnerType = FieldType.Float;
                else InnerType = FieldType.Struct;
            }
        }

        public dynamic Cast()
        {
            return null;
        }

        public string GetShortName()
        {
            var result = this.Name;
            result = result.Replace("*", "");
            result = result.Replace("(", "");
            result = result.Replace(")", "");
            var index = result.ToCharArray().IndexOf('[');
            if (index != -1)
            {
                result = new string(result.ToCharArray().Take(index).ToArray());
            }

            return result;
        }
    }

    public enum FieldType
    {
        Pointer,
        Method,
        String, 
        Array,
        Float,
        Int,
        Byte,
        Matrix,
        Struct

    }


    
}
