using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VL.BlenderUtils.Parser.DNA;

namespace VL.BlenderUtils.Parser.Native
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DNA_DEPRECATED : Attribute
    {
    }
    public static class BlenderMarshal
    {
        
        public static T ReadFromBytes<T>(byte[] bytes, BlendFile blendFile) where T : new()
        {

            //Get the DNA structure which correspond to the native class/struct
            var nativeClassName = typeof(T).Name;
            Console.WriteLine(nativeClassName);
            DNAStructure dnaStruct = blendFile.GetDNACatalog().Structures.Find(x => x.TypeName == nativeClassName);
            var dnaStructFields = dnaStruct.Fields;
            var size = dnaStruct.Size;
            //size = BlenderMarshal.SizeOf(typeof(T));
            //Create new istance of the output class/struct
            T result = Activator.CreateInstance<T>();
            // 2. Box the struct. This is the key step.
            object boxedResult = result;


            
            var nativeFields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);

            //Iterate through all the Fields of the native struct

            foreach (FieldInfo field in nativeFields)
            {
                //Lookup the dna structures collection and locate the exact dna field that matches the exact name of the native class fields

                var dnaEquivalentField = dnaStruct.Fields.Find(x => x.GetShortName() == field.Name);
                var dnaFName = "------";
                object value = null;

                var isDepricated = field.GetCustomAttribute<DNA_DEPRECATED>() == null ? false : true;
                //Console.WriteLine(isDepricated);

                if (dnaEquivalentField != null)
                {

                    var offset = dnaEquivalentField.Offset;
                    dnaFName = dnaEquivalentField.GetShortName();
                    var dnaFType = dnaEquivalentField.InnerType;
                    var dnaSize = dnaEquivalentField.CalculatedSize;
                    var NFType = field.FieldType;
                    var NFName = field.Name;
                    var _bytes = bytes.Skip(offset).Take(dnaSize).ToArray();
                    //Addition to get the size manually from class
                    dnaSize = BlenderMarshal.SizeOf(NFType, false); 
                    if (dnaFType == FieldType.Pointer || NFType == typeof(IntPtr))
                    {
                        _bytes = bytes.Skip(offset).ToArray();
                        if (blendFile.header.PointerSize == 8)
                            value = new IntPtr(BitConverter.ToInt64(_bytes));
                        else
                            value = new IntPtr(BitConverter.ToInt32(_bytes));
                        
                        dnaSize = Marshal.SizeOf<IntPtr>();
                    }

                    else if (dnaFType == FieldType.Array)
                    {
                        if (!dnaEquivalentField.IsMultiDimArray && dnaFName.Contains("pad")) { value = new byte[dnaSize]; }
                        if (!dnaFName.Contains("pad") && !dnaEquivalentField.IsMultiDimArray && dnaEquivalentField.Type.Name == "char") { value = ReadString(bytes, offset, dnaSize); }
                    }

                    else if (dnaFType == FieldType.StructType)
                    {

                        if (NFType == typeof(ID))
                        {
                            _bytes = bytes.Skip(offset).ToArray();
                            value = ReadFromBytes<ID>(_bytes, blendFile);
                            
                        }
                        if(NFType == typeof(Camera))
                        {
                            _bytes = bytes.Skip(offset).ToArray();
                            value = ReadFromBytes<Camera>(_bytes, blendFile);
                            
                        }
                        else if (NFType == typeof(Native.Object))
                        {

                        }
                        else
                        {
                            value = "Struct";
                        }

                    }
                    else if (dnaFType == FieldType.ValueType)
                    {
                        if (NFType == typeof(Int32)) value = BitConverter.ToInt32(_bytes);
                        if (NFType == typeof(Int16)) value = BitConverter.ToInt16(_bytes);
                        if (NFType == typeof(Single)) value = BitConverter.ToSingle(_bytes);
                        if (NFType == typeof(short)) value = BitConverter.ToInt16(_bytes);
                        if (NFType == typeof(ushort)) value = BitConverter.ToUInt16(_bytes);
                        else
                        {
                            value = "Uknown";
                        }
                    }

                    //Console.WriteLine($"{offset} Native: {field.Name}|{NFType.Name} \t\t DNA: {dnaFName}|{dnaFType}|{dnaSize}");
                    if (value != null)
                    {
                        //Console.WriteLine($"{offset} - {NFType} {NFName} = {value} | {dnaSize}"); 
                        if (value.GetType() == NFType)
                        {
                            field.SetValue(boxedResult, value);
                        }
                    }
                    
                }
            }

            Console.WriteLine($"Native Auto Generated Class {result.GetType()}");
            return (T)boxedResult;

        }



        public static string ReadString(byte[] bytes, int offset, int size)
        {
            if (size == 0 || offset + size > bytes.Length)
            {
                return string.Empty;
            }

            var stringBytes = new byte[size];
            Array.Copy(bytes, offset, stringBytes, 0, size);

            // Find the index of the null terminator.
            int nullIndex = Array.IndexOf(stringBytes, (byte)0);

            // Decode the string, ensuring we stop at the null terminator if found.
            if (nullIndex >= 0)
            {
                return Encoding.ASCII.GetString(stringBytes, 0, nullIndex);
            }
            else
            {
                // No null terminator found, read the entire allocated size.
                return Encoding.ASCII.GetString(stringBytes);
            }
        }

        public static int SizeOf(Type Type, bool ExcludeDeprecated = false)
        { 
            int size = 0;
            //var Type = Value.GetType();
            var Fields = Type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            if (ExcludeDeprecated)
            {
                Fields = Type.GetFields(BindingFlags.Instance | BindingFlags.Public).Where(x => x.GetCustomAttribute<DNA_DEPRECATED>() != null).ToArray();
            }
                

            foreach (var field in Fields)
            {
                if(field.FieldType.IsArray || field.FieldType == typeof(string))
                {
                    var SizeConst = field.GetCustomAttribute<MarshalAsAttribute>().SizeConst;
                    
                    size += SizeConst;
                }
                else if(field.FieldType.IsPrimitive)
                {
                    size += Marshal.SizeOf(field.FieldType);
                    //Console.WriteLine($"{field.FieldType} {field.Name}");
                }
                else
                {
                    size += BlenderMarshal.SizeOf(field.FieldType);
                    //Console.WriteLine($"Structs: {field.FieldType} {field.Name}");
                }
                
            }

            return size;


        }
    }
}
