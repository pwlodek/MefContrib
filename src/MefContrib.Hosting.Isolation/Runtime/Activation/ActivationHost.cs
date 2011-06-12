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

        public const string DefaultGroup = "DefaultGroup";

        static ActivationHost()
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            Timer timer = new Timer(s =>
            {
                foreach (var partActivationHost in Hosts)
                {
                    if (!partActivationHost.Started) continue;

                    try
                    {
                        var activator = partActivationHost.GetActivator();
                        activator.HeartBeat();
                        RemotingServices.CloseActivator(activator);
                    }
                    catch (Exception exception)
                    {
                        if (!partActivationHost.Faulted)
                        {
                            var hostBase = (PartActivationHostBase) partActivationHost;
                            hostBase.Faulted = true;
                            try
                            {
                                PartHost.OnFaulted(hostBase, exception);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            });
            timer.Change(0, 1000);
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

        public static IPartActivationHost GetActivationHost(ObjectReference objectReference)
        {
            return Hosts.Single(t => t.Description == objectReference.Description);
        }

        public static IPartActivationHost GetActivationHost(ActivationHostDescription description)
        {
            return Hosts.Single(t => t.Description == description);
        }

        public static IPartActivationHost CreateActivationHost(Type implementationType, IIsolationMetadata isolationMetadata)
        {
            var groupName = isolationMetadata.IsolationGroup ?? DefaultGroup;
            var activationHost = isolationMetadata.HostPerInstance
                                 ? Hosts.FirstOrDefault(
                                     t =>
                                     !t.Faulted && t.Description.Isolation == isolationMetadata.Isolation &&
                                     t.Description.Group == groupName && !t.ActivatedTypes.Contains(implementationType))
                                 : Hosts.FirstOrDefault(
                                     t =>
                                     !t.Faulted && t.Description.Isolation == isolationMetadata.Isolation &&
                                     t.Description.Group == groupName);
            
            if (activationHost != null)
            {
                return activationHost;
            }

            var description = new ActivationHostDescription(isolationMetadata.Isolation, groupName);
            PartActivationHostBase host;

            switch (isolationMetadata.Isolation)
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

            // have the connectivity - started
            host.Started = true;

            return host;
        }

        private static void ThrowIfCannotConnect(IPartActivationHost host)
        {
            // test connection with the remote activator
            IRemoteActivator activator = null;
            var retryCount = 4;
            var success = false;
            var currentWaitTimeMilis = 100;

            while (!success && !host.Faulted && retryCount > 0)
            {
                try
                {
                    activator = host.GetActivator();
                    activator.HeartBeat();
                    success = true;
                }
                catch (Exception)
                {
                    Thread.Sleep(currentWaitTimeMilis);
                    currentWaitTimeMilis *= 2; // exponential backoff
                    success = false;
                    retryCount--;
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