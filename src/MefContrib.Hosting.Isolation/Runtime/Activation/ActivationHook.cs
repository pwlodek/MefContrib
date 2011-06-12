namespace MefContrib.Hosting.Isolation.Runtime.Activation
{
    public abstract class ActivationHook
    {
        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            InitializeCore();

            IsInitialized = true;
        }

        protected abstract void InitializeCore();
    }
}