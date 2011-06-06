using System;

namespace MefContrib.Hosting.Isolation.Runtime
{
    [Serializable]
    public class ObjectReference : IEquatable<ObjectReference>
    {
        private readonly Guid _activatorHostId;
        private Guid _guid;

        public ObjectReference(Guid activatorHostId)
        {
            _activatorHostId = activatorHostId;
            _guid = Guid.NewGuid();
        }

        public Guid ActivatorHostId
        {
            get { return _activatorHostId; }
        }

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
    }
}