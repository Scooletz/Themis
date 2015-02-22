using System;
using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping;
using NHibernate.Persister.Entity;
using Themis.Cfg;
using Themis.NHibernate.Impl;
using Themis.NHibernate.Persister.Entity;
using Themis.NHibernate.Util;

namespace Themis.NHibernate.Cfg
{
    /// <summary>
    /// The configuration element used to configure the nhibernate filters,
    /// with expressions, and <see cref="IDemand{T}"/> approach.
    /// </summary>
    public class NHibernateFilteringConfiguration
    {
        private readonly Action<IDemandService> _afterCreation;
        private readonly Configuration _cfg;
        private readonly EvaluatorFactory _factory;
        private readonly FluentConfiguration _fluentConfiguration;

        internal NHibernateFilteringConfiguration(FluentConfiguration fluentConfiguration, Configuration cfg, Action<IDemandService> afterCreation)
        {
            _fluentConfiguration = fluentConfiguration;
            _cfg = cfg;
            _afterCreation = afterCreation;

            var dialect = Dialect.GetDialect(cfg.Properties);
            cfg.CreateMappings(dialect).AddSecondPass(ForNhConfiguration);

            _factory = new EvaluatorFactory(cfg);
            _fluentConfiguration.AddEvaluatorFactory(_factory);
        }

        public static void ReplacePersisterType(PersistentClass model)
        {
            var persisterClass = model.EntityPersisterClass;

            if ((persisterClass == null) || (persisterClass == typeof (SingleTableEntityPersister)))
            {
                model.EntityPersisterClass = typeof (SpecialFilteringSingleTableEntityPersister);
            }
            if (persisterClass == typeof (JoinedSubclassEntityPersister))
            {
                model.EntityPersisterClass = typeof (SpecialFilteringJoinedSubclassEntityPersister);
            }
            if (persisterClass == typeof (UnionSubclassEntityPersister))
            {
                model.EntityPersisterClass = typeof (SpecialFilteringUnionSubclassEntityPersister);
            }
        }

        /// <summary>
        /// Returns the original <see cref="FluentConfiguration"/> object.
        /// </summary>
        /// <returns>An original <see cref="FluentConfiguration"/> object.</returns>
        public FluentConfiguration BackToMainConfig()
        {
            return _fluentConfiguration;
        }

        private void ForNhConfiguration(IDictionary<string, PersistentClass> persistentclasses)
        {
            if (FilterHelper.ContainsAnyThemisFilter(_cfg))
            {
                throw new InvalidOperationException(
                    "The configuration contains a filter with name beginning with forbidden preffix: " +
                    FilterHelper.ThemisFilterPreffix);
            }

            foreach (var kvp in persistentclasses)
            {
                ReplacePersisterType(kvp.Value);
            }

            _factory.SetPersistentClasses(persistentclasses);

            // finally, build service
            var service = _fluentConfiguration.BuildDemandService();
            _afterCreation(service);
        }
    }
}