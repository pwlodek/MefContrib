using System;

namespace MefContrib.Hosting.Isolation.Runtime.Activation.Hosts
{
    public abstract class PartActivationHostBase : IPartActivationHost
    {
        protected PartActivationHostBase(ActivationHostDescription description)
        {
            if (description == null)
            {
                throw new ArgumentNullException("description");
            }

            Description = description;

            Address = string.Concat(RemotingServices.BaseAddress, description.Id);
        }

        public string Address { get; private set; }

        public ActivationHostDescription Description { get; private set; }

        public abstract void Start();

        public abstract void Stop();

        public IRemoteActivator GetActivator()
        {
            return RemotingServices.CreateActivator(Address);
        }
    }
}