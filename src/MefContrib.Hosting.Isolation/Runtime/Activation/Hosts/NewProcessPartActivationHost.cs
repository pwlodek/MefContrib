namespace MefContrib.Hosting.Isolation.Runtime.Activation.Hosts
{
    using System.Diagnostics;

    public class NewProcessPartActivationHost : PartActivationHostBase
    {
        private readonly Process _process;

        public NewProcessPartActivationHost(ActivationHostDescription description) 
            : base(description)
        {
            _process = new Process();
        }
        
        public override void Start()
        {
            _process.StartInfo.Arguments = Address + " " + Process.GetCurrentProcess().Id;
            _process.StartInfo.CreateNoWindow = true;
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.FileName = "PluginContainer.exe";
            _process.Start();
        }

        public override void Stop()
        {
            _process.Kill();
        }
    }
}