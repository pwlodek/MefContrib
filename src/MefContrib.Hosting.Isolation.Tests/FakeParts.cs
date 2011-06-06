using System.ComponentModel.Composition;

namespace MefContrib.Hosting.Isolation.Tests
{
    [Export]
    public class TempPart
    {
        [Import]
        public IFakePart Part { get; set; }
    }

    [IsolationExport(typeof(IFakePart), Isolation = IsolationLevel.None)]
    public class FakePart1 : IFakePart
    {
        
    }

    public interface IFakePart { }
}