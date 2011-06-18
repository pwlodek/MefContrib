namespace MefContrib.Hosting.Isolation
{
    using System;
    using System.ComponentModel.Composition;

    /// <summary>
    /// Marks specified part to be instantiated in isolation.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IsolatedAttribute : Attribute, IIsolationMetadata
    {
        /// <summary>
        /// Gets the isolation for a part.
        /// </summary>
        public IsolationLevel Isolation { get; set; }

        /// <summary>
        /// Indicates if a new host should be created for a new instance of the part.
        /// </summary>
        public bool HostPerInstance { get; set; }

        /// <summary>
        /// Gets the name of the isolation group. Used to make sure parts which
        /// defined the same name will be hosted in the same activation host.
        /// </summary>
        public string IsolationGroup { get; set; }
    }
}