namespace MefContrib.Hosting.Isolation.Runtime.Activation.Hosts
{
    using System;
    using System.ServiceModel;
    using MefContrib.Hosting.Isolation.Runtime.Remote;

    /// <summary>
    /// Activation host which activates parts in the current app domain.
    /// </summary>
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

        /// <summary>
        /// Starts the host.
        /// </summary>
        public override void Start()
        {
            if (_serviceHost.State != CommunicationState.Created)
            {
                throw new InvalidOperationException();
            }

            _serviceHost.Open();
        }

        /// <summary>
        /// Stops the host.
        /// </summary>
        public override void Stop()
        {
            _serviceHost.Close();
        }
    }
}