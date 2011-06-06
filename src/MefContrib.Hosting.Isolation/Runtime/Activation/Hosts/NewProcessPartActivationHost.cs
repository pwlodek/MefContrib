using System;
using System.Diagnostics;

namespace MefContrib.Hosting.Isolation.Runtime.Activation.Hosts
{
    public class NewProcessPartActivationHost : IPartActivationHost
    {
        private readonly string _address;
        private readonly Process _process;

        public NewProcessPartActivationHost()
        {
            Id = Guid.NewGuid();

            _address = string.Concat(RemotingServices.BaseAddress, Id);
            _process = new Process();
        }

        public Guid Id { get; private set; }
        
        public void Start()
        {
            _process.StartInfo.Arguments = _address + " " + Process.GetCurrentProcess().Id;
            _process.StartInfo.CreateNoWindow = true;
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.FileName = "PluginContainer.exe";
            _process.Start();
        }

        public void Stop()
        {
            _process.Kill();
        }

        public IRemoteActivator GetActivator()
        {
            return RemotingServices.CreateActivator(_address);
        }
    }
}