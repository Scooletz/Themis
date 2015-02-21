using System;
using System.Collections.Generic;
using System.Linq;
using Themis.Model;

namespace Themis.Impl
{
    /// <summary>
    /// The implementation of <see cref="IDemandService"/> being
    /// a simple wrapper around the enumeration of <see cref="IRoleModel"/>.
    /// </summary>
    public class DemandService : IDemandService
    {
        private readonly Dictionary<Type, IRoleModel> _evaluators;

        public DemandService(IEnumerable<IRoleModel> evaluators)
        {
            _evaluators = evaluators.ToDictionary(m => m.RoleType, m => m);
        }

        #region IDemandService Members

        /// <summary>
        /// Evaluates the specified demand.
        /// </summary>
        /// <typeparam name="TDemand">The type of the demand.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="demand">The demand.</param>
        /// <param name="roles">The roles.</param>
        /// <returns>An array of results.</returns>
        /// <remarks>
        /// The method returns an array of results. The roles are evaluated in an 
        /// order of passing.
        /// </remarks>
        public TResult[] Evaluate<TDemand, TResult>(TDemand demand, params object[] roles)
            where TDemand : class, IDemand<TResult>
        {
            return EvaluateImpl<TDemand, TResult>(demand, roles);
        }

        /// <summary>
        /// Evaluates the specified claim.
        /// </summary>
        /// <typeparam name="TClaim">The type of the claim.</typeparam>
        /// <param name="claim">The claim.</param>
        /// <param name="roles">The roles.</param>
        /// <returns>An array of bools.</returns>
        /// <remarks>
        /// The method returns an array of results. The roles are evaluated in an order of passing.
        /// </remarks>
        public bool[] Evaluate<TClaim>(TClaim claim, params object[] roles)
            where TClaim : class, IClaim
        {
            return EvaluateImpl<TClaim, bool>(claim, roles);
        }

        public bool HasAnyEvaluators<TDemand, TResult>(params object[] roles) 
            where TDemand : class, IDemand<TResult>
        {
            foreach (var t in roles)
            {
                var key = t.GetType();
                IRoleModel evaluator;
                if (_evaluators.TryGetValue(key,out evaluator) && evaluator.GetEvaluatorsCount(typeof(TDemand)) > 0)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        private TResult[] EvaluateImpl<TDemand, TResult>(TDemand demand, object[] roles)
            where TDemand : class, IDemand<TResult>
        {
            var result = new List<TResult>();

            foreach (var role in roles)
            {
                var key = role.GetType();
                IRoleModel evaluator;
                if (_evaluators.TryGetValue(key, out evaluator))
                {
                    var values = evaluator.Evaluate(demand, role);

                    result.Capacity = Math.Max(result.Capacity, result.Count + values.Length);

                    for (var i = 0; i < values.Length; i++)
                    {
                        result.Add((TResult) values[i]);
                    }
                }
            }

            return result.ToArray();
        }
    }
}