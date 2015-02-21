using Themis.Tests.NHibernate.Data;

namespace Themis.Tests.NHibernate.RoleDefinitions
{
    public class SupportingManagerRoleDefinition : BaseRoleDefinition<SupportingManagerRole>
    {
        public SupportingManagerRoleDefinition()
        {
            View<RecruitmentMotion>((m, r) => m.ForUnit.Id == r.BarelyManagedUnit.Id); // all motions owned by the employee having this role
        }
    }
}