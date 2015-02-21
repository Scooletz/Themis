using System.Collections.Generic;
using NHibernate;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Persister.Entity;
using Themis.NHibernate.Util;

namespace Themis.NHibernate.Persister.Entity
{
    public class SpecialFilteringSingleTableEntityPersister : SingleTableEntityPersister
    {
        private readonly FilterHelper _filterHelper;

        public SpecialFilteringSingleTableEntityPersister(PersistentClass persistentClass,
                                                          ICacheConcurrencyStrategy cache,
                                                          ISessionFactoryImplementor factory, IMapping mapping)
            : base(persistentClass, cache, factory, mapping)
        {
            _filterHelper = new FilterHelper(persistentClass.FilterMap, factory.Dialect, factory.SQLFunctionRegistry);
        }

        public override string FilterFragment(string alias, IDictionary<string, IFilter> enabledFilters)
        {
            return _filterHelper.FilterFragment(alias, enabledFilters, this);
        }
    }
}