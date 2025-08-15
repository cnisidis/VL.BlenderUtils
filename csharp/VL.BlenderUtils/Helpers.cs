
using System.Runtime.InteropServices;



namespace VL.BlenderUtils
{

    public enum ReaderType
    {
        US,
        S,
        UI,
        I,
        F,
        UL,
        L,
        P,
    }
    

    public static class Helpers
    {

        /// <summary>
        /// Marshals data from a byte array to a struct, starting at a specific offset.
        /// This method pins the byte array to avoid memory copying, making it efficient.
        /// </summary>
        /// <typeparam name="T">The type of the struct to marshal.</typeparam>
        /// <param name="byteArray">The source byte array.</param>
        /// <param name="offset">The offset in the array to begin reading from.</param>
        /// <returns>A new instance of the struct populated with the data.</returns>
        public static T BytesToStruct<T>(byte[] byteArray, int offset) where T : struct
        {
            // Allocate a GCHandle to pin the byte array in memory.
            GCHandle handle = GCHandle.Alloc(byteArray, GCHandleType.Pinned);
            try
            {
                // Get a pointer to the start of the pinned byte array.
                nint pointer = handle.AddrOfPinnedObject();

                // Add the offset to the pointer to get the correct starting address for the struct.
                pointer = nint.Add(pointer, offset);

                // Marshal the data from the pointer into the struct.
                return (T)Marshal.PtrToStructure(pointer, typeof(T));
            }
            finally
            {
                // IMPORTANT: Always free the handle in a finally block to unpin the array.
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }
        
    }
}
