using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
