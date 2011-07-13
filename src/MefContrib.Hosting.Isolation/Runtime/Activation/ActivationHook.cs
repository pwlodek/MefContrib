namespace MefContrib.Hosting.Isolation.Runtime.Activation
{
    /// <summary>
    /// Represents an activation hook which gets executed once an assembly the hook is defined in is loaded.
    /// </summary>
    public abstract class ActivationHook
    {
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Performs the initialization.
        /// </summary>
        public void Initialize()
        {
            InitializeCore();

            IsInitialized = true;
        }

        protected abstract void InitializeCore();
    }
}