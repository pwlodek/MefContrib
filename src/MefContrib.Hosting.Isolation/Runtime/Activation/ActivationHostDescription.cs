using System;

namespace MefContrib.Hosting.Isolation.Runtime.Activation
{
    [Serializable]
    public class ActivationHostDescription : IEquatable<ActivationHostDescription>
    {
        public ActivationHostDescription(string groupName)
        {
            Id = Guid.NewGuid();
            Group = groupName;
        }

        public ActivationHostDescription()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }

        public string Group { get; private set; }

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