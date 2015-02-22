namespace Themis
{
    /// <summary>
    /// Demands represents queries against the Themis, to provide the result of type <typeparamref name="TResult"/>
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <remarks>
    /// </remarks>
    public interface IDemand<TResult>
    {
    }

    public interface IDemand : IDemand<bool>
    { }
}