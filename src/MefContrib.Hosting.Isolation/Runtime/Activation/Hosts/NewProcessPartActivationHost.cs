namespace MefContrib.Hosting.Isolation.Runtime.Activation.Hosts
{
    using System.Diagnostics;

    /// <summary>
    /// Activation host which activates parts in a separate process.
    /// </summary>
    public class NewProcessPartActivationHost : PartActivationHostBase
    {
        private readonly Process _process;

        public NewProcessPartActivationHost(ActivationHostDescription description) 
            : base(description)
        {
            _process = new Process();
        }

        /// <summary>
        /// Starts the host.
        /// </summary>
        public override void Start()
        {
            _process.StartInfo.Arguments = Address + " " + Process.GetCurrentProcess().Id;
            _process.StartInfo.CreateNoWindow = true;
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.FileName = "PluginContainer.exe";
            _process.Start();
        }

        /// <summary>
        /// Stops the host.
        /// </summary>
        public override void Stop()
        {
            _process.Kill();
        }
    }
}