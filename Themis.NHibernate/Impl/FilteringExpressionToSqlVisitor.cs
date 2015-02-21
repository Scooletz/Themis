using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Mapping;

namespace Themis.NHibernate.Impl
{
    /// <summary>
    /// The expression visitor, which for a specified pair: entity type and role type,
    /// provides the compounded sql condition.
    /// </summary>
    internal class FilteringExpressionToSqlVisitor : ExpressionVisitor
    {
        private readonly Type _entityType;
        private readonly PersistentClass _model;
        private readonly Func<MemberExpression, string> _roleParameterNameGetter;
        private readonly Type _roleType;
        private List<string> _partialConditions;

        public FilteringExpressionToSqlVisitor(Type roleType, Type entityType,
            PersistentClass model, Func<MemberExpression, string> roleParameterNameGetter)
        {
            _roleType = roleType;
            _entityType = entityType;
            _model = model;
            _roleParameterNameGetter = roleParameterNameGetter;
        }

        /// <summary>
        /// Gets the compounded, or-connected condition for the expressions passed as a paramter.
        /// </summary>
        /// <param name="expressions">The bool returning expressions having an entity type and role type as parameters.</param>
        /// <returns></returns>
        public string GetCondition(IEnumerable<LambdaExpression> expressions)
        {
            _partialConditions = new List<string>();

            foreach (var e in expressions)
            {
                _partialConditions.Add(Visit(e).ToString());
            }

            if (_partialConditions.Count == 1)
                return _partialConditions[0];

            var conditions = _partialConditions.Select(WrapWithBrackets).ToArray();
            var result = string.Join(" OR ", conditions);
            return WrapWithBrackets(result);
        }

        private static string WrapWithBrackets(string result)
        {
            return "(" + result + ")";
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            if (u.NodeType != ExpressionType.Not)
            {
                throw new ArgumentException("Currently only NOT unary operator is handled", "u");
            }

            return Expression.Constant(new ConstantExpressionValue("NOT (" + Visit(u.Operand) + ")"));
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            switch (b.NodeType)
            {
                case ExpressionType.LessThan:
                    return VisitBinary(b, "<");
                case ExpressionType.LessThanOrEqual:
                    return VisitBinary(b, "<=");
                case ExpressionType.GreaterThan:
                    return VisitBinary(b, ">");
                case ExpressionType.GreaterThanOrEqual:
                    return VisitBinary(b, ">=");
                case ExpressionType.Equal:
                    return VisitBinary(b, "=");
                case ExpressionType.NotEqual:
                    return VisitBinary(b, "<>");
                default:
                    throw new ArgumentException("Currently only comperator expressions are handled", "b");
            }
        }

        protected override Expression VisitLambda<T>(Expression<T> lambda)
        {
            return Visit(lambda.Body);
        }

        protected override Expression VisitInvocation(InvocationExpression iv)
        {
            throw new ArgumentException("Visiting InvocationExpression is not handled");
        }

        protected override Expression VisitListInit(ListInitExpression init)
        {
            throw new ArgumentException("Visiting ListInitExpression is not handled");
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            throw new ArgumentException("Visiting MemberAssignment is not handled");
        }

        protected override Expression VisitMemberInit(MemberInitExpression init)
        {
            throw new ArgumentException("Visiting MemberInitExpression is not handled");
        }

        protected override MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            throw new ArgumentException("Visiting MemberListBinding is not handled");
        }

        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            throw new ArgumentException("Visiting MemberMemberBinding is not handled");
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            throw new ArgumentException("Visiting MethodCallExpression is not handled");
        }

        protected override Expression VisitNew(NewExpression node)
        {
            throw new ArgumentException("Visiting NewExpression is not handled");
        }

        protected override Expression VisitNewArray(NewArrayExpression na)
        {
            throw new ArgumentException("Visiting NewArrayExpression is not handled");
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            throw new ArgumentException(
                "Visiting ParameterExpression is not handled. There should be no situation where Themis process a parameter expression.");
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            var temp = m.Expression;

            while (temp is MemberExpression)
            {
                temp = ((MemberExpression)temp).Expression;
            }

            var param = temp as ParameterExpression;
            if (param == null)
            {
                throw new ArgumentException("Themis cannot establish for which paramter the passed expression was used");
            }

            if (param.Type == _entityType)
            {
                return ProcessEntityExpression(m);
            }
            if (param.Type == _roleType)
            {
                return ProcessRoleExpression(m);
            }

            throw new ArgumentException("The passed member expression is neither entity or role expression", "m");
        }

        private Expression ProcessEntityExpression(MemberExpression m)
        {
            var isSimpleProperty = m.Expression is ParameterExpression;

            if (isSimpleProperty)
            {
                var property = _model.GetProperty(m.Member.Name);

                return ProcessEntitySimpleProperty(property);
            }

            var mayBeToOne = m.Expression is MemberExpression;
            if (mayBeToOne)
            {
                var member = (MemberExpression)m.Expression;
                var property = _model.GetProperty(member.Member.Name);

                var entityPropInfo = string.Format("Entity type: {0}, property name: {1}", _entityType.FullName,
                                                   property.Name);

                if (!(property.Value is ToOne))
                {
                    throw new InvalidOperationException("Only to one mappings are considered. " + entityPropInfo);
                }

                var dbField = property.Value.ColumnIterator.First().Text;
                return Expression.Constant(new ConstantExpressionValue(dbField));
            }

            throw new ArgumentException("The member expression " + m + " cannot be processed by Themis.");
        }

        private Expression ProcessEntitySimpleProperty(Property property)
        {
            var entityPropInfo = string.Format("Entity type: {0}, property name: {1}", _entityType.FullName,
                                               property.Name);

            if (property.IsComposite)
            {
                throw new InvalidOperationException("Composite columns are not handled by Themis. " +
                                                    entityPropInfo);
            }

            if (property.Value.GetType() == typeof(SimpleValue))
            {
                var dbField = property.Value.ColumnIterator.First().Text;
                return Expression.Constant(new ConstantExpressionValue(dbField));
            }

            throw new InvalidOperationException("The equality can be done only on the SimpleValue property. " +
                                                entityPropInfo);
        }

        private Expression ProcessRoleExpression(MemberExpression memberExpression)
        {
            return
                Expression.Constant(
                    new ConstantExpressionValue(":" + _roleParameterNameGetter(memberExpression)));
        }

        private ConstantExpression VisitBinary(BinaryExpression b, string op)
        {
            return Expression.Constant(new ConstantExpressionValue("(" + Visit(b.Left) + op + Visit(b.Right) + ")"));
        }

        #region Nested type: ConstantExpressionValue

        private class ConstantExpressionValue
        {
            private readonly string _value;

            public ConstantExpressionValue(string value)
            {
                _value = value;
            }

            public override string ToString()
            {
                return _value;
            }
        }

        #endregion
    }
}