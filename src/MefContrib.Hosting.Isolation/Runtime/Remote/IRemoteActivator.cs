namespace MefContrib.Hosting.Isolation.Runtime.Remote
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using MefContrib.Hosting.Isolation.Runtime.Activation;

    /// <summary>
    /// Remote activator which is responsible for activating\deactivating instances as well as invoking methods on it.
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