using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Themis.Claims
{
    /// <summary>
    /// A readonly wrapper around <see cref="ClaimsPrincipal"/>.
    /// </summary>
    public sealed class ReadonlyClaimPrincipal
    {
        private readonly ClaimsPrincipal _principal;

        public ReadonlyClaimPrincipal(ClaimsPrincipal principal)
        {
            _principal = principal;
        }

        public IEnumerable<Claim> FindAll(Predicate<Claim> match)
        {
            return _principal.FindAll(match);
        }

        public IEnumerable<Claim> FindAll(string type)
        {
            return _principal.FindAll(type);
        }

        public Claim FindFirst(Predicate<Claim> match)
        {
            return _principal.FindFirst(match);
        }

        public Claim FindFirst(string type)
        {
            return _principal.FindFirst(type);
        }

        public bool HasClaim(Predicate<Claim> match)
        {
            return _principal.HasClaim(match);
        }

        public bool HasClaim(string type, string value)
        {
            return _principal.HasClaim(type, value);
        }

        public IEnumerable<Claim> Claims
        {
            get { return _principal.Claims; }
        }

        public IEnumerable<ReadonlyClaimIdentity> Identities
        {
            get { return _principal.Identities.Select(i => new ReadonlyClaimIdentity(i)); }
        }
    }
}