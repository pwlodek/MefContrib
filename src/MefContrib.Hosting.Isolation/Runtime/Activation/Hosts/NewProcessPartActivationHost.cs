using System;

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
            _process.Exited += new System.EventHandler(_process_Exited);
            _process.Start();
        }

        void _process_Exited(object sender, System.EventArgs e)
        {
            Console.WriteLine("Exited with code: " + _process.ExitCode);
        }

        public override void Stop()
        {
            _process.Kill();
        }
    }
}