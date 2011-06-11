using System;
using System.ComponentModel.Composition;

namespace MefContrib.Hosting.Isolation.Tests
{
    [Export]
    public class TempPart
    {
        [Import]
        public IFakePart Part { get; set; }
    }

    [IsolatedExport(typeof(IFakePart), Isolation = IsolationLevel.None)]
    public class FakePart1 : IFakePart
    {
        
    }

    [Export(typeof(IFakePart))]
    [Isolated(Isolation = IsolationLevel.None)]
    public class DisposableFakePart1 : IFakePart, IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    public interface IFakePart { }
}