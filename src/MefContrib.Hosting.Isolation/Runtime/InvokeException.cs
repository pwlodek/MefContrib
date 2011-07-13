namespace MefContrib.Hosting.Isolation.Runtime
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception thrown when there is an arror when invoking any member on a remotely activated part.
    /// </summary>
    [Serializable]
    public class InvokeException : Exception
    {
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