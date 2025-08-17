using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.DNA
{
    public class DNAType
    {
        public string Name;
        public int Size;
        
        public DNAType(string name, int size)
        {
            this.Name = name;
            this.Size = size;
        }

    }
}
