using System;
using System.ServiceModel;

namespace MefContrib.Hosting.Isolation.Runtime.Activation.Hosts
{
    public class CurrentAppDomainPartActivationHost : IPartActivationHost
    {
        private readonly ServiceHost _serviceHost;
        private readonly string _address;

        public CurrentAppDomainPartActivationHost()
        {
            Id = Guid.NewGuid();

            _address = string.Concat(RemotingServices.BaseAddress, Id);
            _serviceHost = RemotingServices.CreateServiceHost(_address);
        }

        public Guid Id { get; private set; }
        
        public void Start()
        {
            if (_serviceHost.State != CommunicationState.Created)
            {
                throw new InvalidOperationException();
            }

            _serviceHost.Open();

            Console.WriteLine("Started in AppDomain: " + AppDomain.CurrentDomain.FriendlyName);
        }

        public void Stop()
        {
            _serviceHost.Close();
        }

        public IRemoteActivator GetActivator()
        {
            return RemotingServices.CreateActivator(_address);
        }
    }
}