namespace MefContrib.Hosting.Isolation.Runtime
{
    using System;
    using System.Linq;
    using System.Reflection;
    using MefContrib.Hosting.Isolation.Runtime.Activation;
    using MefContrib.Hosting.Isolation.Runtime.Activation.Hosts;
    using MefContrib.Hosting.Isolation.Runtime.Proxies;

    public static class PartHost
    {
        public static event EventHandler<ActivationHostEventArgs> Faulted;

        internal static void OnFaulted(IPartActivationHost host, Exception exception)
        {
            if (Faulted != null)
            {
                Faulted(host, new ActivationHostEventArgs(host.Description, exception));
            }
        }

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

            IPartActivationHost activatorHost;

            try
            {
                activatorHost = ActivationHost.CreateActivationHost(implementationType, isolationMetadata);
            }
            catch (Exception)
            {
                throw new Exception("Cannot activate host.");
            }

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
                HandleHostException(exception, activatorHost.Description);
            }

            throw new InvalidOperationException("Should never happen.");
        }

        public static void ReleaseInstance(object instance)
        {
            var aware = instance as IObjectReferenceAware;
            if (aware != null)
            {
                var reference = aware.Reference;
                var activator = ActivationHost.GetActivator(reference);
                activator.DeactivateInstance(reference);
                reference.IsDisposed = true;

                RemotingServices.CloseActivator(activator);
            }
        }

        public static object InvokeMember(ObjectReference objectReference, MethodInfo methodInfo, object[] arguments)
        {
            if (objectReference == null)
            {
                throw new ArgumentNullException("objectReference");
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

            if (methodInfo.DeclaringType == typeof(IObjectReferenceAware))
            {
                return objectReference;
            }

            IRemoteActivator remoteActivator = ActivationHost.GetActivator(objectReference);
            InvokeReturnValue invokeReturnValue = null;
            Exception exception = null;
            try
            {
                invokeReturnValue = remoteActivator.InvokeMember(
                    objectReference,
                    methodInfo.Name,
                    SerializationServices.Serialize(arguments));

                RemotingServices.CloseActivator(remoteActivator);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            HandleHostException(exception, objectReference.Description, objectReference);

            return SerializationServices.Deserialize(invokeReturnValue);
        }

        internal static void HandleHostException(Exception exception, ActivationHostDescription description, ObjectReference reference = null)
        {
            if (exception != null)
            {
                // first check if we have the connectivity with the activator host
                var remoteActivator = ActivationHost.GetActivationHost(description).GetActivator();

                try
                {
                    remoteActivator.HeartBeat();
                    RemotingServices.CloseActivator(remoteActivator);
                }
                catch (Exception)
                {
                    // mark object as faulted
                    if (reference != null)
                    {
                        reference.Faulted = true;
                    }

                    var host = ActivationHost.GetActivationHost(description);
                    var activationHostBase = host as PartActivationHostBase;
                    if (activationHostBase != null)
                    {
                        // we have a serious problem - plugin host has crashed
                        if (activationHostBase.Faulted == false)
                        {
                            activationHostBase.Faulted = true;
                            PartHost.OnFaulted(host, exception);
                        }
                    }
                    else
                    {
                        PartHost.OnFaulted(host, exception);
                    }
                }

                throw new InvokeException("Error while executing remote method.", exception);
            }
        }
    }
}