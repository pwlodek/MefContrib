namespace MefContrib.Hosting.Isolation.Runtime
{
    using System;
    using System.Linq;
    using System.Reflection;
    using MefContrib.Hosting.Isolation.Runtime.Activation.Hosts;
    using MefContrib.Hosting.Isolation.Runtime.Proxies;
    using MefContrib.Hosting.Isolation.Runtime.Remote;

    /// <summary>
    /// Class used to activate parts in isolation, deactivate them, and to send messages to them.
    /// </summary>
    public static class PartHost
    {
        /// <summary>
        /// Creates new instance of a type in isolation.
        /// </summary>
        /// <typeparam name="TContract">Type of the contract that the part implements.</typeparam>
        /// <typeparam name="TImplementation">Part implementation type.</typeparam>
        /// <param name="isolationMetadata"><see cref="IIsolationMetadata"/> instance.</param>
        /// <returns>Proxy for smooth communication with remotly activated parts.</returns>
        public static TContract CreateInstance<TContract, TImplementation>(IIsolationMetadata isolationMetadata)
            where TImplementation : TContract
        {
            var contractType = typeof (TContract);
            var implementationType = typeof (TImplementation);

            return (TContract) CreateInstance(contractType, implementationType, isolationMetadata);
        }
        
        /// <summary>
        /// Creates new instance of a type in isolation.
        /// </summary>
        /// <param name="implementationType">Part implementation type.</param>
        /// <param name="isolationMetadata"><see cref="IIsolationMetadata"/> instance.</param>
        /// <returns>Proxy for smooth communication with remotly activated parts.</returns>
        public static object CreateInstance(Type implementationType, IIsolationMetadata isolationMetadata)
        {
            var interfaces = implementationType.GetInterfaces();
            var contractType = interfaces.First();

            return CreateInstance(contractType, implementationType, isolationMetadata);
        }

        /// <summary>
        /// Creates new instance of a type in isolation.
        /// </summary>
        /// <param name="contractType">Type of the contract that the part implements.</param>
        /// <param name="implementationType">Part implementation type.</param>
        /// <param name="isolationMetadata"><see cref="IIsolationMetadata"/> instance.</param>
        /// <returns>Proxy for smooth communication with remotly activated parts.</returns>
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

                RemotingServices.TryCloseActivator(activator);

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

        /// <summary>
        /// Releases given instance.
        /// </summary>
        /// <param name="instance">Instance to be deactivated.</param>
        public static void ReleaseInstance(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

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

                    RemotingServices.TryCloseActivator(activator);
                }
                catch (Exception exception)
                {
                    ActivationHost.MarkFaultedIfNeeded(
                        objectReference.Description,
                        exception,
                        objectReference);

                    throw new InvokeException(
                        string.Format("Unable to release object {0}.", objectReference), exception);
                }
            }
        }

        /// <summary>
        /// Invokes specific method on a object identified by the <see cref="ObjectReference"/> instance.
        /// </summary>
        /// <param name="objectReference">Reference to the object on which method will be invoked.</param>
        /// <param name="methodInfo">Identifies method to be invoked.</param>
        /// <param name="arguments">Arguments to be passed to the method being invoked.</param>
        /// <returns>Object returned by the method.</returns>
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

                RemotingServices.TryCloseActivator(remoteActivator);
            }
            catch (Exception exception)
            {
                ActivationHost.MarkFaultedIfNeeded(
                    objectReference.Description,
                    exception,
                    objectReference);

                throw new InvokeException(
                    string.Format("Unable to invoke method {0} on object {1}.", methodInfo.Name, objectReference), exception);
            }

            return SerializationServices.Deserialize(invokeReturnValue);
        }
    }
}