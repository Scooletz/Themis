using System;
using NHibernate;

namespace Themis.NHibernate.Impl
{
    /// <summary>
    /// The implementation of <see cref="FilterScope"/> providing thread static scope
    /// for filtering nhibernate queries.
    /// </summary>
    public class FilterScope : IDisposable
    {
        private readonly FilterApplier[] _filterAppliers;
        private readonly ISession _session;

        public FilterScope(IDemandService service, ISession session, object[] roles)
        {
            _filterAppliers = service.Evaluate<FilteringDemand, FilterApplier>(FilteringDemand.Instance, roles);

            foreach (var t in _filterAppliers)
            {
                t.Enable(session);
            }

            _session = session;
        }

        public void Dispose()
        {
            foreach (var t in _filterAppliers)
            {
                t.Disable(_session);
            }
        }
    }
}
