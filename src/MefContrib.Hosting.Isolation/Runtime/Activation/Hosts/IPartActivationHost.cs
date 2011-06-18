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
        ActivationHostDescription Description { get; }

        void Start();

        void Stop();

        bool Started { get; }

        bool Faulted { get; }

        IRemoteActivator GetActivator();

        ISet<Type> ActivatedTypes { get; }
    }
}