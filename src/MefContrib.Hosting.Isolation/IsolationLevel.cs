namespace MefContrib.Hosting.Isolation
{
    using System.ComponentModel.Composition.Hosting;

    /// <summary>
    /// Represents an isolation level for a part.
    /// </summary>
    public enum IsolationLevel
    {
        /// <summary>
        /// Part is activated in the same domain as the <see cref="CompositionContainer"/> used to instantiate it.
        /// </summary>
        None,

        /// <summary>
        /// Part is activated in a new app domain.
        /// </summary>
        AppDomain,

        /// <summary>
        /// Part is activated in a separate process.
        /// </summary>
        Process
    }
}