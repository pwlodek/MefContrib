using System;
using System.Runtime.Serialization;

namespace MefContrib.Hosting.Isolation.Runtime.Activation
{

    [Serializable]
    public class InvokeException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public InvokeException()
        {
        }

        public InvokeException(string message) : base(message)
        {
        }

        public InvokeException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvokeException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}