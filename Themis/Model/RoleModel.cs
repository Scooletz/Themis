using System;
using System.Collections.Generic;
using System.Linq;

namespace Themis.Model
{
    /// <summary>
    /// The model wrapping all the evaluators for the <typeparamref name="TRole"/>.
    /// </summary>
    /// <typeparam name="TRole">The type of the role.</typeparam>
    public class RoleModel<TRole> : IRoleModel
        where TRole : class
    {
        private static readonly object[] EmptyObjectArray = new object[0];
        private readonly Dictionary<Type, IEvaluator[]> _evaluators;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleModel&lt;TRole&gt;"/> class.
        /// </summary>
        /// <param name="evaluators">The evaluators provided for this role.</param>
        public RoleModel(IEnumerable<IEvaluator> evaluators)
        {
            var temp = new Dictionary<Type, List<IEvaluator>>();

            foreach (var evaluator in evaluators)
            {
                var permissionType = evaluator.DemandType;
                if (!temp.ContainsKey(permissionType))
                    temp[permissionType] = new List<IEvaluator>();
                temp[permissionType].Add(evaluator);
            }

            _evaluators = temp.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray());
        }

        #region IRoleModel Members

        public object[] Evaluate(object demand, object role)
        {
            if (role == null)
                throw new ArgumentNullException("role");
            if (demand == null)
                throw new ArgumentNullException("demand");
            if (role.GetType() != typeof (TRole))
                throw new ArgumentException("The passed role is not type of " + typeof (TRole).FullName);

            return Evaluate(demand, (TRole) role);
        }

        public int GetEvaluatorsCount(Type demandType)
        {
            if (demandType == null)
                throw new ArgumentNullException("demandType");

            IEvaluator[] evaluators;
            if (_evaluators.TryGetValue(demandType, out evaluators))
                return evaluators.Length;

            return 0;
        }

        public Type RoleType
        {
            get { return typeof (TRole); }
        }

        #endregion

        public object[] Evaluate(object demand, TRole role)
        {
            if (role == null)
                throw new ArgumentNullException("role");
            if (demand == null)
                throw new ArgumentNullException("demand");

            var permissionType = demand.GetType();
            IEvaluator[] evaluators;
            if (_evaluators.TryGetValue(permissionType, out evaluators))
            {
                var result = new object[evaluators.Length];

                for (var i = 0; i < evaluators.Length; i++)
                {
                    result[i] = evaluators[i].Evaluate(demand, role);
                }

                return result;
            }

            return EmptyObjectArray;
        }

        public IEnumerable<TResult> Evaluate<TDemand, TResult>(TDemand permission, TRole role)
            where TDemand : IDemand<TResult>
        {
            return Evaluate(permission, role).Cast<TResult>();
        }
    }
}