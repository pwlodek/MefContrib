using System;

namespace MefContrib.Hosting.Isolation.Runtime.Activation
{
    public class ActivationHostEventArgs : EventArgs
    {
        public ActivationHostDescription Description { get; private set; }

        public ActivationHostEventArgs(ActivationHostDescription description)
        {
            if (description == null)
            {
                throw new ArgumentNullException("description");
            }

            Description = description;
        }
    }
}