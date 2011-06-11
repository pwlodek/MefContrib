using System.Runtime.Serialization;

namespace MefContrib.Hosting.Isolation.Runtime
{
    [DataContract]
    public class InvokeReturnValue
    {
        [DataMember]
        public byte[] Value { get; set; }
    }
}