using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MBODM.WoW
{
    [Serializable]
    public class CurseParserException : Exception
    {
        public CurseParserException() { }
        public CurseParserException(string message) : base(message) { }
        public CurseParserException(string message, Exception inner) : base(message, inner) { }
        protected CurseParserException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
