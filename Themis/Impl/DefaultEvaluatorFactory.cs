using System;
using System.Linq.Expressions;
using Themis.Model;

namespace Themis.Impl
{
    /// <summary>
    /// The default evaluator factory, creating the evaluator by compiling the expression
    /// </summary>
    public class DefaultEvaluatorFactory : IEvaluatorFactory
    {
        #region IEvaluatorFactory Members

        public IEvaluator[] GetEvaluators<TDemand, TRole, TResult>(
            Expression<Func<TDemand, TRole, TResult>> expression)
            where TDemand : class, IDemand<TResult>
            where TRole : class
        {
            return new IEvaluator[] {new Evaluator<TDemand, TRole, TResult>(expression.Compile())};
        }

        public void EndModelsBuildUp()
        {
        }

        #endregion
    }
}