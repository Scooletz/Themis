namespace Themis.Claims
{
    public interface ITransformToResult
    {
        TResult[] Map<TDemand, TResult>(ReadonlyClaimPrincipal principal, TDemand demand)
            where TDemand : IDemand<TResult>;
    }
}