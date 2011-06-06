using System;
using System.Runtime.Serialization;

namespace MefContrib.Hosting.Isolation.Runtime
{
    [DataContract]
    public class RuntimeArgument
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Type Type { get; set; }

        [DataMember]
        public byte[] Value { get; set; }
    }
}