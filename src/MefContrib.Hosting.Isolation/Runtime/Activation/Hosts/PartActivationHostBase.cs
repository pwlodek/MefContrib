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

        public bool Faulted { get; internal set; }

        public IRemoteActivator GetActivator()
        {
            if (Faulted)
            {
                throw new ActivationHostException("This host is faulted.");
            }

            try
            {
                var activator = RemotingServices.CreateActivator(Address);
                return activator;
            }
            catch (Exception exception)
            {
                Faulted = true;
                throw new ActivationHostException("Cannot create activator.", exception);
            }
        }
    }
}