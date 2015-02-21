using Themis.Cfg;

namespace Themis.Tests.Examples
{
    public abstract class RoleDefinitionBase<TRoleType> : RoleDefinition<TRoleType> 
        where TRoleType : class
    {
        protected void ValueIsAllowed<TValue>(TValue value)
        {
            Add<AllowedValueDemand<TValue>, TValue>((d, r) => value);
        }
    }
}