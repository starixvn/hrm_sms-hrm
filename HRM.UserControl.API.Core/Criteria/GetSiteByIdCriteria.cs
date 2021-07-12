namespace HRM.UserControl.API.Core.Criteria
{
    using Fanex.Data.Repository;

    public class GetSiteByIdCriteria : CriteriaBase
    {
        public int SiteId { get; set; }

        public override string GetSettingKey() => "Common_Site_GetById";

        public override bool IsValid() => SiteId > 0;
    }
}