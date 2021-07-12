namespace HRM.UserControl.API.Core.Criteria
{
    using Fanex.Data.Repository;

    public class GetRoleBySiteIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public int SiteId { get; set; }

        public override string GetSettingKey() => "API_UserRole_GetBySiteId";

        public override bool IsValid() => UserId > 0 && SiteId > 0;
    }
}