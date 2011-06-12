namespace MefContrib.Hosting.Isolation.Runtime
{
    using System;
    using System.Linq;
    using System.Reflection;
    using MefContrib.Hosting.Isolation.Runtime.Activation;
    using MefContrib.Hosting.Isolation.Runtime.Activation.Hosts;
    using MefContrib.Hosting.Isolation.Runtime.Proxies;
    using MefContrib.Hosting.Isolation.Runtime.Remote;

    public static class PartHost
    {
        public static TContract CreateInstance<TContract, TImplementation>(IIsolationMetadata isolationMetadata)
            where TImplementation : TContract
        {
            var contractType = typeof (TContract);
            var implementationType = typeof (TImplementation);

            return (TContract) CreateInstance(contractType, implementationType, isolationMetadata);
        }
        
        public static object CreateInstance(Type implementationType, IIsolationMetadata isolationMetadata)
        {
            var interfaces = implementationType.GetInterfaces();
            var contractType = interfaces.First();

            return CreateInstance(contractType, implementationType, isolationMetadata);
        }

        public static object CreateInstance(Type contractType, Type implementationType, IIsolationMetadata isolationMetadata)
        {
            var assembly = implementationType.Assembly.FullName;
            var typeName = implementationType.FullName;
            var interfaces = implementationType.GetInterfaces();
            var additionalInterfaces = interfaces.Where(t => t != contractType).ToArray();

            IPartActivationHost activatorHost = ActivationHost.CreateActivationHost(implementationType, isolationMetadata);
            
            try
            {
                var partActivationHostBase = (PartActivationHostBase) activatorHost;
                partActivationHostBase.ActivatedTypes.Add(implementationType);
                var activator = activatorHost.GetActivator();
                var reference = activator.ActivateInstance(activatorHost.Description, assembly, typeName);

                RemotingServices.CloseActivator(activator);

                return ProxyFactory.GetFactory().CreateProxy(reference, contractType, additionalInterfaces);
            }
            catch (Exception exception)
            {
                ActivationHost.MarkFaultedIfNeeded(activatorHost.Description, exception);

                throw new ActivationException(
                    string.Format("Unable to activate instance of {0}.", implementationType.FullName),
                    exception);
            }
        }

        public static void ReleaseInstance(object instance)
        {
            var aware = instance as IObjectReferenceAware;
            if (aware != null)
            {
                var objectReference = aware.Reference;

                try
                {
                    var activationHost = ActivationHost.GetActivationHost(objectReference);
                    var activator = activationHost.GetActivator();
                    activator.DeactivateInstance(objectReference);
                    objectReference.IsDisposed = true;

                    RemotingServices.CloseActivator(activator);
                }
                catch (Exception exception)
                {
                    ActivationHost.MarkFaultedIfNeeded(
                        objectReference.Description,
                        exception,
                        objectReference);

                    throw new InvokeException(
                        string.Format("Unable to release object {0}.", objectReference));
                }
            }
        }

        public static object InvokeMember(ObjectReference objectReference, MethodInfo methodInfo, object[] arguments)
        {
            if (objectReference == null)
            {
                throw new ArgumentNullException("objectReference");
            }

            if (methodInfo.DeclaringType == typeof(IObjectReferenceAware))
            {
                return objectReference;
            }

            if (objectReference.Faulted)
            {
                throw new InvokeException(
                    string.Format("Object [{0}] is faulted.", objectReference));
            }

            if (objectReference.IsDisposed)
            {
                throw new ObjectDisposedException(
                    string.Format("Object [{0}] has been disposed.", objectReference));
            }

            IPartActivationHost activationHost = ActivationHost.GetActivationHost(objectReference);
            IRemoteActivator remoteActivator = activationHost.GetActivator();
            InvokeReturnValue invokeReturnValue;
            try
            {
                invokeReturnValue = remoteActivator.InvokeMember(
                    objectReference,
                    methodInfo.Name,
                    SerializationServices.Serialize(arguments));

                RemotingServices.CloseActivator(remoteActivator);
            }
            catch (Exception exception)
            {
                ActivationHost.MarkFaultedIfNeeded(
                    objectReference.Description,
                    exception,
                    objectReference);

                throw new InvokeException(
                    string.Format("Unable to invoke method {0} on object {1}.", methodInfo.Name, objectReference));
            }

            return SerializationServices.Deserialize(invokeReturnValue);
        }
    }
}