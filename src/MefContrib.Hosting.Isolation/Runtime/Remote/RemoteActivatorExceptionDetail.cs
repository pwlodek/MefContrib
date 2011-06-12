using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using MefContrib.Hosting.Isolation.Runtime.Activation;

namespace MefContrib.Hosting.Isolation.Runtime.Remote
{
    [DataContract]
    public class RemoteActivatorExceptionDetail : ExceptionDetail
    {
        public RemoteActivatorExceptionDetail(Exception exception) : base(exception)
        {
        }

        [DataMember]
        public ActivationHostDescription Description { get; set; }
    }
}