using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Themis.Claims
{
    /// <summary>
    /// A readonly wrapper around <see cref="ClaimsIdentity"/>.
    /// </summary>
    public sealed class ReadonlyClaimIdentity
    {
        private readonly ClaimsIdentity _identity;

        public ReadonlyClaimIdentity(ClaimsIdentity identity)
        {
            _identity = identity;
        }

        public IEnumerable<Claim> FindAll(Predicate<Claim> match)
        {
            return _identity.FindAll(match);
        }

        public IEnumerable<Claim> FindAll(string type)
        {
            return _identity.FindAll(type);
        }

        public bool HasClaim(Predicate<Claim> match)
        {
            return _identity.HasClaim(match);
        }

        public bool HasClaim(string type, string value)
        {
            return _identity.HasClaim(type, value);
        }

        public Claim FindFirst(Predicate<Claim> match)
        {
            return _identity.FindFirst(match);
        }

        public Claim FindFirst(string type)
        {
            return _identity.FindFirst(type);
        }

        public string AuthenticationType
        {
            get { return _identity.AuthenticationType; }
        }

        public IEnumerable<Claim> Claims
        {
            get { return _identity.Claims; }
        }

        public string Name
        {
            get { return _identity.Name; }
        }

        public string NameClaimType
        {
            get { return _identity.NameClaimType; }
        }

        public string RoleClaimType
        {
            get { return _identity.RoleClaimType; }
        }

        public bool IsAuthenticated
        {
            get { return _identity.IsAuthenticated; }
        }
    }
}