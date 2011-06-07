using System.ComponentModel.Composition.Hosting;

namespace MefContrib.Hosting.Isolation.Runtime.Activation
{
    public abstract class ActivationHook
    {
        public bool IsInitialized { get; private set; }

        protected ExportProvider ExportProvider { get; private set; }

        protected AggregateCatalog AggregateCatalog { get; private set; }

        public void Initialize(ExportProvider exportProvider, AggregateCatalog aggregateCatalog)
        {
            ExportProvider = exportProvider;
            AggregateCatalog = aggregateCatalog;

            InitializeCore();

            IsInitialized = true;
        }

        protected abstract void InitializeCore();
    }
}