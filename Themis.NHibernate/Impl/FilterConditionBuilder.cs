using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Mapping;
using Themis.Expressions;
using Themis.NHibernate.Util;

namespace Themis.NHibernate.Impl
{
    /// <summary>
    /// The builder class used to construct all of the filters applied by Themis to one entity type.
    /// </summary>
    /// <remarks>
    /// For each role, which had any expression of <see cref="EntityDemand{TEntity}"/> there 
    /// </remarks>
    internal class FilterConditionBuilder : ExpressionVisitor
    {
        private readonly Type _demandType;
        private readonly Type _entityType;
        private readonly PersistentClass _model;
        private readonly IDictionary<Type, List<LambdaExpression>> _roleConditions;

        public FilterConditionBuilder(Type entityType, PersistentClass model)
        {
            _entityType = entityType;
            _demandType = typeof(EntityDemand<>).MakeGenericType(entityType);
            _model = model;
            _roleConditions = new Dictionary<Type, List<LambdaExpression>>();
        }

        public void Add<TDemand, TRole, TResult>(Expression<Func<TDemand, TRole, TResult>> expression)
            where TDemand : class, IDemand<TResult>
            where TRole : class
        {
            var dtype = typeof(TDemand);
            if (!dtype.IsGenericType || dtype.GetGenericTypeDefinition() != typeof(EntityDemand<>))
            {
                throw new ArgumentException("The passed expression is not an EntityDemand", "expression");
            }

            var entityType = dtype.GetGenericArguments()[0];
            if (entityType != _entityType)
            {
                throw new ArgumentException("The passed expression in an EntityDemand for type: " +
                                            entityType.FullName + " but should be for: " + _entityType.FullName,
                                            "expression");
            }

            var roleType = typeof(TRole);

            if (!_roleConditions.ContainsKey(roleType))
            {
                _roleConditions[roleType] = new List<LambdaExpression>();
            }

            var replaced = ReplaceDemandWithEntityParam(expression);
            _roleConditions[roleType].Add(replaced);
        }

        public void BuildAndAddFilters(Dictionary<Type, FilterParameterProvider> roleToProvider)
        {
            foreach (var kvp in _roleConditions)
            {
                if (kvp.Value.Any())
                {
                    var filterName = FilterHelper.GetFilterName(kvp.Key);
                    var condition = GetCondition(kvp.Key, kvp.Value, roleToProvider[kvp.Key]);
                    _model.AddFilter(filterName, condition);
                }
            }
        }

        private string GetCondition(Type roleType, IEnumerable<LambdaExpression> whereExpressions, FilterParameterProvider provider)
        {
            return new FilteringExpressionToSqlVisitor(roleType, _entityType, _model, provider.GetParameterName)
                .GetCondition(whereExpressions);
        }

        private LambdaExpression ReplaceDemandWithEntityParam(LambdaExpression expression)
        {
            var entityParam = Expression.Parameter(_entityType, "Entity");
            var replacer = new ExpressionReplacer<MemberExpression>();

            var bodyAfterReplacement = replacer.Replace(expression.Body,
                                                        m =>
                                                        {
                                                            var param = m.Expression as ParameterExpression;
                                                            if (param == null)
                                                                return false;

                                                            if (param.Type != _demandType)
                                                                return false;

                                                            return m.Member.Name == "Entity";
                                                        },
                                                        m => entityParam);

            return Expression.Lambda(bodyAfterReplacement, entityParam, expression.Parameters[1]);
        }

        #region Nested type: ConstantEpressionValue

        /// <summary>
        /// Class used as a string wrapper only to make constant expressi
        /// </summary>
        private class ConstantEpressionValue
        {
            private readonly string _value;

            public ConstantEpressionValue(string value)
            {
                _value = value;
            }

            public override string ToString()
            {
                return _value;
            }
        }

        #endregion
    }
}