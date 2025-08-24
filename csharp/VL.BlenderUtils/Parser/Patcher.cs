using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VL.BlenderUtils.Parser.DNA;
using VL.Lib.Collections;

namespace VL.BlenderUtils.Parser
{
    /*
     * Patcher <-> Dummy Objects | Native Classes Coupling
     */
    /// <summary>
    /// Patcher holds all the dummy objects created directly from sdna and compares them to native classes in order
    /// to update them later
    /// </summary>
    public class Patcher
    {
        public DNADummyObject Root;
        public List<DNADummyObject> Objects;
        public Patcher()
        {
            Objects = new();
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
                    if(fValue.GetType() == typeof(DNADummyObject))
                    {
                        //Do something for Dummy Object (recursive)
                    }
                    else
                    {
                        try
                        {
                            if(fValue.GetType() == field.GetType())
                                field.SetValue(boxedResult, fValue);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        
                    }
                }
            }

            return (T)boxedResult;
        }
        public partial record DummyValue
        {
            public bool isDummyObject = false;
            string dnaTypeName;
            public object? Value { private set; get; }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="dnaTypeName">The DNA Type name, can be the name of a struct or the type of the field</param>
            /// <param name="Value">Can be any value</param>
            public DummyValue(string dnaTypeName, object? Value)
            {
                
                this.Value = Value;
                if(this.Value != null && this.Value.GetType() == typeof(DNADummyObject))
                {
                    this.isDummyObject = true;
                }
                
            }

            public override string ToString()
            {
               return this.dnaTypeName +"  "+Value.ToString();
            }

        }
        /// <summary>
        /// Dummy Object holds any information (field.name | object) derives from sDNA parsing.
        /// </summary>
        public partial class DNADummyObject
        {
            
            public string Name;
            
            Dictionary<string, object?> DataFields = new();

            public DNADummyObject(string Name)
            {
               
                this.Name = Name;
                
            }

            public override string ToString()
            {
                //StringBuilder sb = new StringBuilder();
                //foreach(var f in DataFields) 
                //{
                //        sb.AppendLine(f.Key);
                //}
                //return sb.ToString();
                return this.Name;
            }

            public void AddField(string name, object? value)
            {
                
                var add = DataFields.TryAdd(name, value);

            }

            public Spread<object> GetFields()
            {
                return this.DataFields.Values.ToSpread();
            }

            public object GetValue(string fieldName)
            {
                this.DataFields.TryGetValue(fieldName, out object value);
                return value;
            }

            public DNADummyObject GetObject(string fieldName)
            {
                this.DataFields.TryGetValue(fieldName, out object value);
                
                return (DNADummyObject)value;
            }

            public Spread<KeyValuePair<string, object>> GetChildren()
            {
                

               return DataFields.ToSpread();
            }

            public Spread<string> GetFieldNames()
            {
                return DataFields.Select(x=>x.Key).ToSpread();
            }

            

            
        }
    }


}
