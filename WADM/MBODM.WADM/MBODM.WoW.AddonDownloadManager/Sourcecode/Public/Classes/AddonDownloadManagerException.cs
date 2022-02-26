using System;
using System.Runtime.Serialization;

namespace MBODM.WoW
{
    [Serializable]
    public class AddonDownloadManagerException : Exception
    {
        public AddonDownloadManagerException() { }
        public AddonDownloadManagerException(string message) : base(message) { }
        public AddonDownloadManagerException(string message, Exception inner) : base(message, inner) { }
        protected AddonDownloadManagerException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
