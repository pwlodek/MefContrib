using System;
using System.ComponentModel.Composition;

namespace MefContrib.Hosting.Isolation
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IsolatedExportAttribute : ExportAttribute, IIsolationMetadata
    {
        public IsolatedExportAttribute()
        {
        }

        public IsolatedExportAttribute(Type contractType) : base(contractType)
        {
        }

        public IsolatedExportAttribute(string contractName) : base(contractName)
        {
        }

        public IsolatedExportAttribute(string contractName, Type contractType) : base(contractName, contractType)
        {
        }

        public IsolationLevel Isolation { get; set; }

        public bool HostPerInstance { get; set; }

        public string IsolationGroup { get; set; }
    }
}