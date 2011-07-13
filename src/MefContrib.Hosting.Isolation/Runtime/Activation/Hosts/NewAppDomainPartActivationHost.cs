namespace MefContrib.Hosting.Isolation.Runtime.Activation.Hosts
{
    using System;
    using MefContrib.Hosting.Isolation.Runtime.Remote;

    /// <summary>
    /// Activation host which activates parts in a separate app domain.
    /// </summary>
    public class NewAppDomainPartActivationHost : PartActivationHostBase
    {
        private readonly AppDomain _domain;
        
        public NewAppDomainPartActivationHost(ActivationHostDescription description)
            : base(description)
        {
            _domain = AppDomain.CreateDomain("Plugin_" + description.Id);

            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
        }

        private void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var senderDomain = (AppDomain) sender;
            if (senderDomain.Id == _domain.Id)
            {
                ActivationHost.MarkFaulted(this, e.ExceptionObject as Exception);
            }
        }

        /// <summary>
        /// Starts the host.
        /// </summary>
        public override void Start()
        {
            var t = new Trampoline(Address);
            _domain.DoCallBack(t.StartCore);
        }

        /// <summary>
        /// Stops the host.
        /// </summary>
        public override void Stop()
        {
            AppDomain.Unload(_domain);
        }
        
        [Serializable]
        private class Trampoline
        {
            private string _address;

            public Trampoline(string address)
            {
                _address = address;
                
            }

            public void StartCore()
            {
                var serviceHost = RemotingServices.CreateServiceHost(_address);
                serviceHost.Open();

                AppDomain.CurrentDomain.DomainUnload += (s, e) => serviceHost.Close();
            }
        }
    }
}