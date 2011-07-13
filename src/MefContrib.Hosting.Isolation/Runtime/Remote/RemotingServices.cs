namespace MefContrib.Hosting.Isolation.Runtime.Remote
{
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    /// <summary>
    /// Helper class which eases WCF hosting and proxy creation.
    /// </summary>
    public static class RemotingServices
    {
        /// <summary>
        /// Base address.
        /// </summary>
        public const string BaseAddress = "net.pipe://localhost/";

        /// <summary>
        /// Default binding.
        /// </summary>
        public static readonly Binding Binding;

        static RemotingServices()
        {
            var binding = new NetNamedPipeBinding();
            binding.TransactionFlow = true;

            Binding = binding;
        }

        /// <summary>
        /// Closes given activator.
        /// </summary>
        /// <param name="activator">Activator to be closed.</param>
        public static void TryCloseActivator(IRemoteActivator activator)
        {
            if (activator == null)
            {
                return;
            }

            var communicationObject = (ICommunicationObject) activator;
            if (communicationObject.State != CommunicationState.Faulted)
            {
                communicationObject.Close();
            }
        }

        /// <summary>
        /// Creates proxy to the remote activator.
        /// </summary>
        /// <param name="address">Remote activator address.</param>
        /// <returns>Proxy to the remote activator.</returns>
        public static IRemoteActivator CreateActivator(string address)
        {
            return ChannelFactory<IRemoteActivator>.CreateChannel(
                Binding, new EndpointAddress(address));
        }

        /// <summary>
        /// Creates <see cref="ServiceHost"/> which hosts <see cref="RemoteActivator"/> service.
        /// </summary>
        /// <param name="address">Address of the remote activator.</param>
        /// <returns><see cref="ServiceHost"/> instance.</returns>
        public static ServiceHost CreateServiceHost(string address)
        {
            var serviceHost = new ServiceHost(typeof(RemoteActivator));
            serviceHost.AddServiceEndpoint(typeof(IRemoteActivator), Binding, address);

            return serviceHost;
        }
    }
}