using System.ComponentModel.Composition.Hosting;
using NUnit.Framework;

namespace MefContrib.Hosting.Isolation.Tests
{
    [TestFixture]
    public class IsolatingCatalogTests
    {
        [Test]
        public void IsolatingCatalog_intercepts_parts_with_its_own_definitions()
        {
            var typeCatalog = new TypeCatalog(typeof(TempPart), typeof(FakePart1));
            var isolatingCatalog = new IsolatingCatalog(typeCatalog);
            var container = new CompositionContainer(isolatingCatalog);

            var part = container.GetExportedValue<TempPart>();
        }
    }
}