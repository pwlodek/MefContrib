namespace MefContrib.Hosting.Isolation
{
    using System;
    using System.Linq;
    using MefContrib.Hosting.Isolation.Runtime;
    using MefContrib.Hosting.Isolation.Runtime.Activation;
    using MefContrib.Hosting.Isolation.Runtime.Activation.Hosts;
    using MefContrib.Hosting.Isolation.Runtime.Proxies;

    public static class PartHost
    {
        public static event EventHandler<ActivationHostEventArgs> Failure;

        internal static void OnFailure(IPartActivationHost host)
        {
            if (Failure != null)
            {
                Failure(host, new ActivationHostEventArgs(host.Description));
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

            var activatorHost = ActivationHost.CreateActivatorHost(isolationMetadata);
            var activator = activatorHost.GetActivator();

            var reference = activator.ActivateInstance(activatorHost.Description, assembly, typeName);

            RemotingServices.CloseActivator(activator);

            return ProxyFactory.GetFactory().CreateProxy(reference, contractType, additionalInterfaces);
        }

        public static void ReleaseInstance(object instance)
        {
            var aware = instance as IObjectReferenceAware;
            if (aware != null)
            {
                var reference = aware.Reference;
                var activator = ActivationHost.GetActivator(reference);
                activator.DeactivateInstance(reference);

                RemotingServices.CloseActivator(activator);
            }
        }
    }
}