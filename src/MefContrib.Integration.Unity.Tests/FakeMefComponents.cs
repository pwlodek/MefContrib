using System;
using System.ComponentModel.Composition;

namespace MefContrib.Integration.Unity.Tests
{
    using System.Collections.Generic;

    public interface IMefComponent
    {
        void Foo();
    }

    [Export(typeof(IMefComponent))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MefComponent1 : IMefComponent
    {
        public static int InstanceCount;

        public MefComponent1()
        {
            InstanceCount++;
        }

        public void Foo()
        {
        }
    }

    [Export("component2", typeof(IMefComponent))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MefComponent2 : IMefComponent
    {
        public void Foo()
        {
        }
    }

    [Export]
    public class MefComponentDisposable : IDisposable
    {
        public bool Disposed { get; private set; }

        public MefComponentDisposable()
        {
            
        }
        
        public void Dispose()
        {
            this.Disposed = true;
        }
    }

    public interface IMefComponentWithUnityDependencies
    {
        IUnityOnlyComponent UnityOnlyComponent { get; set; }
        IMefComponent MefOnlyComponent { get; set; }
        void Foo();
    }

    [Export(typeof(IMefComponentWithUnityDependencies))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MefComponentWithUnityDependencies1 : IMefComponentWithUnityDependencies
    {
        public IMefComponent MefOnlyComponent { get; set; }
        public IUnityOnlyComponent UnityOnlyComponent { get; set; }

        [ImportingConstructor]
        public MefComponentWithUnityDependencies1(
            IUnityOnlyComponent unityOnlyComponent,
            IMefComponent mefComponent)
        {
            MefOnlyComponent = mefComponent;
            UnityOnlyComponent = unityOnlyComponent;
        }

        public void Foo()
        {
        }
    }

    [Export("component2", typeof(IMefComponentWithUnityDependencies))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MefComponentWithUnityDependencies2 : IMefComponentWithUnityDependencies
    {
        public IMefComponent MefOnlyComponent { get; set; }
        public IUnityOnlyComponent UnityOnlyComponent { get; set; }
        public IUnityComponent MixedUnityMefComponent { get; set; }

        [ImportingConstructor]
        public MefComponentWithUnityDependencies2(
            IUnityOnlyComponent unityOnlyComponent,
            IMefComponent mefComponent,
            IUnityComponent mixedUnityMefComponent)
        {
            MefOnlyComponent = mefComponent;
            UnityOnlyComponent = unityOnlyComponent;
            MixedUnityMefComponent = mixedUnityMefComponent;
        }

        public void Foo()
        {
        }
    }

    public interface IMultipleMefComponent
    {
        void Foo();
    }

    public class MultipleMefComponent1 : IMultipleMefComponent
    {
        public void Foo()
        {
        }
    }

    public class MultipleMefComponent2 : IMultipleMefComponent
    {
        public void Foo()
        {
        }
    }

    [Export]
    public class MefMixedComponent
    {
        [ImportingConstructor]
        public MefMixedComponent(IMefComponent mefComponent, IUnityOnlyComponent unityComponent)
        {
            MefComponent = mefComponent;
            UnityComponent = unityComponent;
        }

        public IUnityOnlyComponent UnityComponent { get; set; }

        public IMefComponent MefComponent { get; set; }
    }

    // to test resolution with metadata

    public interface IPartWithTextMetadata
    {
        void HelloWorld(string message);
    }

    [Export(typeof(IPartWithTextMetadata))]
    [ExportMetadata("World", "Earth")]
    public class HelloWorldDispatcher : IPartWithTextMetadata
    {
        public void HelloWorld(string message)
        {
            Console.WriteLine(message);
        }
    }

    public interface IPartWithStronglyTypedMetadata
    {
    }

    [Export(typeof(IPartWithStronglyTypedMetadata))]
    [MyStronglyTypedMetadata(ListOfNames = new[] { "Element One", "Element Two" }, MetadataIdentifier = 5)]
    public class StronglyTypedHelloWorldDispatcher : IPartWithStronglyTypedMetadata
    {
        public void HelloWorld(string message)
        {
            Console.WriteLine(message);
        }
    }

    public interface IMyStronglyTypedMetadataAttribute
    {
        int MetadataIdentifier { get;  }

        string[] ListOfNames { get; }
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public class MyStronglyTypedMetadataAttribute : Attribute, IMyStronglyTypedMetadataAttribute
    {
        public int MetadataIdentifier { get; set; }

        public string[] ListOfNames { get; set; }
    }
}