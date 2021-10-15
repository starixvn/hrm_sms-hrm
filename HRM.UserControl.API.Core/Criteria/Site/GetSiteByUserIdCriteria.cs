namespace HRM.UserControl.API.Core.Criteria.Site
{
    using Fanex.Data.Repository;

    public class GetSiteByUserIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }
        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "Site_GetByUserId";

        public override bool IsValid() => UserId > 0;
    }
}