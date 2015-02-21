using Themis.Cfg;

namespace Themis
{
    /// <summary>
    /// The service build by <see cref="FluentConfiguration"/> used to evaluate demands.
    /// </summary>
    public interface IDemandService
    {
        /// <summary>
        /// Evaluates the passed demand in the context of passed roles.
        /// </summary>
        /// <typeparam name="TDemand">The demand type.</typeparam>
        /// <typeparam name="TResult">The demand evaluation result.</typeparam>
        /// <param name="demand">The demand object.</param>
        /// <param name="roles">The roles.</param>
        /// <returns>An array of evaluated result objects.</returns>
        TResult[] Evaluate<TDemand, TResult>(TDemand demand, params object[] roles)
            where TDemand : class, IDemand<TResult>;

        /// <summary>
        /// Evaluates the passed <see cref="claim"/> in the context of passed roles.
        /// </summary>
        /// <typeparam name="TClaim">The type of the claim.</typeparam>
        /// <param name="claim">The claim instance.</param>
        /// <param name="roles">The roles.</param>
        /// <returns>An array of evaluated result objects.</returns>
        bool[] Evaluate<TClaim>(TClaim claim, params object[] roles)
            where TClaim : class, IClaim;

        /// <summary>
        /// Checks whether there are any evaluators registered in the service for the specified demand type <see cref="TDemand"/>
        /// for the given roles <see cref="roles"/>.
        /// </summary>
        /// <typeparam name="TDemand">The type of the demand.</typeparam>
        /// <typeparam name="TResult">The result of the demand.</typeparam>
        /// <param name="roles">The roles.</param>
        /// <returns>Whether this service instance has any evaluators for the given conditions.</returns>
        /// <remarks>
        /// This method can be widely used to determine whether one has any chance of getting at least one demand result.
        /// One of the scenarios, where it can be used is checking whether a user can access a view of list of entities at all.
        /// He/she is allowed to do this iff an example ViewEntity is defined for one of the roles. If so, the method returns
        /// true; otherwise - false.
        /// </remarks>
        bool HasAnyEvaluators<TDemand, TResult>(params object[] roles)
            where TDemand : class, IDemand<TResult>;
    }
}