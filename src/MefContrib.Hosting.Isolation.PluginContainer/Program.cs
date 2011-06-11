using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MefContrib.Hosting.Isolation.Runtime;

namespace MefContrib.Hosting.Isolation.PluginContainer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2) return;

            var address = args[0];
            var parentId = int.Parse(args[1]);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            var parentProcess = Process.GetProcessById(parentId);
            parentProcess.Exited += OnParentProcessExited;
            var serviceHost = RemotingServices.CreateServiceHost(address);
            serviceHost.Open();
            
            Task task = new Task(() =>
            {
                while (true)
                {
                    var processes = Process.GetProcesses();
                    var parent = processes.FirstOrDefault(t => t.Id == parentId);
                    if (parent == null)
                    {
                        Environment.Exit(0);
                    }

                    Thread.Sleep(2000);
                }
            });

            task.Start();
            Console.ReadKey();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Environment.Exit(1);
        }

        private static void OnParentProcessExited(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
