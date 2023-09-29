namespace GO.Website.Criteria.Account
{
    using Core.Models;

    public class UpdateSecretKeyCriteria : BaseNonQueryCommand
    {
        public int UserId { get; set; }

        public int SiteId { get; set; }
        public string SecretKey { get; set; }

        public override string GetSettingKey() => "UserOTP_UpdateSecretKey";

        public override bool IsValid() => UserId > 0 && SiteId > 0;
    }
}
