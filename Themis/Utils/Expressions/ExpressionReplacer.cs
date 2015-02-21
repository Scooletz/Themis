using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Themis.Utils.Expressions
{
    /// <summary>
    /// A class that is capable of doing a find and replace in an <see cref="Expression"/> tree.
    /// </summary>
    /// <typeparam name="TExpression">The type of <see cref="Expression"/> to find and replace.</typeparam>
    /// <remarks>
    /// The implementation taken from: <see cref="http://bernhard-richter.blogspot.com/2010/02/implementing-find-and-replace-for.html"/>.
    /// </remarks>
    [DebuggerStepThrough]
    public class ExpressionReplacer<TExpression> : ExpressionVisitor
        where TExpression : Expression
    {
        private Func<TExpression, bool> _predicate;
        private Func<TExpression, Expression> _replaceWith;

        /// <summary>
        /// Searches for expressions using the given <paramref name="predicate"/> and 
        /// replaces matches with the result from the <paramref name="replaceWith"/> delegate.          
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> that 
        /// represents the sub tree for which to start searching.</param>
        /// <param name="predicate">The <see cref="Func{T,TResult}"/> used to filter the result</param>
        /// <param name="replaceWith">The <see cref="Func{T,TResult}"/> 
        /// used to specify the replacement expression.</param>
        /// <returns>The modified <see cref="Expression"/> tree.</returns>
        public Expression Replace(Expression expression, Func<TExpression, bool> predicate,
                                  Func<TExpression, Expression> replaceWith)
        {
            _replaceWith = replaceWith;
            _predicate = predicate;
            return Visit(expression);
        }

        /// <summary>
        /// Visits each node of the <see cref="Expression"/> tree checks 
        /// if the current expression matches the predicate. If a match is found 
        /// the expression will be replaced.        
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> currently being visited.</param>
        /// <returns><see cref="Expression"/></returns>        
        public override Expression Visit(Expression expression)
        {
            if (expression is TExpression)
            {
                if (_predicate((TExpression) expression))
                    return _replaceWith((TExpression) expression);
            }
            return base.Visit(expression);
        }
    }
}