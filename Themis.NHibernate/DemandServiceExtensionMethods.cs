using NHibernate;
using Themis.NHibernate;
using Themis.NHibernate.Impl;

namespace Themis
{
    public static class DemandServiceExtensionMethods
    {
        public static IFilterScope ApplyFilters(this IDemandService service, ISession session, params object[] roles)
        {
            return new FilterScope(service, session, roles);
        }
    }
}