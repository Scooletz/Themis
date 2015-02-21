using System;

namespace Themis.Tests.NHibernate.Data
{
    public class RecruitmentMotion
    {
        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }

        public virtual Employee Owner { get; set; }

        public virtual Unit ForUnit { get; set; }
    }
}