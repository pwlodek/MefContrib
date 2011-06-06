using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;

namespace MefContrib.Hosting.Isolation
{
    public class IsolatingComposablePart : ComposablePart
    {
        private readonly IDictionary<ExportDefinition, object> _values;
        private readonly ComposablePartDefinition _definition;

        public IsolatingComposablePart(ComposablePartDefinition definition)
        {
            _definition = definition;
            _values = new Dictionary<ExportDefinition, object>();
        }

        public override object GetExportedValue(ExportDefinition definition)
        {
            if (_values.ContainsKey(definition))
            {
                return _values[definition];
            }

            var memberInfo = ReflectionModelServices.GetExportingMember(definition);
            var type = (Type) memberInfo.GetAccessors()[0];
            var metadata = AttributedModelServices.GetMetadataView<IIsolationMetadata>(definition.Metadata);
            var isolationLevel = metadata.Isolation;
            var partProxy = PartHost.CreateInstance(type, isolationLevel);

            _values[definition] = partProxy;

            return partProxy;
        }

        public override void SetImport(ImportDefinition definition, IEnumerable<Export> exports)
        {
        }

        public override IEnumerable<ExportDefinition> ExportDefinitions
        {
            get { return _definition.ExportDefinitions; }
        }

        public override IEnumerable<ImportDefinition> ImportDefinitions
        {
            get { return _definition.ImportDefinitions; }
        }

        public override IDictionary<string, object> Metadata
        {
            get { return _definition.Metadata; }
        }
    }
}