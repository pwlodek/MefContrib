namespace MefContrib.Hosting.Isolation.Runtime.Activation.Hosts
{
    using System;
    using System.Collections.Generic;
    using MefContrib.Hosting.Isolation.Runtime.Remote;

    /// <summary>
    /// Represents a part activation host. Host is responsible for providing part isolation.
    /// </summary>
    public interface IPartActivationHost
    {
        /// <summary>
        /// Gets the host description.
        /// </summary>
        ActivationHostDescription Description { get; }

        /// <summary>
        /// Starts the host.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the host.
        /// </summary>
        void Stop();

        /// <summary>
        /// Indicates if the host is started.
        /// </summary>
        bool Started { get; }

        /// <summary>
        /// Indicates if the host is faulted.
        /// </summary>
        bool Faulted { get; }

        /// <summary>
        /// Gets a proxy to the remote activator service hosted by this part activation host.
        /// </summary>
        /// <returns></returns>
        IRemoteActivator GetActivator();

        /// <summary>
        /// Gets a set of types activated in this host.
        /// </summary>
        ISet<Type> ActivatedTypes { get; }
    }
}