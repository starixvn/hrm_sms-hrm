namespace GO.WebAPI.Criteria.Role
{
    using Fanex.Data.Repository;

    public class GetRoleByUserIdCriteria : CriteriaBase
    {
        public short SiteId { get; set; }
        public int UserId { get; set; }

        public override string GetSettingKey() => "API_Role_GetByUserId";

        public override bool IsValid() => SiteId > 0 && UserId > 0;
    }
}