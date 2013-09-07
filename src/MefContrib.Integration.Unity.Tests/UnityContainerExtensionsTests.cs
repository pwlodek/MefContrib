using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using MefContrib.Integration.Unity.Extensions;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace MefContrib.Integration.Unity.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;

    [TestFixture]
    public class UnityContainerExtensionsTests
    {
        public interface IMefExposedComponent
        {
        }

        [Export(typeof(IMefExposedComponent))]
        public class MefComponent : IMefExposedComponent
        {
        }

        public interface IUnityComponent
        {
        }

        public class UnityComponent : IUnityComponent
        {
        }

        public interface IMefDependingOnUnity
        {
            IUnityComponent UnityComponent { get; }
        }

        public interface IMefDependingOnUnity2
        {
            IUnityComponent UnityComponent { get; }
        }

        [Export(typeof(IMefDependingOnUnity))]
        public class MefComponentDependingOnUnity : IMefDependingOnUnity
        {
            private readonly IUnityComponent m_UnityComponent;

            [ImportingConstructor]
            public MefComponentDependingOnUnity(IUnityComponent unityComponent)
            {
                m_UnityComponent = unityComponent;
            }

            public IUnityComponent UnityComponent { get { return m_UnityComponent; } }
        }

        [Export(typeof(IMefDependingOnUnity2))]
        public class MefComponentDependingOnUnityByProperty : IMefDependingOnUnity2
        {
            [Import]
            public IUnityComponent UnityComponent { get; set; }
        }

        private static UnityContainer ConfigureUnityThenMef()
        {
            var container = new UnityContainer();
            TypeRegistrationTrackerExtension.RegisterIfMissing(container);

            var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            container.RegisterType<IUnityComponent, UnityComponent>();
            container.RegisterType<ICountableUnityComponent, CountableUnityComponent>();
            container.RegisterCatalog(catalog);

            return container;
        }

        private static UnityContainer ConfigureMefThenUnity()
        {
            var container = new UnityContainer();
            var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            container.RegisterCatalog(catalog);
            container.RegisterType<IUnityComponent, UnityComponent>();
            container.RegisterType<ICountableUnityComponent, CountableUnityComponent>();

            return container;
        }

        [Test]
        public void ResolveUnityFromUnityTest()
        {
            var container = ConfigureUnityThenMef();

            var component = container.Resolve<IUnityComponent>();
            Assert.IsNotNull(component);
        }

        [Test]
        public void ResolveUnityFromUnity2Test()
        {
            var container = ConfigureMefThenUnity();

            var component = container.Resolve<IUnityComponent>();
            Assert.IsNotNull(component);
        }

        [Test]
        public void ResolveMefFromUnityTest()
        {
            var container = ConfigureUnityThenMef();

            var component = container.Resolve<IMefExposedComponent>();
            Assert.IsNotNull(component);
        }
        [Test]
        public void ResolveMefFromUnity2Test()
        {
            var container = ConfigureMefThenUnity();

            var component = container.Resolve<IMefExposedComponent>();
            Assert.IsNotNull(component);
        }

        [Test]
        public void ResolveUnityFromMefByCtorTest()
        {
            var container = ConfigureUnityThenMef();

            var unityComponent = container.Resolve<IUnityComponent>();

            var component = container.Resolve<IMefDependingOnUnity>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.UnityComponent);
            Assert.AreNotEqual(unityComponent, component.UnityComponent);
        }

        [Test]
        public void ResolveUnityFromMefByCtor2Test()
        {
            var container = ConfigureMefThenUnity();

            var unityComponent = container.Resolve<IUnityComponent>();

            var component = container.Resolve<IMefDependingOnUnity>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.UnityComponent);
            Assert.AreNotEqual(unityComponent, component.UnityComponent);
        }

        [Test]
        public void ResolveUnityFromMefByPropTest()
        {
            var container = ConfigureUnityThenMef();

            var unityComponent = container.Resolve<IUnityComponent>();

            var component = container.Resolve<IMefDependingOnUnity2>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.UnityComponent);
            Assert.AreNotEqual(unityComponent, component.UnityComponent);
        }

        [Test]
        public void ResolveUnityFromMefByProp2Test()
        {
            var container = ConfigureMefThenUnity();

            var unityComponent = container.Resolve<IUnityComponent>();

            var component = container.Resolve<IMefDependingOnUnity2>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.UnityComponent);
            Assert.AreNotEqual(unityComponent, component.UnityComponent);
        }

        [Test]
        public void ResolveUnityFromMefByCtorWithScopeTest()
        {
            IUnityContainer container = ConfigureUnityThenMef();

            var unityComponent = container.Resolve<IUnityComponent>();
            container = container.CreateChildContainer(true);
            container.RegisterInstance(unityComponent);

            var component = container.Resolve<IMefDependingOnUnity>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.UnityComponent);
            Assert.AreEqual(unityComponent, component.UnityComponent);
        }

        [Test]
        public void ResolveUnityFromMefByCtorWithScope2Test()
        {
            IUnityContainer container = ConfigureMefThenUnity();

            var unityComponent = container.Resolve<IUnityComponent>();
            container = container.CreateChildContainer();
            container.EnableCompositionIntegration();
            container.RegisterInstance(unityComponent);

            var component = container.Resolve<IMefDependingOnUnity>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.UnityComponent);
            Assert.AreEqual(unityComponent, component.UnityComponent);
        }

        [Test]
        public void ResolveUnityFromMefByPropWithScopeTest()
        {
            IUnityContainer container = ConfigureUnityThenMef();

            var unityComponent = container.Resolve<IUnityComponent>();
            container = container.CreateChildContainer();
            container.EnableCompositionIntegration();
            container.RegisterInstance(unityComponent);

            var component = container.Resolve<IMefDependingOnUnity2>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.UnityComponent);
            Assert.AreEqual(unityComponent, component.UnityComponent);
        }

        [Test]
        public void ResolveUnityFromMefByPropWithScope2Test()
        {
            IUnityContainer container = ConfigureMefThenUnity();

            var unityComponent = container.Resolve<IUnityComponent>();
            container = container.CreateChildContainer();
            container.EnableCompositionIntegration();
            container.RegisterInstance(unityComponent);

            var component = container.Resolve<IMefDependingOnUnity2>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.UnityComponent);
            Assert.AreEqual(unityComponent, component.UnityComponent);
        }


        public interface ICountableUnityComponent
        {
            int InstanceCount { get; }
        }

        public interface ICountableMefComponent
        {
            int InstanceCount { get; }
        }

        public class CountableUnityComponent : ICountableUnityComponent
        {
            private static int m_InstanceCount;

            public CountableUnityComponent()
            {
                ++m_InstanceCount;
            }

            public static void ResetInstanceCount()
            {
                m_InstanceCount = 0;
            }

            public int InstanceCount { get { return m_InstanceCount; } }
        }

        [Export(typeof(ICountableMefComponent))]
        public class CountableMefComponent : ICountableMefComponent
        {
            private static int m_InstanceCount;

            public CountableMefComponent()
            {
                ++m_InstanceCount;
            }

            public static void ResetInstanceCount()
            {
                m_InstanceCount = 0;
            }

            public int InstanceCount { get { return m_InstanceCount; } }
        }

        [Test]
        public void UnityInstanceCountTest()
        {
            CountableUnityComponent.ResetInstanceCount();
            IUnityContainer container = ConfigureUnityThenMef();
            var countable = container.Resolve<ICountableUnityComponent>();
            Assert.AreEqual(1, countable.InstanceCount);
        }

        [Test]
        public void UnityInstanceCount2Test()
        {
            CountableUnityComponent.ResetInstanceCount();
            IUnityContainer container = ConfigureMefThenUnity();
            var countable = container.Resolve<ICountableUnityComponent>();
            Assert.AreEqual(1, countable.InstanceCount);
        }

        [Test]
        public void MefInstanceCountTest()
        {
            CountableMefComponent.ResetInstanceCount();
            IUnityContainer container = ConfigureUnityThenMef();
            var countable = container.Resolve<ICountableMefComponent>();
            Assert.AreEqual(1, countable.InstanceCount);
        }

        [Test]
        public void MefInstanceCount2Test()
        {
            CountableMefComponent.ResetInstanceCount();
            IUnityContainer container = ConfigureMefThenUnity();
            var countable = container.Resolve<ICountableMefComponent>();
            Assert.AreEqual(1, countable.InstanceCount);
        }

        public interface IDependOnCountableUnity
        {
            ICountableUnityComponent Component { get; set; }
        }

        [Export(typeof(IDependOnCountableUnity))]
        public class DependOnCountableUnity : IDependOnCountableUnity
        {
            [Import]
            public ICountableUnityComponent Component { get; set; }
        }

        [Test]
        public void UnityInstanceCountDepScopedTest()
        {
            CountableUnityComponent.ResetInstanceCount();
            IUnityContainer container = ConfigureUnityThenMef();
            var childContainer = container.CreateChildContainer(true);
            var countableDep = childContainer.Resolve<IDependOnCountableUnity>();
            Assert.AreEqual(1, countableDep.Component.InstanceCount);
        }

        [Test]

        public void MefCanResolveLazyTypeRegisteredInMefWithTextMetadataTest()
        {

            ComposablePartCatalog catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            CompositionContainer cc = new CompositionContainer(catalog);

            var export = cc.GetExports<IPartWithTextMetadata, IDictionary<string, object>>();

        }



        [Test]
        public void UnityCanResolveLazyTypeRegisteredInMefWithTextMetadataTest()
        {

            // Setup

            var unityContainer = new UnityContainer();

            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());



            // Add composition support for unity

            unityContainer.AddExtension(new CompositionIntegration(false));

            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);



           // Lazy<IPartWithTextMetadata, IDictionary<string, object>> v = new Lazy<IPartWithTextMetadata, IDictionary<string, object>>();



            //unityContainer.Configure<InjectedMembers>().ConfigureInjectionFor<Lazy<IPartWithTextMetadata, IDictionary<string, object>>>(

            //    new InjectionConstructor()

            //    );



            Lazy<IPartWithTextMetadata, IDictionary<string, object>> lazyMefComponent = unityContainer.Resolve<Lazy<IPartWithTextMetadata, IDictionary<string, object>>>();



            Assert.That(lazyMefComponent, Is.Not.Null);

            Assert.That(lazyMefComponent.Value, Is.Not.Null);

            Assert.That(lazyMefComponent.Metadata, Is.Not.Null);



            Assert.That(lazyMefComponent.Value.GetType(), Is.EqualTo(typeof(HelloWorldDispatcher)));

        }


        [Test]
        public void UnityCanResolveLazyTypeRegisteredInMefWithStronglyTypedMetadataTest()
        {
            //throw new NotImplementedException();
            // Setup

            var unityContainer = new UnityContainer();

            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());



            // Add composition support for unity

            unityContainer.AddExtension(new CompositionIntegration(false));

            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);
      

            Lazy<IPartWithStronglyTypedMetadata, IMyStronglyTypedMetadataAttribute> lazyMefComponent = unityContainer.Resolve<Lazy<IPartWithStronglyTypedMetadata, IMyStronglyTypedMetadataAttribute>>();



            Assert.That(lazyMefComponent, Is.Not.Null);

            Assert.That(lazyMefComponent.Value, Is.Not.Null);

            Assert.That(lazyMefComponent.Metadata, Is.Not.Null);

            Assert.That(lazyMefComponent.Metadata.ListOfNames[0], Is.EqualTo("Element One"));
            Assert.That(lazyMefComponent.Metadata.ListOfNames[1], Is.EqualTo("Element Two"));

            Assert.That(lazyMefComponent.Metadata.MetadataIdentifier, Is.EqualTo(5));

            Assert.That(lazyMefComponent.Value.GetType(), Is.EqualTo(typeof(StronglyTypedHelloWorldDispatcher)));

        }



        [Test]

        public void UnityCanResolveLazyTypeRegisteredInMefWithoutItsMetadataTest()
        {

            // Setup

            var unityContainer = new UnityContainer();

            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());



            // Add composition support for unity

            unityContainer.AddExtension(new CompositionIntegration(false));

            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);



            var lazyMefComponent = unityContainer.Resolve<Lazy<IPartWithTextMetadata>>();



            Assert.That(lazyMefComponent, Is.Not.Null);

            Assert.That(lazyMefComponent.Value, Is.Not.Null);

            Assert.That(lazyMefComponent, Is.Not.Null);
        }

        [Test]
        public void UnityCanResolveEnumerableOfLazyTypeRegisteredInMefWithStronglyTypedMetadataTest()
        {
            //throw new NotImplementedException();
            // Setup

            var unityContainer = new UnityContainer();

            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());



            // Add composition support for unity

            unityContainer.AddExtension(new CompositionIntegration(false));

            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);
            


            var lazyMefComponent = unityContainer.Resolve<IEnumerable<Lazy<IPartWithStronglyTypedMetadata, IMyStronglyTypedMetadataAttribute>>>().ToArray();



            Assert.That(lazyMefComponent, Is.Not.Null);

            Assert.That(lazyMefComponent[0].Value, Is.Not.Null);

            Assert.That(lazyMefComponent[0].Metadata, Is.Not.Null);



            Assert.That(lazyMefComponent[0].Value.GetType(), Is.EqualTo(typeof(StronglyTypedHelloWorldDispatcher)));

        }
    }
}