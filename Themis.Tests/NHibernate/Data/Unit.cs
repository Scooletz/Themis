using System;
using System.Collections.Generic;

namespace Themis.Tests.NHibernate.Data
{
    public class Unit
    {
// ReSharper disable InconsistentNaming
        private readonly ISet<Unit> childUnits = new HashSet<Unit>();
// ReSharper restore InconsistentNaming

        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }

        public virtual Employee Manager { get; set; }

        public virtual Unit ParentUnit { get; set; }

        public virtual ISet<Unit> ChildUnits { get { return childUnits; } }

        public virtual void AddChildUnit(Unit unit)
        {
            if (unit == null)
                throw new ArgumentException("Can't add a null Unit as child.");
            // Remove from old parent category
            if (unit.ParentUnit != null)
                unit.ParentUnit.ChildUnits.Remove(unit);
            // Set parent in child
            unit.ParentUnit = this;
            // Set child in parent
            ChildUnits.Add(unit);
        }
    }
}