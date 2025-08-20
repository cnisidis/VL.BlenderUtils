using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VL.BlenderUtils.Parser.Native;

namespace VL.BlenderUtils.Parser.DNA
{
    public static class sDNA
    {
        public static DNAStructure GetDNAMetadata(string structNameInFile, DNACatalog DNACatalog)
        {
            var sdnaStruct = DNACatalog.Structures.Find(x=>x.TypeName == structNameInFile);
            return sdnaStruct;
        }
        public static T ReadStructFromBuffer<T>(byte[] dataBlockBytes) where T : new()
        {
            var result = new T();
            var offset = 0;

            // This is where we safely and correctly handle the struct's fields.
            foreach (var field in typeof(T).GetFields())
            {
                // First, get the size of the field.
                int fieldSize = Marshal.SizeOf(field.FieldType);

                // Check if the field is marked as deprecated.
                bool isDeprecated = field.IsDefined(typeof(DNA_DEPRECATED), false);

                if (isDeprecated)
                {
                    Console.WriteLine($"Skipping field '{field.Name}' as it is deprecated. Advancing offset.");
                    offset += fieldSize;
                    continue; // Skip to the next field in the loop.
                }

                // This is the safe way to read a struct from a buffer.
                // We'll read directly from the in-memory byte array, not from an IntPtr.
                IntPtr tempPtr = Marshal.AllocHGlobal(fieldSize);
                Marshal.Copy(dataBlockBytes, offset, tempPtr, fieldSize);

                try
                {
                    object value = Marshal.PtrToStructure(tempPtr, field.FieldType);
                    field.SetValue(result, value);
                    Console.WriteLine($"Parsed field '{field.Name}' of type {field.FieldType.Name}.");
                }
                finally
                {
                    Marshal.FreeHGlobal(tempPtr);
                }

                offset += fieldSize;
            }

            return result;
        }
        public static T ReadStruct<T>(IntPtr ptr, string structNameInFile, BlendFile blendFile) where T : new()
        {
            // 1. Get the SDNA metadata for the struct from the file.
            var sdnaMetadata = sDNA.GetDNAMetadata(structNameInFile, blendFile.GetDNACatalog());

            // 2. Read the raw bytes from the memory location.
            byte[] buffer = new byte[sdnaMetadata.Size];
            Marshal.Copy(ptr, buffer, 0, buffer.Length);

            // 3. Create an instance of your hardcoded C# struct.
            var result = new T();

            // 4. Manually assign values based on SDNA offsets and types.
            foreach (var field in sdnaMetadata.Fields)
            {
                // This is where the complex logic would go.
                // For example:
                if (field.GetShortName() == "name")
                {
                    // Read the string from the buffer at the correct offset
                    // and with the correct size.
                    // This is the manual string marshaling we discussed.
                }
                else if (field.Name == "tag")
                {
                    // Read the integer from the buffer.
                    //int tagValue = BitConverter.ToInt32(buffer, field.Offset);
                    // Now, you need to set the `tag` field of your C# struct.
                    // This requires using reflection, which is a big performance hit.
                    // That's why the Generator pattern is often preferred.
                }
            }

            return result;
        }

        

        public static byte[] ReadRawBytes(IntPtr ptr, int length)
        {
            if (ptr == IntPtr.Zero)
            {
                return new byte[0];
            }
            if (length <= 0)
            {
                return new byte[0];
            }

            byte[] buffer = new byte[length];

            // This is the key line. It copies 'length' bytes from the
            // unmanaged memory at 'ptr' into our managed 'buffer'.
            try
            {
                Console.WriteLine($"Copy from {ptr} with length of {length}");
                Marshal.Copy(ptr, buffer, 0, length);
            }
            catch (Exception ex) { 
                Console.WriteLine(ex.ToString());
            }
            

            return buffer;
        }
    }


    public class DNATypeResolver
    {
        private static readonly Dictionary<string, (System.Type type, int size)> BaseTypeMap = new Dictionary<string, (System.Type, int)>
    {
        { "int", (typeof(int), 4) },
        { "short", (typeof(short), 2) },
        { "float", (typeof(float), 4) },
        { "double", (typeof(double), 8) },
        { "char", (typeof(char), 1) },
        { "void", (typeof(void), 0) },
        
    };

        private readonly Dictionary<string, DNAType> _allStructs;

        // Regex for standard fields (data pointers, arrays, value types).
        private static readonly Regex FieldNameRegex = new Regex(@"^(\*+)?(\w+)(?:\[(\d+)\])*$");

        // New regex for a function pointer.
        private static readonly Regex FunctionPointerRegex = new Regex(@"^\(\*(\w+)\)\(\)$");

        public DNATypeResolver(Dictionary<string, DNAType> allStructs)
        {
            _allStructs = allStructs;
        }

        public ResolvedTypeInfo Resolve(string fieldName, string fieldTypeName)
        {
            var resolvedInfo = new ResolvedTypeInfo();

            // --- Step 1: Check for function pointers. ---
            var funcPtrMatch = FunctionPointerRegex.Match(fieldName);
            if (funcPtrMatch.Success)
            {
                if(fieldTypeName == "void")
                {
                    resolvedInfo.FieldType = FieldType.FunctionPointer;
                    resolvedInfo.SystemType = typeof(IntPtr);
                    resolvedInfo.PointerLevel = 1;
                    resolvedInfo.CalculatedSize = IntPtr.Size;
                }
                else
                {
                    resolvedInfo.FieldType = FieldType.Void;
                    resolvedInfo.SystemType = null;
                    resolvedInfo.CalculatedSize = 0;
                }
                
                return resolvedInfo;
            }

            // --- Step 2: Parse standard field types. ---
            var match = FieldNameRegex.Match(fieldName);

            if (!match.Success)
            {
                throw new ArgumentException($"Field name '{fieldName}' could not be parsed.");
            }

            int pointerLevel = match.Groups[1].Success ? match.Groups[1].Value.Length : 0;
            resolvedInfo.PointerLevel = pointerLevel;

            // Capture all array dimensions, if any.
            resolvedInfo.ArraySizes = new List<int>();
            foreach (Capture capture in match.Groups[3].Captures)
            {
                resolvedInfo.ArraySizes.Add(int.Parse(capture.Value));
            }
            resolvedInfo.IsMultiDimArray = resolvedInfo.ArraySizes.Count > 1;

            // --- Step 3: Calculate the size and determine the type. ---
            if (resolvedInfo.PointerLevel > 0)
            {
                resolvedInfo.SystemType = typeof(IntPtr);
                resolvedInfo.FieldType = FieldType.Pointer;
                resolvedInfo.CalculatedSize = IntPtr.Size;
            }
            else if (resolvedInfo.ArraySizes.Count > 0)
            {
                int baseTypeSize;
                System.Type baseType;
                if (BaseTypeMap.TryGetValue(fieldTypeName, out var typeInfo))
                {
                    baseType = typeInfo.type;
                    baseTypeSize = typeInfo.size;
                }
                else if (_allStructs.TryGetValue(fieldTypeName, out var dnaType))
                {
                    // We'll use a placeholder System.Type for custom structs.
                    baseType = typeof(object);
                    baseTypeSize = dnaType.Size;
                }
                else
                {
                    throw new InvalidOperationException($"Base type '{fieldTypeName}' for array is unknown.");
                }

                resolvedInfo.SystemType = baseType == typeof(char) ? typeof(string) : baseType.MakeArrayType();
                resolvedInfo.FieldType = resolvedInfo.IsMultiDimArray ? FieldType.MultiDimArray : FieldType.Array;

                // Calculate the total size of the array.
                int arrayLength = resolvedInfo.ArraySizes.Aggregate(1, (a, b) => a * b);
                resolvedInfo.CalculatedSize = arrayLength * baseTypeSize;
            }
            else if (BaseTypeMap.TryGetValue(fieldTypeName, out var typeInfo))
            {
                resolvedInfo.SystemType = typeInfo.type;
                resolvedInfo.FieldType = FieldType.ValueType;
                resolvedInfo.CalculatedSize = typeInfo.size;
            }
            else if (_allStructs.TryGetValue(fieldTypeName, out var dnaType))
            {
                resolvedInfo.SystemType = typeof(object);
                resolvedInfo.FieldType = FieldType.StructType;
                resolvedInfo.CalculatedSize = dnaType.Size;
            }
            else
            {
                resolvedInfo.SystemType = typeof(object);
                resolvedInfo.FieldType = FieldType.Unknown;
                resolvedInfo.CalculatedSize = 0; // Unknown size
            }

            return resolvedInfo;
        }
    }

}
