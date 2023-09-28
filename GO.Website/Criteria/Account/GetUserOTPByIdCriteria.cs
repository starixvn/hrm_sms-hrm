namespace GO.Website.Criteria.Account
{
    using Fanex.Data.Repository;

    public class GetUserOTPByIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }
        public int SiteId { get; set; }

        public override string GetSettingKey() => "UserOTP_GetById";

        public override bool IsValid() => UserId > 0 && SiteId > 0;
    }
}
