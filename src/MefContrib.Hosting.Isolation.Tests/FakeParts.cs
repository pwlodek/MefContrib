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
        public static bool FooHasBeenCalled;

        public void Foo()
        {
            FooHasBeenCalled = true;
        }
    }

    [Export(typeof(IFakePart)), Isolated(Isolation = IsolationLevel.None)]
    public class DisposableFakePart1 : IFakePart, IDisposable
    {
        public static bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }

        public void Foo()
        {
        }
    }

    public interface IFakePart
    {
        void Foo();
    }

    [IsolatedExport(typeof(IFakePartSupportingValueTypes), Isolation = IsolationLevel.None)]
    public class FakePartSupportingValueTypes : IFakePartSupportingValueTypes
    {
        public string Foo(string text, int intNumber, double doubleNumber)
        {
            return string.Format("{0} {1} {2}", text, intNumber, doubleNumber);
        }
    }

    public interface IFakePartSupportingValueTypes
    {
        string Foo(string text, int intNumber, double doubleNumber);
    }

    [Serializable]
    public class Something
    {
        public string Name { get; set; }
    }

    [IsolatedExport(typeof(IFakePartSupportingSerializableTypes), Isolation = IsolationLevel.None)]
    public class FakePartSupportingSerializableTypes : IFakePartSupportingSerializableTypes
    {
        public Something Foo(Something input)
        {
            var s = new Something();
            s.Name = input.Name;

            return s;
        }
    }

    public interface IFakePartSupportingSerializableTypes
    {
        Something Foo(Something input);
    }
}