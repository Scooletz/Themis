namespace Themis
{
    /// <summary>
    /// The main interface of Themis, defining the demand, 
    /// and the result which should be provided by the system
    /// evaluating the demand.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <remarks>
    /// Implementations of IDemand should be context full, 
    /// passing all the needed context in their read-only properties. 
    /// The result can be any type, but in majority of authorization
    /// cases it is <see cref="bool"/> and then, <see cref="IClaim"/>
    /// interface should be used.
    /// </remarks>
    public interface IDemand<TResult>
    {
    }
}