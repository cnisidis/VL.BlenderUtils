using Stride.Core.Extensions;


namespace VL.BlenderUtils.Parser.DNA
{
    public class DNAName
    {
        public string Name { get; }
        public DNAName(string name)
        {
            this.Name = name;
        }

        public string AsReference(string parent)
        {
            string result = string.Empty;

            if (parent == null)
            {
                return "";
            }
            else return parent + ".";
        }

        public string ShortName()
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

        public bool IsPointer()
        {
            return Name.IndexOf('*') > -1;
        }

        public bool IsMethodPointer()
        {
            return Name.Contains("(*");
        }

        public int ArraySize()
        {
            var result = 1;

            var tmp = Name;
            var idx = Name.IndexOf('[');

            while (idx != -1)
            {
                var idx2 = tmp.IndexOf(']');
                var mult = int.Parse(new string((tmp.Skip(idx + 1).Take(idx2).ToArray())));
                result *= mult;
                tmp = new string(tmp.Skip(idx2 + 1).ToArray());
                idx = tmp.IndexOf('[');
            }


            return result;
        }

    }
}
