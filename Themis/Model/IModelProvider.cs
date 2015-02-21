using System.Collections.Generic;
using Themis.Impl;

namespace Themis.Model
{
    /// <summary>
    /// The interface of a class providing the role models.
    /// </summary>
    public interface IModelProvider
    {
        /// <summary>
        /// Gets the role model, creating it with <paramref name="factories"/> usage.
        /// </summary>
        /// <param name="factories">The enumerable of evaluator factories.</param>
        /// <returns>A role model.</returns>
        IRoleModel GetModel(IEnumerable<IEvaluatorFactory> factories);
    }
}