namespace MefContrib.Hosting.Isolation
{
    using System;
    using System.ComponentModel.Composition.Primitives;
    using System.Threading;
    using MefContrib.Hosting.Isolation.Runtime;

    public class DisposableIsolatingComposablePart : IsolatingComposablePart, IDisposable
    {
        private int _isDisposed;

        public DisposableIsolatingComposablePart(ComposablePartDefinition definition) : base(definition)
        {
        }
        
        void IDisposable.Dispose()
        {
            if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
            {
                foreach (var disposableValue in ExportedValues)
                {
                    PartHost.ReleaseInstance(disposableValue);
                }
            }
        }
    }
}