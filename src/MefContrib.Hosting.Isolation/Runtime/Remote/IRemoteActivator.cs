using System;
using System.Collections.Generic;
using System.ServiceModel;
using MefContrib.Hosting.Isolation.Runtime.Activation;

namespace MefContrib.Hosting.Isolation.Runtime.Remote
{
    /// <summary>
    /// remote activator
    /// </summary>
    [ServiceContract]
    public interface IRemoteActivator
    {
        [OperationContract]
        void HeartBeat();

        [OperationContract]
        [FaultContract(typeof(RemoteActivatorExceptionDetail))]
        ObjectReference ActivateInstance(ActivationHostDescription description, string assembly, string type);

        [OperationContract]
        [FaultContract(typeof(RemoteActivatorExceptionDetail))]
        InvokeReturnValue InvokeMember(ObjectReference objectReference, string name, List<InvokeArgument> arguments);

        [OperationContract]
        [FaultContract(typeof(RemoteActivatorExceptionDetail))]
        void DeactivateInstance(ObjectReference objectReference);
    }
}