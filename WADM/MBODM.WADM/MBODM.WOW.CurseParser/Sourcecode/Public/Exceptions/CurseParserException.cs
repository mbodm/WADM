using System;
using System.Runtime.Serialization;

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
