using System;

namespace MefContrib.Hosting.Isolation.Runtime.Activation.Hosts
{
    public interface IPartActivationHost
    {
        Guid Id { get; }

        void Start();

        void Stop();

        IRemoteActivator GetActivator();
    }
}