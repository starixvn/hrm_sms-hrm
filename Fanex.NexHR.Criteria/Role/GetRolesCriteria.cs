namespace Fanex.NexHR.Criteria.Role
{
    using Fanex.Data.Repository;

    public class GetRolesCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Role_GetRoles";

        public override bool IsValid() => true;
    }
}