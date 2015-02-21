using System;
using System.Linq.Expressions;
using Themis.Model;

namespace Themis.Impl
{
    public interface IEvaluatorFactory
    {
        /// <summary>
        /// Gets the evalutators on the basis of the passed expression.
        /// </summary>
        /// <typeparam name="TDemand">Type of the expression demand.</typeparam>
        /// <typeparam name="TRole">Type of the expression role.</typeparam>
        /// <typeparam name="TResult">Type of the expression demand result.</typeparam>
        /// <param name="expression">The expression to be analyzed</param>
        /// <returns>An non-null array of evaluators.</returns>
        /// <remarks>
        /// The result evaluators may have its <see cref="IEvaluator.DemandType"/> different from
        /// <typeparamref name="TDemand"/>. It's up to implementor what factory produce from demands.
        /// </remarks>
        IEvaluator[] GetEvaluators<TDemand, TRole, TResult>(Expression<Func<TDemand, TRole, TResult>> expression)
            where TDemand : class, IDemand<TResult>
            where TRole : class;

        /// <summary>
        /// Called when all models were built up.
        /// </summary>
        void EndModelsBuildUp();
    }
}