using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;
using NHibernate.Impl;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace Themis.NHibernate.Util
{
    /// <summary>
    /// The filter helper, based on the NHibernate filter helper, 
    /// augmented to handle Themis fitlers.
    /// </summary>
    public sealed class FilterHelper
    {
        /// <summary>
        /// The preffix for filters used by Themis. Do not use it in your filters.
        /// </summary>
        public const string ThemisFilterPreffix = "Themis_";

        private readonly string[] _filterConditions;

        private readonly string[] _filterNames;

        public FilterHelper(ICollection<KeyValuePair<string, string>> filters, Dialect dialect,
                            SQLFunctionRegistry sqlFunctionRegistry)
        {
            var filterCount = filters.Count;
            _filterNames = new string[filterCount];
            _filterConditions = new string[filterCount];
            filterCount = 0;
            foreach (var entry in filters)
            {
                _filterNames[filterCount] = entry.Key;
                _filterConditions[filterCount] = Template.RenderWhereStringTemplate(entry.Value, FilterImpl.MARKER,
                                                                                    dialect, sqlFunctionRegistry);
                _filterConditions[filterCount] = StringHelper.Replace(_filterConditions[filterCount], ":",
                                                                      ":" + _filterNames[filterCount] + ".");
                filterCount++;
            }
        }

        public static bool ContainsAnyThemisFilter(Configuration cfg)
        {
            return cfg.FilterDefinitions.Any(kvp => IsThemisFilter(kvp.Key));
        }

        public static bool IsThemisFilter(string filterName)
        {
            return filterName.StartsWith(ThemisFilterPreffix, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string GetFilterName(Type roleType)
        {
            return ThemisFilterPreffix + roleType.Name;
        }

        public string FilterFragment(string alias, IDictionary<string, IFilter> enabledFilters,
                                     AbstractEntityPersister persister)
        {
            var sessionFilterFragment = new StringBuilder();
            Render(sessionFilterFragment, persister.GenerateFilterConditionAlias(alias), enabledFilters);
            return sessionFilterFragment.Append(persister.FilterFragment(alias)).ToString();
        }

        public void Render(StringBuilder buffer, string alias, IDictionary<string, IFilter> enabledFilters)
        {
            if ((_filterNames == null) || (_filterNames.Length <= 0))
            {
                return;
            }

            List<string> themisFilters = null;

            var i = 0;
            var max = _filterNames.Length;
            while (i < max)
            {
                var filterName = _filterNames[i];

                if (enabledFilters.ContainsKey(filterName))
                {
                    var condition = _filterConditions[i];
                    if (StringHelper.IsNotEmpty(condition))
                    {
                        if (IsThemisFilter(filterName))
                        {
                            if (themisFilters == null)
                            {
                                themisFilters = new List<string>();
                            }
                            themisFilters.Add(StringHelper.Replace(condition, FilterImpl.MARKER, alias));
                        }
                        else
                        {
                            buffer.Append(" and ");
                            buffer.Append(StringHelper.Replace(condition, FilterImpl.MARKER, alias));
                        }
                    }
                }
                i++;
            }

            if (themisFilters != null)
            {
                var oredFilters = string.Join(" or ", themisFilters.ToArray());
                buffer.Append(" and (");
                buffer.Append(oredFilters);
                buffer.Append(")");
            }
        }
    }
}