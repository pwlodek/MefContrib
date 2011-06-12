using System.ComponentModel.Composition.Hosting;
using System.Linq;
using MefContrib.Hosting.Isolation.Runtime;
using NUnit.Framework;

namespace MefContrib.Hosting.Isolation.Tests
{
    [TestFixture]
    public class IsolatingCatalogTests
    {
        [Test]
        public void IsolatingCatalog_intercepts_non_disposable_part_with_its_own_definitions()
        {
            var typeCatalog = new TypeCatalog(typeof(FakePart1));
            var isolatingCatalog = new IsolatingCatalog(typeCatalog);
            var part = isolatingCatalog.Parts.Single();
            var partDef = part.CreatePart();
            var isolatingPartDef = partDef as IsolatingComposablePart;
            
            Assert.That(part, Is.Not.Null);
            Assert.That(partDef, Is.Not.Null);
            Assert.That(isolatingPartDef, Is.Not.Null);
        }

        [Test]
        public void IsolatingCatalog_intercepts_disposable_part_with_its_own_definitions()
        {
            var typeCatalog = new TypeCatalog(typeof(DisposableFakePart1));
            var isolatingCatalog = new IsolatingCatalog(typeCatalog);
            var part = isolatingCatalog.Parts.Single();
            var partDef = part.CreatePart();
            var isolatingPartDef = partDef as DisposableIsolatingComposablePart;

            Assert.That(part, Is.Not.Null);
            Assert.That(partDef, Is.Not.Null);
            Assert.That(isolatingPartDef, Is.Not.Null);
        }

        [Test]
        public void Remote_proxy_supports_IObjectReferenceAware_interface()
        {
            var typeCatalog = new TypeCatalog(typeof(TempPart), typeof(FakePart1));
            var isolatingCatalog = new IsolatingCatalog(typeCatalog);
            var container = new CompositionContainer(isolatingCatalog);

            var part = container.GetExportedValue<TempPart>();
            Assert.That(part, Is.Not.Null);
            Assert.That(part.Part, Is.Not.Null);

            var aware = part.Part as IObjectReferenceAware;
            Assert.That(aware, Is.Not.Null);
        }

        [Test]
        public void Dispose_is_called_on_a_disposable_part_activated_remotely()
        {
            var typeCatalog = new TypeCatalog(typeof(TempPart), typeof(DisposableFakePart1));
            var isolatingCatalog = new IsolatingCatalog(typeCatalog);
            var container = new CompositionContainer(isolatingCatalog);

            var part = container.GetExportedValue<TempPart>();
            Assert.That(part, Is.Not.Null);
            Assert.That(part.Part, Is.Not.Null);
            
            var aware = (IObjectReferenceAware) part.Part;
            Assert.That(aware.Reference.IsDisposable, Is.True);
            Assert.That(aware.Reference.IsDisposed, Is.False);
            Assert.That(DisposableFakePart1.IsDisposed, Is.False);

            // dispose the container should dispose remotely activated part
            container.Dispose();

            Assert.That(aware.Reference.IsDisposable, Is.True);
            Assert.That(aware.Reference.IsDisposed, Is.True);
            Assert.That(DisposableFakePart1.IsDisposed, Is.True);
        }
    }
}