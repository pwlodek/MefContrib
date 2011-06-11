using System;
using System.ComponentModel.Composition;

namespace MefContrib.Hosting.Isolation
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IsolatedAttribute : Attribute, IIsolationMetadata
    {
        public IsolationLevel Isolation { get; set; }

        public bool HostPerInstance { get; set; }

        public string IsolationGroup { get; set; }
    }
}