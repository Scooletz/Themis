using System.Collections.Generic;
using NHibernate;

namespace Themis.NHibernate.Impl
{
    /// <summary>
    /// The class used by <see cref="FilterScope"/> to enable filter on a <see cref="ISession"/>.
    /// </summary>
    internal class FilterApplier
    {
        private readonly string _filterName;
        private readonly IDictionary<string, object> _values;

        public FilterApplier(string filterName, IDictionary<string, object> values)
        {
            _filterName = filterName;
            _values = values;
        }

        public void Enable(ISession session)
        {
            var filter = session.EnableFilter(_filterName);

            foreach (var kvp in _values)
            {
                filter.SetParameter(kvp.Key, kvp.Value);
            }
        }

        public void Disable(ISession session)
        {
            session.DisableFilter(_filterName);
        }
    }
}