namespace MefContrib.Hosting.Isolation.Runtime
{
    using System;
    using MefContrib.Hosting.Isolation.Runtime.Activation;

    /// <summary>
    /// Represents a clinet reference to the remote object.
    /// </summary>
    [Serializable]
    public class ObjectReference : IEquatable<ObjectReference>
    {
        private Guid _guid;

        public ObjectReference(ActivationHostDescription description)
        {
            if (description == null)
            {
                throw new ArgumentNullException("description");
            }
            
            Description = description;
            _guid = Guid.NewGuid();
        }

        /// <summary>
        /// Indicates if the object is faulted. This happens if an activation host responsible
        /// for maintaining the original object crashes.
        /// </summary>
        public bool Faulted { get; internal set; }

        /// <summary>
        /// Indicates if the object is disposable.
        /// </summary>
        public bool IsDisposable { get; internal set; }

        /// <summary>
        /// indicates if the remote object is disposed.
        /// </summary>
        public bool IsDisposed { get; internal set; }

        /// <summary>
        /// Gets information about activation host used to activate this object.
        /// </summary>
        public ActivationHostDescription Description { get; private set; }

        #region Overrides

        public override string ToString()
        {
            return _guid.ToString();
        }

        #endregion

        #region Equality implementation

        public bool Equals(ObjectReference other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._guid.Equals(_guid);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ObjectReference)) return false;
            return Equals((ObjectReference) obj);
        }

        public override int GetHashCode()
        {
            return _guid.GetHashCode();
        }
        
        public static bool operator ==(ObjectReference left, ObjectReference right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ObjectReference left, ObjectReference right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}