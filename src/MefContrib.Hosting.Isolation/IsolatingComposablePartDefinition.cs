using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;

namespace MefContrib.Hosting.Isolation
{
    public class IsolatingComposablePartDefinition : ComposablePartDefinition
    {
        private readonly ComposablePartDefinition _sourcePart;

        public IsolatingComposablePartDefinition(ComposablePartDefinition sourcePart)
        {
            _sourcePart = sourcePart;
        }

        public override ComposablePart CreatePart()
        {
            return new IsolatingComposablePart(_sourcePart);
        }

        public override IEnumerable<ExportDefinition> ExportDefinitions
        {
            get { return _sourcePart.ExportDefinitions; }
        }

        public override IEnumerable<ImportDefinition> ImportDefinitions
        {
            get { return _sourcePart.ImportDefinitions; }
        }

        public override IDictionary<string, object> Metadata
        {
            get { return _sourcePart.Metadata; }
        }
    }
}