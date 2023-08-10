namespace GO.Website.Criteria
{
    using Fanex.Data.Repository;

    public class GetUserSiteByUserIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override string GetSettingKey() => "GO_User_GetUserSiteByUserId";

        public override bool IsValid() => UserId > 0;
    }
}