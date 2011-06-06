using System;
using System.ComponentModel.Composition;

namespace MefContrib.Hosting.Isolation
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IsolationAttribute : Attribute, IIsolationMetadata
    {
        public IsolationLevel Isolation { get; set; }

        public string IsolationGroup { get; set; }
    }
}