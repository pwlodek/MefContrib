namespace MefContrib.Hosting.Isolation
{
    /// <summary>
    /// Metadata describing the isolation for a particular part.
    /// </summary>
    public interface IIsolationMetadata
    {
        /// <summary>
        /// Gets the isolation for a part.
        /// </summary>
        IsolationLevel Isolation { get; }

        /// <summary>
        /// Indicates if a new host should be created for a new instance of the part.
        /// </summary>
        bool HostPerInstance { get; }

        /// <summary>
        /// Gets the name of the isolation group. Used to make sure parts which
        /// defined the same name will be hosted in the same activation host.
        /// </summary>
        string IsolationGroup { get; }
    }
}