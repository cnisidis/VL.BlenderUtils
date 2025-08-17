
using VL.BlenderUtils.Parser.Native;

namespace VL.BlenderUtils.Parser.Managed
{
    public abstract class BlenderObjectBase 
    {
        protected readonly Native.Object _nativeObject;

        // The base class constructor takes the native object.
        public BlenderObjectBase(Native.Object nativeObject)
        {
            _nativeObject = nativeObject;
        }

        // Common properties implemented here for all child classes.
        public string Name =>_nativeObject.id.GetName();
        public float[] Location => _nativeObject.loc;
        public float[] Rotation => _nativeObject.rot;
        public float[] Scale => _nativeObject.scale;

        public ID GetID()
        {
            return _nativeObject.id;
        }

        
    }

    public class BlenderObject<T>: BlenderObjectBase where T : struct 
    {
        public string Type;
        
        // The native C struct that holds the raw data.
        private readonly Native.Object _nativeObject;

        // A reference to the main BlendFile class for pointer resolution.
        private readonly BlendFile _fileParser;

        // A private field to cache the resolved data. This ensures we only
        // read the data from the file once.
        private T? _dataCache;

        /// <summary>
        /// Initializes a new instance of the ManagedObject class.
        /// </summary>
        /// <param name="native">The native Object struct read from the file.</param>
        /// <param name="fileParser">The BlendFile instance used for resolving pointers.</param>
        public BlenderObject(Native.Object native, BlendFile fileParser):base(native) 
        {
            _nativeObject = native;
            _fileParser = fileParser;
            if(native.data != IntPtr.Zero)
                _dataCache = fileParser.ResolvePtr<T >(native.data);
            else _dataCache = null;
        }

        /// <summary>
        /// Lazily resolves the object's data pointer and returns the linked data block.
        /// The data is only loaded from the file on the first access.
        /// </summary>
        public T Data
        {
            get
            {
                /*
                // If the data has not been loaded yet, resolve it now.
                if (_dataCache == null)
                {
                    // The native.data field is the IntPtr pointer to the linked data block.
                    _dataCache = _fileParser.ResolvePtr<T>(_nativeObject.data);
                }
                */
                // Return the cached data.
                return _dataCache.Value;
            }
        }
    }

    
}
