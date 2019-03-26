namespace Fanex.NexHR.Criteria.Role
{
    using Fanex.Data.Repository;

    public class GetRolesByUserIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override bool IsValid()
        {
            return UserId > 0;
        }

        public override string GetSettingKey()
        {
            return "Role_GetRolesByUserId";
        }
    }
}
