using System.ServiceModel;
using System.ServiceModel.Channels;

namespace MefContrib.Hosting.Isolation.Runtime
{
    public static class RemotingServices
    {
        public const string BaseAddress = "net.pipe://localhost/";

        public static readonly Binding Binding;

        static RemotingServices()
        {
            var binding = new NetNamedPipeBinding();
            binding.TransactionFlow = true;

            Binding = binding;
        }

        public static void CloseActivator(IRemoteActivator activator)
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

        public static IRemoteActivator CreateActivator(string address)
        {
            return ChannelFactory<IRemoteActivator>.CreateChannel(
                Binding, new EndpointAddress(address));
        }

        public static ServiceHost CreateServiceHost(string address)
        {
            var serviceHost = new ServiceHost(typeof(RemoteActivator));
            serviceHost.AddServiceEndpoint(typeof(IRemoteActivator), Binding, address);

            return serviceHost;
        }
    }
}