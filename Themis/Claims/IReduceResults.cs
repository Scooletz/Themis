namespace Themis.Claims
{
    /// <summary>
    /// The interface providing reduce function for results.
    /// </summary>
    public interface IReduceResults
    {
        TResult Reduce<TDemand, TResult>(ReadonlyClaimPrincipal principal, TDemand demand, TResult[] results)
            where TDemand : IDemand<TResult>;
    }
}