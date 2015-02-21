using System;
using Themis.Model;

namespace Themis.Impl
{
    /// <summary>
    /// The evaluator wrapping any <see cref="Func{T1,T2,TResult}"/>
    /// to a non generic <see cref="IEvaluator"/>.
    /// </summary>
    /// <typeparam name="TDemand">The type of the permission.</typeparam>
    /// <typeparam name="TRole">The type of the role.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class Evaluator<TDemand, TRole, TResult> : IEvaluator
        where TDemand : IDemand<TResult>
        where TRole : class
    {
        private readonly Func<TDemand, TRole, TResult> _function;

        public Evaluator(Func<TDemand, TRole, TResult> function)
        {
            _function = function;
        }

        #region IEvaluator Members

        public object Evaluate(object permission, object role)
        {
            return _function((TDemand) permission, (TRole) role);
        }

        public Type DemandType
        {
            get { return typeof (TDemand); }
        }

        #endregion
    }
}