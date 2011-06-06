using System;
using System.ServiceModel;

namespace MefContrib.Hosting.Isolation.Runtime.Activation.Hosts
{
    public class CurrentAppDomainPartActivationHost : PartActivationHostBase
    {
        private readonly ServiceHost _serviceHost;

        public CurrentAppDomainPartActivationHost(ActivationHostDescription description) 
            : base(description)
        {
            _serviceHost = RemotingServices.CreateServiceHost(Address);
        }
        
        public override void Start()
        {
            if (_serviceHost.State != CommunicationState.Created)
            {
                throw new InvalidOperationException();
            }

            _serviceHost.Open();
        }

        public override void Stop()
        {
            _serviceHost.Close();
        }
    }
}