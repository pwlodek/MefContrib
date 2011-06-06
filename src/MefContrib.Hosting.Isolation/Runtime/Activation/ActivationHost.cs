using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MefContrib.Hosting.Isolation.Runtime.Activation.Hosts;

namespace MefContrib.Hosting.Isolation.Runtime.Activation
{
    public static class ActivationHost
    {
        private static readonly List<IPartActivationHost> Hosts = new List<IPartActivationHost>();

        static ActivationHost()
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            foreach (var activatorHost in Hosts)
            {
                try
                {
                    activatorHost.Stop();
                }
                catch (Exception)
                {
                }
            }
        }
  
        public static IRemoteActivator GetActivator(ObjectReference objectReference)
        {
            return Hosts.Single(t => t.Description == objectReference.Description).GetActivator();
        }

        public static IPartActivationHost GetActivatorHost(ObjectReference objectReference)
        {
            return Hosts.Single(t => t.Description == objectReference.Description);
        }

        public static IPartActivationHost CreateActivatorHost(IsolationLevel isolationLevel, string groupName)
        {
            var activationHost = Hosts.FirstOrDefault(t => t.Description.Group == groupName);
            if (activationHost != null)
            {
                return activationHost;
            }

            var description = new ActivationHostDescription(groupName);
            IPartActivationHost host;

            switch (isolationLevel)
            {
                case IsolationLevel.None:
                    host = new CurrentAppDomainPartActivationHost(description);
                    break;

                case IsolationLevel.AppDomain:
                    host = new NewAppDomainPartActivationHost(description);
                    break;

                case IsolationLevel.Process:
                    host = new NewProcessPartActivationHost(description);
                    break;

                default:
                    throw new InvalidOperationException();
            }
            
            // Stash the reference to the host
            Hosts.Add(host);
            
            // Start activation host
            host.Start();

            // Wait till hosts starts up, if this is taking too much time, throw an exception
            ThrowIfCannotConnect(host);

            return host;
        }

        private static void ThrowIfCannotConnect(IPartActivationHost host)
        {
            // test connection with the remote activator
            var activator = host.GetActivator();
            var retryCount = 4;
            var success = false;
            var currentWaitTimeMilis = 100;

            while (!success && retryCount > 0)
            {
                try
                {
                    activator.HeartBeat();
                    success = true;
                }
                catch (Exception)
                {
                    Thread.Sleep(currentWaitTimeMilis);
                    currentWaitTimeMilis *= 2; // exponential backoff
                    success = false;
                    retryCount--;
                    activator = host.GetActivator();
                }
            }
            
            if (!success)
            {
                throw new ActivationHostException("Cannot start host.");
            }

            RemotingServices.CloseActivator(activator);
        }
    }
}