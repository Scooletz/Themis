using System;
using System.Linq.Expressions;
using Themis.Cfg;

namespace Themis.Expressions
{
    /// <summary>
    /// Provides methods for variety of expression operations,
    /// allowing writing more programmer friendly <see cref="RoleDefinition{TRole}"/>
    /// implementations.
    /// </summary>
    public static class ExpressionHelper
    {
        /// <summary>
        /// Maps the expression operating onto a demand member to a demand operating expression.
        /// </summary>
        /// <typeparam name="TDemand">The type of the demand.</typeparam>
        /// <typeparam name="TRole">The type of the role.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TDemandMemberType">The type of the demand member.</typeparam>
        /// <param name="demandMemberExpression">The demand member expression.</param>
        /// <param name="demendMemberAccessor">The expression describing accessing the member of demand.</param>
        /// <returns>The expression operating on a demand, providing the same</returns>
        public static Expression<Func<TDemand, TRole, TResult>> MapDemandMemberToDemand
            <TDemand, TRole, TResult, TDemandMemberType>(
            Expression<Func<TDemandMemberType, TRole, TResult>> demandMemberExpression,
            Expression<Func<TDemand, TDemandMemberType>> demendMemberAccessor)
            where TDemand : IDemand<TResult>
        {
            var replacer = new ExpressionReplacer<ParameterExpression>();
            var bodyAfterReplacement = replacer.Replace(demandMemberExpression.Body,
                                                        p => p.Type == typeof (TDemandMemberType),
                                                        p => demendMemberAccessor.Body);
            return Expression.Lambda<Func<TDemand, TRole, TResult>>(bodyAfterReplacement,
                                                                    demendMemberAccessor.Parameters[0],
                                                                    demandMemberExpression.Parameters[1]);
        }
    }
}