namespace HRM.UserControl.Admin.Website.Criteria.User
{
    using Fanex.Data.Repository;

    public class GetMappingByUserIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override string GetSettingKey() => "User_GetMapping";

        public override bool IsValid() => UserId > 0;
    }
}