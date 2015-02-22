using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Type;
using Themis.Expressions;

namespace Themis.NHibernate.Impl
{
    /// <summary>
    /// The class handling the NHibernate filter parameters.
    /// </summary>
    public class FilterParameterProvider
    {
        private readonly List<Expression> _expressions;
        private readonly ParameterExpression _roleParameterExpression;
        private Func<object, object>[] _paramGetters;
        private string[] _paramNames;

        public FilterParameterProvider(Type roleType, IEnumerable<MemberExpression> roleMemberExpressions)
        {
            RoleType = roleType;
            _expressions = new List<Expression>();
            _roleParameterExpression = Expression.Parameter(RoleType, RoleType.Name);

            foreach (var e in roleMemberExpressions)
            {
                _expressions.Add(SanitizeParameterName(e));
            }
        }

        public Type RoleType { get; private set; }

        public string GetParameterName(MemberExpression m)
        {
            var e = SanitizeParameterName(m);
            return GetNameFromSanitizedExpression(e);
        }

        /// <summary>
        /// Gets the filter parameters for the role.
        /// </summary>
        /// <returns>A dictionary of string-IType pairs.</returns>
        /// <remarks>
        /// The names of the parameters are parsed member access expressions.
        /// The order of filter parameters is the same as order of member expressions passed to the contructor (skipping the doubles).
        /// </remarks>
        public IDictionary<string, IType> GetFilterParameters()
        {
            var result = new Dictionary<string, IType>();
            var filterParameterGetters = new Dictionary<string, Func<object, object>>();

            foreach (var e in _expressions)
            {
                var exprName = GetNameFromSanitizedExpression(e);

                if (result.ContainsKey(exprName))
                {
                    continue;
                }

                if (!e.Type.IsPrimitive && e.Type != typeof(Guid))
                {
                    throw new InvalidOperationException(
                        string.Format("Currently Themis handles only primitive types used in role expressions. " +
                                      "The expression causing an exeption is: {0}", exprName));
                }

                var guessedType = NHibernateUtil.GuessType(e.Type);

                result[exprName] = guessedType;
                filterParameterGetters[exprName] = GenerateGetter(e);
            }

            _paramNames = filterParameterGetters.Select(kvp => kvp.Key).ToArray();
            _paramGetters = filterParameterGetters.Select(kvp => kvp.Value).ToArray();

            return result;
        }

        private static string GetNameFromSanitizedExpression(Expression e)
        {
            return e.ToString().Replace(".", "_");
        }

        private Func<object, object> GenerateGetter(Expression expression)
        {
            var objectParam = Expression.Parameter(typeof(object), _roleParameterExpression.Name);

            var replacer = new ExpressionReplacer<ParameterExpression>();
            var expressionWithObjectParameter = replacer.Replace(
                expression,
                p => p.Type == RoleType,
                p => Expression.Convert(objectParam, RoleType));

            var castedExpressionWithObjectParameter = Expression.Convert(expressionWithObjectParameter, typeof(object));

            var lambda = Expression.Lambda<Func<object, object>>(castedExpressionWithObjectParameter, objectParam);
            return lambda.Compile();
        }

        public IDictionary<string, object> GetFilterParametersValues(object role)
        {
            if (role.GetType() != RoleType)
            {
                throw new ArgumentException("The passed role has type different from the original", "role");
            }

            var values = new Dictionary<string, object>(_paramNames.Length);
            for (var i = 0; i < _paramNames.Length; i++)
            {
                values[_paramNames[i]] = _paramGetters[i](role);
            }

            return values;
        }

        private Expression SanitizeParameterName(Expression e)
        {
            var replacer = new ExpressionReplacer<ParameterExpression>();
            return replacer.Replace(e, p => p.Type == RoleType, p => _roleParameterExpression);
        }
    }
}