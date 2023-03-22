namespace GO.WebAPI.Criteria.MenuItem
{
    using Fanex.Data.Repository;

    public class GetMenuItemByUserIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }
        public short SiteId { get; set; }

        public override string GetSettingKey() => "API_MenuItem_GetByUserId";

        public override bool IsValid() => UserId > 0 && SiteId > 0;
    }
}