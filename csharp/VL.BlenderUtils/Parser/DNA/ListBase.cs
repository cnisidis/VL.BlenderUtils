using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser.DNA
{
    public class ListBase
    {
        object first;
        object last;

        public int Size = 16;

        public ListBase()
        {

        }

        public ListBase(IEnumerable<byte> _bytes)
        {
            var bytes = _bytes.ToArray();
            var index = 0;

            first = bytes.Take(8);
            last = bytes.Skip(8);
        }

        public static ListBase FromBytes(IEnumerable<byte> bytes, ref int index)
        {
            index += 16;
            return new ListBase(bytes);
        }


        public static ListBase Read(BinaryReader handle, Pythonic.BlendFile.Header header)
        {
            var listBase = new ListBase();

            listBase.first = Reader.ReadBytes(handle, 8);
            listBase.last = Reader.ReadBytes(handle, 8);

            return listBase;

        }
    }
}
