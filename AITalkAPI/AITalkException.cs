using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace AITalkAPI
{
    [Serializable]
    public class AITalkException : Exception
    {
        public AITalkException() { }

        public AITalkException(string message)
            : base(message) { }

        internal AITalkException(string message, AITalkCore.Result result)
            : base($"{message}({result})") { }

        public AITalkException(string message, Exception inner)
            : base(message, inner) { }

        protected AITalkException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
