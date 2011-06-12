namespace MefContrib.Hosting.Isolation.Runtime.Activation.Hosts
{
    using System;
    using System.Collections.Generic;
    using MefContrib.Hosting.Isolation.Runtime.Remote;

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

        public string Address { get; private set; }

        public ActivationHostDescription Description { get; private set; }

        public abstract void Start();

        public abstract void Stop();

        public bool Started { get; internal set; }

        public bool Faulted { get; internal set; }

        public ISet<Type> ActivatedTypes { get; private set; }

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