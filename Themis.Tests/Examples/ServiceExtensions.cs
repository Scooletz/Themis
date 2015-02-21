namespace Themis.Tests.Examples
{
    public static class ServiceExtensions
    {
        public static TValue[] GetAllowedValues<TValue>(this IDemandService @this, params object[] roles)
        {
            return @this.Evaluate<AllowedValueDemand<TValue>, TValue>(AllowedValueDemand<TValue>.Instance, roles);
        }
    }
}