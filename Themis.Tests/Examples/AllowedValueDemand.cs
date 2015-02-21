namespace Themis.Tests.Examples
{
    /// <summary>
    /// The demand representing an allowed value.
    /// </summary>
    /// <typeparam name="TValue">The value which should be allowed for a specific role.</typeparam>
    internal sealed class AllowedValueDemand<TValue> : IDemand<TValue>
    {
        public static readonly AllowedValueDemand<TValue> Instance = new AllowedValueDemand<TValue>();
    }
}