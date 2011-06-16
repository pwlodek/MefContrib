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
        public void Can_instantiate_FakePart1()
        {
            var typeCatalog = new TypeCatalog(typeof(TempPart), typeof(FakePart1));
            var isolatingCatalog = new IsolatingCatalog(typeCatalog);
            var container = new CompositionContainer(isolatingCatalog);

            var part = container.GetExportedValue<TempPart>();
            Assert.That(part, Is.Not.Null);
            Assert.That(part.Part, Is.Not.Null);
        }

        [Test]
        public void Remote_proxy_supports_IObjectReferenceAware_interface()
        {
            var typeCatalog = new TypeCatalog(typeof(TempPart), typeof(FakePart1));
            var isolatingCatalog = new IsolatingCatalog(typeCatalog);
            var container = new CompositionContainer(isolatingCatalog);

            var part = container.GetExportedValue<TempPart>();
            Assert.That(part, Is.Not.Null);

            var aware = part.Part as IObjectReferenceAware;
            Assert.That(aware, Is.Not.Null);
        }

        [Test]
        public void Foo_accepting_zero_arguments_and_returning_null_is_called_on_a_part_activated_remotely()
        {
            var typeCatalog = new TypeCatalog(typeof(TempPart), typeof(FakePart1));
            var isolatingCatalog = new IsolatingCatalog(typeCatalog);
            var container = new CompositionContainer(isolatingCatalog);

            var part = container.GetExportedValue<TempPart>();
            Assert.That(part, Is.Not.Null);
            Assert.That(part.Part, Is.Not.Null);

            Assert.That(FakePart1.FooHasBeenCalled, Is.False);
            part.Part.Foo();
            Assert.That(FakePart1.FooHasBeenCalled, Is.True);
        }

        [Test]
        public void Foo_accepting_three_arguments_of_value_type_and_returning_string_is_called_on_a_part_activated_remotely()
        {
            var typeCatalog = new TypeCatalog(typeof(FakePartSupportingValueTypes));
            var isolatingCatalog = new IsolatingCatalog(typeCatalog);
            var container = new CompositionContainer(isolatingCatalog);

            var part = container.GetExportedValue<IFakePartSupportingValueTypes>();
            Assert.That(part, Is.Not.Null);
            
            var retVal = part.Foo("some text", 123, 456.789);
            Assert.That(retVal, Is.EqualTo("some text 123 456.789"));
        }

        [Test]
        public void Foo_accepting_serializable_type_and_returning_serializable_type_is_called_on_a_part_activated_remotely()
        {
            var typeCatalog = new TypeCatalog(typeof(FakePartSupportingSerializableTypes));
            var isolatingCatalog = new IsolatingCatalog(typeCatalog);
            var container = new CompositionContainer(isolatingCatalog);

            var part = container.GetExportedValue<IFakePartSupportingSerializableTypes>();
            Assert.That(part, Is.Not.Null);

            var retVal = part.Foo(new Something { Name = "asd 123" });
            Assert.That(retVal, Is.Not.Null);
            Assert.That(retVal.Name, Is.EqualTo("asd 123"));
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