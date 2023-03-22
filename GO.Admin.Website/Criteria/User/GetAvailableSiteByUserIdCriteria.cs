namespace GO.Admin.Website.Criteria.User
{
    using Fanex.Data.Repository;

    public class GetAvailableSiteByUserIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }
        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "User_GetAvailableSites";

        public override bool IsValid() => UserId > 0;
    }
}