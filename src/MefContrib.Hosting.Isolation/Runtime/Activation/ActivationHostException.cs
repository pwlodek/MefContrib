namespace MefContrib.Hosting.Isolation.Runtime.Activation
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class ActivationHostException : Exception
    {
        public ActivationHostException()
        {
        }

        public ActivationHostException(string message, ActivationHostDescription description) : base(message)
        {
            Description = description;
        }

        public ActivationHostException(string message, Exception inner, ActivationHostDescription description) : base(message, inner)
        {
            Description = description;
        }

        public ActivationHostDescription Description { get; private set; }

        protected ActivationHostException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}