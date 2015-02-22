using System;
using NHibernate;
using Themis.NHibernate.Impl;

// ReSharper disable once CheckNamespace
namespace Themis
{
    public static class DemandServiceExtensionMethods
    {
        public static IDisposable ApplyFilters(this IDemandService service, ISession session, params object[] roles)
        {
            return new FilterScope(service, session, roles);
        }
    }
}