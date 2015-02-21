namespace Themis
{
    /// <summary>
    /// The <see cref="IDemandService"/> extension methods.
    /// </summary>
    public static class DemandServiceExtensions
    {
        /// <summary>
        /// The extension method, overriding <see cref="IDemandService.HasAnyEvaluators{TDemand,TResult}"/>
        /// for claims. <seealso cref="IDemandService.HasAnyEvaluators{TDemand,TResult}"/> for more information.
        /// </summary>
        /// <typeparam name="TClaim">The claim type.</typeparam>
        /// <param name="this">The service instance.</param>
        /// <param name="roles">The roles.</param>
        /// <returns><seealso cref="IDemandService.HasAnyEvaluators{TDemand,TResult}"/> for more information.</returns>
        public static bool HasAnyEvaluators<TClaim>(this IDemandService @this, params object[] roles)
            where TClaim : class, IClaim
        {
            return @this.HasAnyEvaluators<TClaim, bool>(roles);
        }
    }
}