namespace HRM.UserControl.API.Core.Criteria
{
    using Fanex.Data.Repository;

    public class GetUserCompanyBySiteCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public int SiteId { get; set; }

        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "API_User_GetCompanyBySite";

        public override bool IsValid() => UserId > 0 && SiteId > 0;
    }
}