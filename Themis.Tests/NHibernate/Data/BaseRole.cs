using System;

namespace Themis.Tests.NHibernate.Data
{
    public abstract class BaseRole
    {
        public virtual Guid Id { get; set; }

        public virtual Employee ForEmployee { get; set; }
    }
}