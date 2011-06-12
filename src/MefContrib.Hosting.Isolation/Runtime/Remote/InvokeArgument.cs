using System;
using System.Runtime.Serialization;

namespace MefContrib.Hosting.Isolation.Runtime.Remote
{
    [DataContract]
    public class InvokeArgument
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Type Type { get; set; }

        [DataMember]
        public byte[] Value { get; set; }
    }
}