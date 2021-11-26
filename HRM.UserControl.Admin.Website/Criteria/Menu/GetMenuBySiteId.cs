
namespace HRM.UserControl.Admin.Website.Criteria.Menu
{
    using Fanex.Data.Repository;

    public class GetMenuBySiteId : CriteriaBase
    {
        public short SiteId { get; set; }

        public override string GetSettingKey() => "Menu_GetBySiteId";

        public override bool IsValid() => SiteId > 0;
    }
   
}
