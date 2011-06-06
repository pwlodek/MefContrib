using System;
using System.Runtime.Serialization;

namespace MefContrib.Hosting.Isolation.Runtime.Activation
{
    [Serializable]
    public class ActivationHostException : Exception
    {
        public ActivationHostException()
        {
        }

        public ActivationHostException(string message) : base(message)
        {
        }

        public ActivationHostException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ActivationHostException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}