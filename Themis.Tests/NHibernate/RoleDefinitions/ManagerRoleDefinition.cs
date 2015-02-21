using Themis.Tests.NHibernate.Data;

namespace Themis.Tests.NHibernate.RoleDefinitions
{
    public class ManagerRoleDefinition : BaseRoleDefinition<ManagerRole>
    {
        public ManagerRoleDefinition()
        {
            View<RecruitmentMotion>((m, r) => m.ForUnit.Id == r.ManagedUnit.Id); // all motions for manager's unit
            View<RecruitmentMotion>((m, r) => m.Owner.Id == r.ForEmployee.Id); // all motions owned by the employee having this role
        }
    }
}