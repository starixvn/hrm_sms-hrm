namespace HRM.UserControl.WebAPI.Criteria.Role
{
    using Fanex.Data.Repository;

    public class GetRoleBySiteIdCriteria : CriteriaBase
    {
        public short SiteId { get; set; }
        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "API_Role_GetBySiteId";

        public override bool IsValid() => SiteId > 0;
    }
}