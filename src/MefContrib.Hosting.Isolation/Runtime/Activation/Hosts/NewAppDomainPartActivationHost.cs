using System;

namespace MefContrib.Hosting.Isolation.Runtime.Activation.Hosts
{
    public class NewAppDomainPartActivationHost : IPartActivationHost
    {
        private readonly string _address;
        private readonly AppDomain _domain;

        public NewAppDomainPartActivationHost()
        {
            Id = Guid.NewGuid();

            _address = string.Concat(RemotingServices.BaseAddress, Id);
            _domain = AppDomain.CreateDomain("Plugin_" + Id);
        }

        public Guid Id { get; private set; }
        
        public void Start()
        {
            Trampoline t = new Trampoline(_address);
            _domain.DoCallBack(t.StartCore);
            
        }

        public void Stop()
        {
            AppDomain.Unload(_domain);
        }

        public IRemoteActivator GetActivator()
        {
            return RemotingServices.CreateActivator(_address);
        }

        [Serializable]
        public class Trampoline
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

                Console.WriteLine("Started in AppDomain: " + AppDomain.CurrentDomain.FriendlyName);
            }
        }
    }
}