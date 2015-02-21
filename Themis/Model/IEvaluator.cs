using System;

namespace Themis.Model
{
    /// <summary>
    /// The non generic evaluator used by the role model.
    /// </summary>
    public interface IEvaluator
    {
        /// <summary>
        /// Gets the type of the permission handled by this evaluator
        /// </summary>
        /// <value>The type of the permission.</value>
        Type DemandType { get; }

        /// <summary>
        /// Evaluates the specified permission in the context of the role.
        /// </summary>
        /// <param name="permission">The permission.</param>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        object Evaluate(object permission, object role);
    }
}