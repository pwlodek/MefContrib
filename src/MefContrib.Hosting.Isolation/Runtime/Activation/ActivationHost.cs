using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MefContrib.Hosting.Isolation.Runtime.Activation.Hosts;

namespace MefContrib.Hosting.Isolation.Runtime.Activation
{
    public static class ActivationHost
    {
        private static readonly List<IPartActivationHost> Activators = new List<IPartActivationHost>();

        static ActivationHost()
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            foreach (var activatorHost in Activators)
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
            return Activators.Single(t => t.Id == objectReference.ActivatorHostId).GetActivator();
        }

        public static IPartActivationHost GetActivatorHost(ObjectReference objectReference)
        {
            return Activators.Single(t => t.Id == objectReference.ActivatorHostId);
        }

        public static IPartActivationHost CreateActivatorHost(IsolationLevel isolationLevel)
        {
            IPartActivationHost host;

            switch (isolationLevel)
            {
                case IsolationLevel.None:
                    host = new CurrentAppDomainPartActivationHost();
                    break;

                case IsolationLevel.AppDomain:
                    host = new NewAppDomainPartActivationHost();
                    break;

                case IsolationLevel.Process:
                    host = new NewProcessPartActivationHost();
                    break;

                default:
                    throw new InvalidOperationException();
            }
            
            Activators.Add(host);
            
            // start activator host
            host.Start();

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
                    currentWaitTimeMilis *= 2;
                    success = false;
                    retryCount--;
                    //RemotingServices.CloseActivator(activator);
                    activator = host.GetActivator();
                }
            }
            
            if (!success)
            {
                throw new ActivationException("Cannot start host.");
            }

            RemotingServices.CloseActivator(activator);
        }
    }
}