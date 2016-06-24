namespace Nexus.RoleManagement.Services
{
    using Fanex.Data.Repository;

    public class GetRolesCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Role_GetRoles";
        }
    }
}
