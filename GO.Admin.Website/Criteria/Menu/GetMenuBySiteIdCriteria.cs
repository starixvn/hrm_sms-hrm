
namespace GO.Admin.Website.Criteria.Menu
{
    using Fanex.Data.Repository;

    public class GetMenuBySiteIdCriteria : CriteriaBase
    {
        public short SiteId { get; set; }

        public override string GetSettingKey() => "Menu_GetBySiteId";

        public override bool IsValid() => SiteId > 0;
    }
   
}
