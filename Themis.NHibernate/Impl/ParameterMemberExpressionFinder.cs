using System.Collections.Generic;
using System.Linq.Expressions;

namespace Themis.NHibernate.Impl
{
    /// <summary>
    /// The class helping to find member expressions which finally access the parameter of a specific type.
    /// </summary>
    public sealed class ParameterMemberExpressionFinder : ExpressionVisitor
    {
        private readonly List<MemberExpression> _memberAccessExpressions;
        private readonly ParameterExpression _parameter;

        /// <summary>
        /// The type of the expression.
        /// </summary>
        /// <param name="parameter">The parameter type.</param>
        public ParameterMemberExpressionFinder(ParameterExpression parameter)
        {
            _memberAccessExpressions = new List<MemberExpression>();
            _parameter = parameter;
        }

        public IEnumerable<MemberExpression> FindExpressionsIn(Expression expression)
        {
            Visit(expression);
            var result = _memberAccessExpressions.ToArray();
            _memberAccessExpressions.Clear();
            return result;
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            DeepSearch(m);
            return m;
        }

        private void DeepSearch(MemberExpression m)
        {
            var temp = m.Expression;

            while (temp is MemberExpression)
            {
                temp = ((MemberExpression) temp).Expression;
            }

            var param = temp as ParameterExpression;
            if (param == null)
                return;

            if (param.Type == _parameter.Type && param.Name == _parameter.Name)
            {
                _memberAccessExpressions.Add(m);
            }
        }
    }
}