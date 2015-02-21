using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Themis.Impl;
using Themis.Model;

namespace Themis.Cfg
{
    /// <summary>
    /// The class providing a fluent interface of defining the role permissions.
    /// </summary>
    /// <typeparam name="TRole">The type of the role.</typeparam>
    /// <remarks>
    /// The <typeparamref name="TRole"/> can be any reference type. There is no root interface or a class. 
    /// You can map any structure used in your application.
    /// </remarks>
    public abstract class RoleDefinition<TRole> : IModelProvider
        where TRole : class
    {
        private readonly Dictionary<Type, List<Func<IEvaluatorFactory, IEvaluator[]>>> _permissionToEvaluators;

        protected RoleDefinition()
        {
            _permissionToEvaluators = new Dictionary<Type, List<Func<IEvaluatorFactory, IEvaluator[]>>>();
        }

        #region IModelProvider Members

        IRoleModel IModelProvider.GetModel(IEnumerable<IEvaluatorFactory> factories)
        {
            var evaluators = _permissionToEvaluators
                .SelectMany(kvp => kvp.Value)
                .SelectMany(action => factories.SelectMany(f => action(f)));

            return new RoleModel<TRole>(evaluators);
        }

        #endregion

        /// <summary>
        /// Adds the specified expression describing the demand.
        /// </summary>
        /// <typeparam name="TDemand">The type of the permission.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <remarks>Use this method in a constructor of a definition derived from this class
        /// to map all the demands. Every <paramref name="expression"/> will be processed by
        /// the <see cref="IEvaluatorFactory"/> during <see cref="FluentConfiguration.BuildDemandService"/> process. </remarks>
        protected void Add<TDemand, TResult>(Expression<Func<TDemand, TRole, TResult>> expression)
            where TDemand : class, IDemand<TResult>
        {
            var key = typeof(TDemand);

            if (!_permissionToEvaluators.ContainsKey(key))
            {
                _permissionToEvaluators[key] = new List<Func<IEvaluatorFactory, IEvaluator[]>>();
            }

            _permissionToEvaluators[key].Add(new FactoryAction<TDemand, TResult>(expression).GetEvaluators);
        }

        /// <summary>
        /// Adds the specified expression describing the claim demand.
        /// </summary>
        /// <typeparam name="TClaim">The type of the claim.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <remarks>Use this method in a constructor of a definition derived from this class
        /// to map all the claims. Every <paramref name="expression"/> will be processed by
        /// the <see cref="IEvaluatorFactory"/> during <see cref="FluentConfiguration.BuildDemandService"/> process. </remarks>
        protected void Add<TClaim>(Expression<Func<TClaim, TRole, bool>> expression)
            where TClaim : class, IClaim
        {
            Add<TClaim, bool>(expression);
        }

        #region Nested type: FactoryAction

        private class FactoryAction<TDemand, TResult>
            where TDemand : class, IDemand<TResult>
        {
            private readonly Expression<Func<TDemand, TRole, TResult>> _expression;

            public FactoryAction(Expression<Func<TDemand, TRole, TResult>> expression)
            {
                _expression = expression;
            }

            public IEvaluator[] GetEvaluators(IEvaluatorFactory factory)
            {
                return factory.GetEvaluators(_expression);
            }
        }

        #endregion
    }
}