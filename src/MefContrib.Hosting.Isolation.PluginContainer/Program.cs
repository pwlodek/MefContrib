using System;
using System.Diagnostics;
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
