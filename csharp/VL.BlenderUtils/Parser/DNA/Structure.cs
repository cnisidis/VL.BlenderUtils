using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VL.Lib.Collections;

namespace VL.BlenderUtils.Parser.DNA
{
    public class DNAStructure
    {

        public string TypeName;
        public List<DNAField> Fields { get; set; } = new();

        public  DNAStructure(string Name)
        {
            TypeName = Name;
        }
        public void ToString(out string Result)
        {
            Result = this.Fields.Count().ToString();
        }

        public void Split(out string Name, out Spread<DNAField> Fields)
        {
            Name = TypeName;
            Fields = this.Fields.ToSpread();
        }
    }
}
