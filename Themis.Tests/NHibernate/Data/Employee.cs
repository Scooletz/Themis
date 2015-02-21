using System;

namespace Themis.Tests.NHibernate.Data
{
    public class Employee
    {
        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Surname { get; set; }

        public virtual DateTime EmployementDate { get; set; }

        public virtual Unit EmployingUnit { get; set; }
    }
}