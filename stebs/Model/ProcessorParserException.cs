using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Stebs.Model
{
    public class ProcessorParserException : ApplicationException
    {
        public ProcessorParserException() {}
        public ProcessorParserException(String  message): base(message) {}
        public ProcessorParserException(String message, Exception inner) : base(message,inner) {}
        
        // Constructor needed for serialization 
        // when exception propagates from a remoting server to the client.
        protected ProcessorParserException(System.Runtime.Serialization.SerializationInfo info,
        StreamingContext context) {}
    }
}
