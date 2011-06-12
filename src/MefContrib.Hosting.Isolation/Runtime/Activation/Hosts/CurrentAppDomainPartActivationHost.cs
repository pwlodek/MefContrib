namespace MefContrib.Hosting.Isolation.Runtime.Activation.Hosts
{
    using System;
    using System.ServiceModel;
    using MefContrib.Hosting.Isolation.Runtime.Remote;

    public class CurrentAppDomainPartActivationHost : PartActivationHostBase
    {
        private readonly ServiceHost _serviceHost;

        public CurrentAppDomainPartActivationHost(ActivationHostDescription description) 
            : base(description)
        {
            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
            _serviceHost = RemotingServices.CreateServiceHost(Address);
        }

        private void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ActivationHost.MarkFaulted(this, e.ExceptionObject as Exception);
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