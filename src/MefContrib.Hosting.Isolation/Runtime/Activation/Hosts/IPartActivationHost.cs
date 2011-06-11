using System;

namespace MefContrib.Hosting.Isolation.Runtime.Activation.Hosts
{
    public interface IPartActivationHost
    {
        ActivationHostDescription Description { get; }

        void Start();

        void Stop();

        bool Started { get; }

        bool Faulted { get; }

        IRemoteActivator GetActivator();
    }
}