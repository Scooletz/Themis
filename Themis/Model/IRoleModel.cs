using System;

namespace Themis.Model
{
    /// <summary>
    /// The interface of a role model provding the evaluation method
    /// as well as the property describing the role type.
    /// </summary>
    public interface IRoleModel
    {
        /// <summary>
        /// Gets the type of the role handled by this model.
        /// </summary>
        /// <value>The type of the role.</value>
        Type RoleType { get; }

        /// <summary>
        /// Evaluates the specified demand in the context of the role object.
        /// </summary>
        /// <param name="demand">The demand.</param>
        /// <param name="role">The role.</param>
        /// <returns>An array of evaluation results.</returns>
        object[] Evaluate(object demand, object role);

        /// <summary>
        /// Gets the count of evaluators of the specific 
        /// demand.
        /// </summary>
        /// <param name="demandType">The type of the demand.</param>
        /// <returns>A number of evaluators, 0, if none.</returns>
        int GetEvaluatorsCount(Type demandType);
    }
}