using System;
using MefContrib.Hosting.Isolation.Runtime.Activation;

namespace MefContrib.Hosting.Isolation.Runtime
{
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
        /// Indicates if the object is faulted. This can happen if an activation host crash.
        /// </summary>
        public bool Faulted { get; internal set; }

        public ActivationHostDescription Description { get; private set; }

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

        public override string ToString()
        {
            return _guid.ToString();
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