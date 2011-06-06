namespace MefContrib.Hosting.Isolation
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;

    public class IsolatingCatalog : ComposablePartCatalog
    {
        private readonly object _synchRoot = new object();
        private readonly ComposablePartCatalog _interceptedCatalog;
        private IQueryable<ComposablePartDefinition> _innerPartsQueryable;

        public IsolatingCatalog(ComposablePartCatalog interceptedCatalog)
        {
            _interceptedCatalog = interceptedCatalog;
        }

        public override IQueryable<ComposablePartDefinition> Parts
        {
            get { return GetParts(); }
        }
        
        private IQueryable<ComposablePartDefinition> GetParts()
        {
            if (_innerPartsQueryable == null)
            {
                lock (_synchRoot)
                {
                    if (_innerPartsQueryable == null)
                    {
                        IEnumerable<ComposablePartDefinition> parts =
                            new List<ComposablePartDefinition>(_interceptedCatalog.Parts);
                        

                        _innerPartsQueryable = parts.Select(GetPart).AsQueryable();
                    }
                }
            }

            return _innerPartsQueryable;
        }

        private ComposablePartDefinition GetPart(ComposablePartDefinition original)
        {
            foreach (var exportDefinition in original.ExportDefinitions)
            {
                var metadata = ContractServices.TryGetMetadataView<IIsolationMetadata>(exportDefinition.Metadata);
                if (metadata != null)
                {
                    return new IsolatingComposablePartDefinition(original);
                }
            }

            return original;
        }
    }
}