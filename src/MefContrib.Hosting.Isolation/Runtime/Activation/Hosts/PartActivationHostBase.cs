namespace MefContrib.Hosting.Isolation.Runtime.Activation.Hosts
{
    using System;
    using System.Collections.Generic;
    using MefContrib.Hosting.Isolation.Runtime.Remote;

    /// <summary>
    /// Represents a base class for all part activation hosts.
    /// </summary>
    public abstract class PartActivationHostBase : IPartActivationHost
    {
        protected PartActivationHostBase(ActivationHostDescription description)
        {
            if (description == null)
            {
                throw new ArgumentNullException("description");
            }

            Description = description;
            ActivatedTypes = new HashSet<Type>();
            Address = string.Concat(RemotingServices.BaseAddress, description.Id);
        }

        /// <summary>
        /// Gets the address of this host.
        /// </summary>
        public string Address { get; private set; }

        /// <summary>
        /// Gets the host description.
        /// </summary>
        public ActivationHostDescription Description { get; private set; }

        /// <summary>
        /// Starts the host.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Stops the host.
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Indicates if the host is started.
        /// </summary>
        public bool Started { get; internal set; }

        /// <summary>
        /// Indicates if the host is faulted.
        /// </summary>
        public bool Faulted { get; internal set; }

        /// <summary>
        /// Gets a set of types activated in this host.
        /// </summary>
        public ISet<Type> ActivatedTypes { get; private set; }

        /// <summary>
        /// Gets a proxy to the remote activator service hosted by this part activation host.
        /// </summary>
        /// <returns></returns>
        public IRemoteActivator GetActivator()
        {
            if (Faulted)
            {
                throw new ActivationHostException("Host you try to connect to is faulted.", Description);
            }

            try
            {
                var activator = RemotingServices.CreateActivator(Address);
                return activator;
            }
            catch (Exception exception)
            {
                Faulted = true;
                throw new ActivationHostException("Cannot create activator.", exception, Description);
            }
        }
    }
}