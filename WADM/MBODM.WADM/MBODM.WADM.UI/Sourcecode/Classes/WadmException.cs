using System;
using System.Runtime.Serialization;

namespace MBODM.WADM.UI
{
    [Serializable]
    public class WadmException : Exception
    {
        public WadmException() { }
        public WadmException(string message) : base(message) { }
        public WadmException(string message, Exception inner) : base(message, inner) { }
        protected WadmException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
