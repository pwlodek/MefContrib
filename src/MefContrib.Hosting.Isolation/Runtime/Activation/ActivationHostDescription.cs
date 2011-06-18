namespace MefContrib.Hosting.Isolation.Runtime.Activation
{
    using System;

    /// <summary>
    /// Represents a description of an activation host.
    /// </summary>
    [Serializable]
    public class ActivationHostDescription : IEquatable<ActivationHostDescription>
    {
        public ActivationHostDescription(IsolationLevel isolationLevel, string groupName)
        {
            Id = Guid.NewGuid();
            Group = groupName;
            Isolation = isolationLevel;
        }

        public ActivationHostDescription(IsolationLevel isolationLevel)
        {
            Id = Guid.NewGuid();
            Isolation = isolationLevel;
        }

        /// <summary>
        /// Id of the activation host.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Name of the activation group this activation host belongs.
        /// </summary>
        public string Group { get; private set; }

        /// <summary>
        /// Isolation level of this host.
        /// </summary>
        public IsolationLevel Isolation { get; private set; }

        #region Equality Implementation

        public bool Equals(ActivationHostDescription other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Id.Equals(Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ActivationHostDescription)) return false;
            return Equals((ActivationHostDescription) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(ActivationHostDescription left, ActivationHostDescription right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ActivationHostDescription left, ActivationHostDescription right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}