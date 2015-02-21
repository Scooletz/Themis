using System;

namespace Themis.Tests
{
    public class TypeTrio : IEquatable<TypeTrio>
    {
        public Type Permission { get; set; }

        public Type Role { get; set; }

        public Type Result { get; set; }

        #region IEquatable<TypeTrio> Members

        public bool Equals(TypeTrio other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Permission, Permission) && Equals(other.Role, Role) && Equals(other.Result, Result);
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (TypeTrio)) return false;
            return Equals((TypeTrio) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = (Permission != null ? Permission.GetHashCode() : 0);
                result = (result*397) ^ (Role != null ? Role.GetHashCode() : 0);
                result = (result*397) ^ (Result != null ? Result.GetHashCode() : 0);
                return result;
            }
        }
    }
}