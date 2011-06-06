using System;
using System.Runtime.Serialization;

namespace MefContrib.Hosting.Isolation.Runtime.Activation
{
    [Serializable]
    public class ActivationException : Exception
    {
        public ActivationException()
        {
        }

        public ActivationException(string message) : base(message)
        {
        }

        public ActivationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ActivationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}