using Stride.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.DNA
{
    public enum FieldType
    {
        Unknown,
        Pointer,
        FunctionPointer,
        ValueType,
        StructType,
        Array,
        MultiDimArray,
    }

    public struct ResolvedTypeInfo
    {
        public FieldType FieldType;
        public System.Type SystemType;
        public List<int> ArraySizes;
        public int PointerLevel;
        public bool IsMultiDimArray;
        public int CalculatedSize;
    }

    /// <summary>
    /// DNAField is a coupled DNAType and DNAName.
    /// </summary>
    public class DNAField
    {
        public DNAType Type;
        public string Name;
        
        public int Size;

        // Resolved properties
        public FieldType InnerType { get; private set; }
        public System.Type SystemType { get; private set; }
        public List<int> ArraySizes { get; private set; } = new List<int>();
        public int PointerLevel { get; private set; }
        public bool IsMultiDimArray { get; private set; }
        public int CalculatedSize { get; private set; } // The new property.

        public void Resolve(Dictionary<string, DNAType> allStructs)
        {
            var resolver = new DNATypeResolver(allStructs);
            var resolvedInfo = resolver.Resolve(this.Name, this.Type.Name);

            this.InnerType = resolvedInfo.FieldType;
            this.SystemType = resolvedInfo.SystemType;
            this.ArraySizes = resolvedInfo.ArraySizes;
            this.PointerLevel = resolvedInfo.PointerLevel;
            this.IsMultiDimArray = resolvedInfo.IsMultiDimArray;
            this.CalculatedSize = resolvedInfo.CalculatedSize;
        }


        public DNAField(string Name, DNAType Type)
        {
            this.Type = Type;
            this.Name = Name;
            
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

    




}
