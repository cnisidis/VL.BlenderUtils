using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VL.BlenderUtils.Parser
{
    public class Patcher
    {
        public DNADummyObject Root;

        public Patcher()
        {

        }

        public T Patch<T>(DNADummyObject dummy)
        {
            var type = typeof(T);
            var fields = type.GetFields();
            T result = Activator.CreateInstance<T>();

            object boxedResult = result;

            foreach (var field in fields) 
            {
                var fValue = dummy.GetValue(field.Name);
                if(fValue != null)
                {
                    if(fValue == typeof(DNADummyObject))
                    {
                        //Do something for Dummy Object (recursive)
                    }
                    else
                    {
                        field.SetValue(boxedResult, fValue);
                    }
                }
            }

            return (T)boxedResult;
        }

        public partial class DNADummyObject
        {
            public bool isRoot;
            public string Name;
            public DNADummyObject Parent;
            Dictionary<string, object> DataFields = new();

            public DNADummyObject(string Name, DNADummyObject? parent)
            {
                this.isRoot = parent == null ? true : false;
                this.Name = Name;
                this.Parent = parent;
            }

            public string ToString()
            {
                return string.Join("\n", DataFields.SelectMany(x => x.Key + "=" + x.Value.ToString()));
            }

            public void AddField(string name, object? value)
            {

                var add = DataFields.TryAdd(name, value);

            }

            public Dictionary<string, object> GetFields()
            {
                return this.DataFields;
            }

            public object GetValue(string fieldName)
            {
                this.DataFields.TryGetValue(fieldName, out var value);
                return value;
            }
        }
    }
}
