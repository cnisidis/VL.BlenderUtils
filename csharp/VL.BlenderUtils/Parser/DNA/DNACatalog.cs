using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using VL.BlenderUtils.Parser;
using VL.Lib.Collections;
using static VL.BlenderUtils.Parser.BlendFile;
using System.Linq;

namespace VL.BlenderUtils.Parser.DNA
{
    public partial class DNACatalog
    {
        
        public List<DNAStructure> Structures;
        public Dictionary<string, DNAType> dnaTypes { get; }
        private Header _header;
        private BinaryReader _reader;
        public DNACatalog(Header header, BinaryReader handle)
        {
            
            dnaTypes = new Dictionary<string, DNAType>();
            Structures = new List<DNAStructure>();
            _header = header;
            _reader = handle;

            Prepare();
        }

        public static DNACatalog CreateCatalog()
        {
            return null;
        }
        public void Prepare()
        {

            List<string> _names = new List<string>();    
            List<string> _types = new List<string>();
            List<int> _sizes = new List<int>();
            
            //List<DNAStructure> dNAStructures = new List<DNAStructure>();

            var startOffset = _reader.BaseStream.Position;
            var SDNA = Reader.ReadString(_reader, 4);
            if (SDNA != "SDNA")
            {
                Console.WriteLine("SDNA tag is not parsed properly");
                return;
            }

            //names -> Name of the property (Field Name)
            var NAME = Reader.ReadString(_reader, 4);
            if (NAME != "NAME")
            {
                Console.WriteLine("NAME tag is not parsed properly");
                return;
            }
            var numberOfNames = Reader.Read(ReaderType.UI, _reader, _header);
            Console.WriteLine("Building {0:G} NAMES", numberOfNames);

            //Potential reason that NAMES are not offseted properly -> 
            //https://github.com/blender/blender/blob/main/source/blender/makesdna/intern/dna_genfile.cc#L377C23-L377C28
            /* "float gravity [3]" was parsed wrong giving both "gravity" and
             * "[3]"  members. we rename "[3]", and later set the type of
             * "gravity" to "void" so the offsets work out correct */

            for (int i = 0; i < numberOfNames; i++)
            {
                var name = Reader.ReadString(_reader);

                if (name.IndexOf("[") == 0 && Regex.IsMatch(name, @"(\[+\d+\])"))
                {
                    Console.WriteLine("{0:G} is not parsed properly", name);
                    var newName = _names[i - 1];
                    name = newName + name;

                }
                _names.Add(name);
            }

            Reader.AlignAlt(_reader, startOffset);

            var TYPE = Reader.ReadString(_reader, 4);

            if (TYPE != "TYPE")
            {
                Console.WriteLine("Error on Parsing Types - Alignment is wrong: {0:G}", TYPE);
                return;
            }



            var numberOfTypes = Reader.Read(ReaderType.UI, _reader, _header);
            Console.WriteLine("Building {0:G} TYPES", numberOfTypes);

            for (int i = 0; i < numberOfTypes; i++)
            {
                var type = Reader.ReadString(_reader);
                
                _types.Add(type);
            }
            Reader.AlignAlt(_reader, startOffset);

            //types lengths
            var TLEN = Reader.ReadString(_reader, 4);
            Console.WriteLine("Building {0:G} TYPE-LENGTHs", numberOfTypes);

            for (int i = 0; i < numberOfTypes; i++)
            {
                var length = Reader.Read(ReaderType.US, _reader, _header);
                //Get dnaType and set its size
                _sizes.Add(length);
                var dnaType = new DNAType(_types[i], length);
                dnaTypes.Add(_types[i], dnaType);
                //Types[i].Size = length;

            }
            Reader.AlignAlt(_reader, startOffset);

            //structs
            var STRC = Reader.ReadString(_reader, 4);
            if (STRC != "STRC")
            {
                return;
            }
            var numberOfStructs = Reader.Read(ReaderType.UI, _reader, _header);
            Console.WriteLine("Building {0:G} STRUCTS", numberOfStructs);


            for (int structureIndex = 0; structureIndex < numberOfStructs; structureIndex++)
            {

                var type = Reader.Read(ReaderType.US, _reader, _header);
                //string typeName = Types[type].Name;
                string typeName = _types[type];
               

                var numberOfFields = Reader.Read(ReaderType.US, _reader, _header);

                var structure = new DNAStructure(typeName);
                structure.Size = dnaTypes[typeName].Size;

                for (int fieldIndex = 0; fieldIndex < numberOfFields; fieldIndex++)
                {
                    var fTypeIndex = Reader.Read(ReaderType.US, _reader, _header);
                    var fNameIndex = Reader.Read(ReaderType.US, _reader, _header);
                    //Type Index -> better pick the DNAType
                    var fType = (DNAType)dnaTypes.Values.ToArray()[fTypeIndex];
                    //Field Name
                    var fName = _names[fNameIndex];

                    

                    var field = new DNAField(fName, fType);
                    field.Resolve(dnaTypes);
                    structure.Fields.Add(field);

                }
                
                Structures.Add(structure);
            }
            

            Console.WriteLine("Built DNA Catalog Succesfully");
        }

        public Dictionary<string, DNAType> GetTypes()
        {
            return this.dnaTypes;
        }

        public Spread<DNAStructure> GetStructures()
        {
            return Structures.ToSpread();
        }
    }
}
