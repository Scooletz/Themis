using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Cfg;
using NHibernate.Mapping;
using Themis.Impl;
using Themis.Model;

namespace Themis.NHibernate.Impl
{
    /// <summary>
    /// The implementation of <see cref="IEvaluatorFactory"/> for NHibernate integration.
    /// </summary>
    /// <remarks>
    /// This factory returns one <see cref="FilteringDemandEvaluator"/> for each role. The evaluator is returned only once,
    /// when the generic method <see cref="GetEvaluators{TDemand,TRole,TResult}"/> is called for the first 
    /// time for the specific role.
    /// This behaviour results in creating one evaluator per role.
    /// </remarks>
    public class EvaluatorFactory : IEvaluatorFactory
    {
        private static readonly IEvaluator[] Empty = new IEvaluator[0];

        private readonly Configuration _cfg;
        private readonly IDictionary<Type, FilterConditionBuilder> _conditionBuilders;

        /// <summary>
        /// A dictionary of lists of member access expressions having their roots in a role.
        /// </summary>
        private readonly Dictionary<Type, FilteringDemandEvaluator> _evaluators;

        private bool _configurationFinished;

        private IDictionary<string, PersistentClass> _persistentClasses;

        public EvaluatorFactory(Configuration cfg)
        {
            _cfg = cfg;
            _evaluators = new Dictionary<Type, FilteringDemandEvaluator>();
            _conditionBuilders = new Dictionary<Type, FilterConditionBuilder>();
        }

        #region IEvaluatorFactory Members

        public IEvaluator[] GetEvaluators<TDemand, TRole, TResult>(Expression<Func<TDemand, TRole, TResult>> expression)
            where TDemand : class, IDemand<TResult>
            where TRole : class
        {
            // if not entity demand return
            var dtype = typeof (TDemand);
            if (!dtype.IsGenericType || dtype.GetGenericTypeDefinition() != typeof (EntityDemand<>))
                return Empty;

            var entityType = dtype.GetGenericArguments()[0];
            FindPersistentClass(entityType);

            // find all member access expressions which start in a role object
            var roleParam = expression.Parameters[1];
            var roleExpressions = new ParameterMemberExpressionFinder(roleParam).FindExpressionsIn(expression);

            var role = typeof (TRole);

            AddToConditionBuilder(entityType, expression);

            if (_evaluators.ContainsKey(role))
            {
                _evaluators[role].AddRoleMemberAccessExpressions(roleExpressions);
                return Empty; // evaluator exists, hence was returned once
            }

            // create evaluator and store it
            var evaluator = new FilteringDemandEvaluator(role);
            evaluator.AddRoleMemberAccessExpressions(roleExpressions);
            _evaluators[role] = evaluator;
            return new[] {evaluator};
        }

        public void EndModelsBuildUp()
        {
            var roleToProvider = new Dictionary<Type, FilterParameterProvider>();

            // sealing up evaluators
            foreach (var kvp in _evaluators)
            {
                roleToProvider[kvp.Key] = kvp.Value.Seal(_cfg);
            }

            foreach (var kvp in _conditionBuilders)
            {
                kvp.Value.BuildAndAddFilters(roleToProvider);
            }

            _configurationFinished = true;
        }

        #endregion

        private void AddToConditionBuilder<TDemand, TRole, TResult>(Type entityType,
                                                                    Expression<Func<TDemand, TRole, TResult>> expression)
            where TDemand : class, IDemand<TResult>
            where TRole : class
        {
            if (!_conditionBuilders.ContainsKey(entityType))
            {
                _conditionBuilders[entityType] = new FilterConditionBuilder(entityType, FindPersistentClass(entityType));
            }
            _conditionBuilders[entityType].Add(expression);
        }

        private PersistentClass FindPersistentClass(Type entityType)
        {
            if (_persistentClasses == null)
            {
                throw new InvalidOperationException(
                    "The NH configuration did not run second pass. Create session factory to allow this class to work.");
            }
            if (_configurationFinished)
            {
                throw new InvalidOperationException(
                    "The configuration of demans has ended. You cannot obtain another demand evaluator");
            }

            var foundPersistentClass = _persistentClasses.Any(kvp => kvp.Value.EntityName == entityType.FullName);
            if (!foundPersistentClass)
            {
                throw new KeyNotFoundException("The persistent class for " +
                                               entityType.Name +
                                               " was not found. Check your mappings ensuring that you do not override entity name in your configuration");
            }

            return _persistentClasses.First(kvp => kvp.Value.EntityName == entityType.FullName).Value;
        }

        internal void SetPersistentClasses(IDictionary<string, PersistentClass> persistentClasses)
        {
            _persistentClasses = persistentClasses;
        }
    }
}