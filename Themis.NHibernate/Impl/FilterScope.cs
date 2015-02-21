using NHibernate;

// ReSharper disable ForCanBeConvertedToForeach

namespace Themis.NHibernate.Impl
{
    /// <summary>
    /// The implementation of <see cref="FilterScope"/> providing thread static scope
    /// for filtering nhibernate queries.
    /// </summary>
    public class FilterScope : IFilterScope
    {
        private readonly FilterApplier[] _filterAppliers;
        private readonly ISession _session;

        public FilterScope(IDemandService service, ISession session, object[] roles)
        {
            _filterAppliers = service.Evaluate<FilteringDemand, FilterApplier>(FilteringDemand.Instance, roles);

            for (var i = 0; i < _filterAppliers.Length; i++)
            {
                _filterAppliers[i].Enable(session);
            }

            _session = session;
        }

        #region IFilterScope Members

        public void Dispose()
        {
            for (var i = 0; i < _filterAppliers.Length; i++)
            {
                _filterAppliers[i].Disable(_session);
            }
        }

        #endregion
    }
}

// ReSharper restore ForCanBeConvertedToForeach