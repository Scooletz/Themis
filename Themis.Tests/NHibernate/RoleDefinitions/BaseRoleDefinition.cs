using System;
using System.Linq.Expressions;
using Themis.Cfg;
using Themis.NHibernate;
using Themis.Tests.NHibernate.Demands;
using Themis.Utils.Expressions;

namespace Themis.Tests.NHibernate.RoleDefinitions
{
    public class BaseRoleDefinition<TRole> : RoleDefinition<TRole> where TRole : class
    {
        protected void View<TEntity>(Expression<Func<TEntity, TRole, bool>> expression)
            where TEntity : class
        {
            Expression<Func<ViewDemand<TEntity>, TEntity>> accessor = v => v.Entity;
            var result = ExpressionHelper.MapDemandMemberToDemand(expression, accessor);
            
            Add(result);
            
            Expression<Func<EntityDemand<TEntity>, TEntity>> nhAccessor = v => v.Entity;
            var nhEntityDemand = ExpressionHelper.MapDemandMemberToDemand(expression, nhAccessor);
            
            Add(nhEntityDemand);
        }
    }
}