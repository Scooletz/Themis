using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Cfg;
using NHibernate.Engine;
using Themis.Model;
using Themis.NHibernate.Util;

namespace Themis.NHibernate.Impl
{
    public class FilteringDemandEvaluator : IEvaluator
    {
        private readonly List<MemberExpression> _expressions;
        private readonly string _filterName;
        private readonly Type _roleType;
        private FilterParameterProvider _provider;

        public FilteringDemandEvaluator(Type roleType)
        {
            _filterName = FilterHelper.GetFilterName(roleType);
            _roleType = roleType;
            _expressions = new List<MemberExpression>();
        }

        #region IEvaluator Members

        public object Evaluate(object permission, object role)
        {
            if (!(permission is FilteringDemand))
                throw new ArgumentException("The permission should be of FilteringDemand type.");

            return Evaluate(role);
        }

        public Type DemandType
        {
            get { return typeof (FilteringDemand); }
        }

        public FilterParameterProvider Provider
        {
            get { return _provider; }
        }

        #endregion

        public void AddRoleMemberAccessExpressions(IEnumerable<MemberExpression> roleExpressions)
        {
            _expressions.AddRange(roleExpressions);
        }

        public FilterParameterProvider Seal(Configuration cfg)
        {
            _provider = new FilterParameterProvider(_roleType, _expressions);
            var def = new FilterDefinition(_filterName, string.Empty, Provider.GetFilterParameters(), false);
            cfg.FilterDefinitions.Add(_filterName, def);
            return _provider;
        }

        private FilterApplier Evaluate(object role)
        {
            return new FilterApplier(_filterName, Provider.GetFilterParametersValues(role));
        }
    }
}