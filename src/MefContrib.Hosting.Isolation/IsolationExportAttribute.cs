using System;
using System.ComponentModel.Composition;

namespace MefContrib.Hosting.Isolation
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IsolationExportAttribute : ExportAttribute, IIsolationMetadata
    {
        public IsolationExportAttribute()
        {
        }

        public IsolationExportAttribute(Type contractType) : base(contractType)
        {
        }

        public IsolationExportAttribute(string contractName) : base(contractName)
        {
        }

        public IsolationExportAttribute(string contractName, Type contractType) : base(contractName, contractType)
        {
        }

        public IsolationLevel Isolation { get; set; }

        public bool HostPerInstance { get; set; }

        public string IsolationGroup { get; set; }
    }
}