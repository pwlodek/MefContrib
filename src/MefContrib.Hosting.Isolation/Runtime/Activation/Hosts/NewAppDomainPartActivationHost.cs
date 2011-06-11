using System;
using System.Threading;

namespace MefContrib.Hosting.Isolation.Runtime.Activation.Hosts
{
    public class NewAppDomainPartActivationHost : PartActivationHostBase
    {
        private readonly AppDomain _domain;
        
        public NewAppDomainPartActivationHost(ActivationHostDescription description)
            : base(description)
        {
            _domain = AppDomain.CreateDomain("Plugin_" + description.Id);

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var senderDomain = (AppDomain) sender;
            if (senderDomain.Id == _domain.Id)
            {
                // if already faulted, do nothing
                if (!Faulted)
                {
                    Faulted = true;
                    PartHost.OnFaulted(this, e.ExceptionObject as Exception);
                }
            }
        }
        
        public override void Start()
        {
            var t = new Trampoline(Address);
            _domain.DoCallBack(t.StartCore);
        }

        public override void Stop()
        {
            AppDomain.Unload(_domain);
        }
        
        [Serializable]
        private class Trampoline
        {
            private string _address;
            private ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

            public Trampoline(string address)
            {
                _address = address;
                
            }

            public void StartCore()
            {
                var serviceHost = RemotingServices.CreateServiceHost(_address);
                serviceHost.Opened += serviceHost_Opened;
                serviceHost.Open();
                _manualResetEvent.WaitOne();
                AppDomain.CurrentDomain.DomainUnload += (s, e) => serviceHost.Close();
            }

            void serviceHost_Opened(object sender, EventArgs e)
            {
                _manualResetEvent.Set();
            }
        }
    }
}