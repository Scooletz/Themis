using System;
using System.Collections.Generic;
using System.Linq;
using Themis.Impl;
using Themis.Model;

namespace Themis.Cfg
{
    /// <summary>
    /// The object providing fluent configuration of an authentication service.
    /// </summary>
    /// <remarks>
    /// After configuring the authorizations, call <see cref="BuildDemandService"/> to get a 
    /// fully initialized <see cref="IDemandService"/>
    /// </remarks>
    public class FluentConfiguration
    {
        private readonly IList<IEvaluatorFactory> _factories;
        private readonly Dictionary<Type, IModelProvider> _modelProviders;

        internal FluentConfiguration()
        {
            _modelProviders = new Dictionary<Type, IModelProvider>();
            _factories = new List<IEvaluatorFactory> {new DefaultEvaluatorFactory()};
        }

        /// <summary>
        /// Adds the evaluator factory, used to build the evaluators 
        /// during <see cref="BuildDemandService"/> call.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <returns>This for method chaining.</returns>
        public FluentConfiguration AddEvaluatorFactory(IEvaluatorFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            _factories.Add(factory);

            return this;
        }

        /// <summary>
        /// Adds the role definition to the configuration.
        /// </summary>
        /// <typeparam name="TRole"></typeparam>
        /// <param name="roleDefinition"></param>
        /// <returns></returns>
        /// <remarks>
        /// Only one <see cref="IModelProvider"/> can be set per role type.
        /// </remarks>
        public FluentConfiguration AddRoleDefinition<TRole>(RoleDefinition<TRole> roleDefinition)
            where TRole : class
        {
            var key = typeof (TRole);
            if (_modelProviders.ContainsKey(key))
                throw new InvalidOperationException("The configuration already contains an entry for role: " +
                                                    key.FullName);

            _modelProviders[key] = roleDefinition;

            return this;
        }

        /// <summary>
        /// Builds the demand service configured with this.
        /// </summary>
        /// <returns>A newly created demand service.</returns>
        public IDemandService BuildDemandService()
        {
            var roleModels = BuildRoleModels(_modelProviders.Select(kvp => kvp.Value), _factories);

            foreach (var factory in _factories)
            {
                factory.EndModelsBuildUp();
            }

            return new DemandService(roleModels);
        }

        protected virtual IRoleModel[] BuildRoleModels(IEnumerable<IModelProvider> modelProviders,
                                                                  IEnumerable<IEvaluatorFactory> factories)
        {
            return modelProviders.Select(m => m.GetModel(factories)).ToArray();
        }
    }
}