namespace GO.Website.Criteria
{
    using Fanex.Data.Repository;

    public class GetSiteAndMenuItemByUserIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override string GetSettingKey() => "User_GetSiteAndMenuItemByUserId";

        public override bool IsValid() => true;
    }
}