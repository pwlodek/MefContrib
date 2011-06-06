using System;

namespace MefContrib.Hosting.Isolation.Runtime.Activation
{
    public class ActivationHostEventArgs : EventArgs
    {
        public Guid ActivatorHostId { get; private set; }

        public ActivationHostEventArgs(Guid activatorHostId)
        {
            ActivatorHostId = activatorHostId;
        }
    }
}