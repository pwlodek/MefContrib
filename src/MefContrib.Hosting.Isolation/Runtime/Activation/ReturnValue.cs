using System.Runtime.Serialization;

namespace MefContrib.Hosting.Isolation.Runtime.Activation
{
    [DataContract]
    public class ReturnValue
    {
        [DataMember]
        public byte[] Value { get; set; }
    }
}