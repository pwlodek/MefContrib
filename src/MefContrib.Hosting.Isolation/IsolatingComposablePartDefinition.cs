namespace MefContrib.Hosting.Isolation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Primitives;
    using System.ComponentModel.Composition.ReflectionModel;

    public class IsolatingComposablePartDefinition : ComposablePartDefinition
    {
        private readonly ComposablePartDefinition _sourcePart;

        public IsolatingComposablePartDefinition(ComposablePartDefinition sourcePart)
        {
            if (sourcePart == null)
            {
                throw new ArgumentNullException("sourcePart");
            }

            _sourcePart = sourcePart;
        }

        public override ComposablePart CreatePart()
        {
            var part = ReflectionModelServices.IsDisposalRequired(_sourcePart)
                           ? new DisposableIsolatingComposablePart(_sourcePart)
                           : new IsolatingComposablePart(_sourcePart);

            return part;
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