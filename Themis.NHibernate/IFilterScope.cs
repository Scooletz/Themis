using System;

namespace Themis.NHibernate
{
    /// <summary>
    /// The disposable object representing a scope in which the session is affected by Themis filters.
    /// </summary>
    /// <remarks>
    /// The filtering is stopped as soon as the filtering scope is disposed.
    /// </remarks>
    public interface IFilterScope : IDisposable
    {
    }
}