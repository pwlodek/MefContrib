namespace MefContrib.Hosting.Isolation.Runtime
{
    /// <summary>
    /// Identifies a n entity as being aware of its <see cref="ObjectReference"/>.
    /// </summary>
    /// <remarks>
    /// Each proxy created by the <see cref="IsolatingCatalog"/> supports this interface.
    /// </remarks>
    public interface IObjectReferenceAware
    {
        /// <summary>
        /// Gets the associated <see cref="ObjectReference"/> instance.
        /// </summary>
        ObjectReference Reference { get; }
    }
}