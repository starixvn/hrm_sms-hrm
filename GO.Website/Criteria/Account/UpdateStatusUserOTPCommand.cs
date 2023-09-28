namespace GO.Website.Criteria.Account
{
    using GO.Core.Models;

    public class UpdateStatusUserOTPCommand : BaseNonQueryCommand
    {
        public int SiteId { get; set; }
        public int UserId { get; set; }
        public bool IsOTPRegistered { get; set; }

        public override string GetSettingKey() => "UserOTP_UpdateStatus";

        public override bool IsValid() => SiteId > 0 && UserId > 0;
    }
}
