namespace HRM.UserControl.API.Core.Criteria
{
    using HRM.UserControl.Core.Models;

    public class InsertUserLoginLogCommand : BaseCommandNonQuery
    {
        public int UserId { get; set; }

        public int SiteId { get; set; }

        public string IPAddress { get; set; }

        public string Browser { get; set; }

        public string Domain { get; set; }

        public string Platform { get; set; }

        public override string GetSettingKey() => "API_User_InsertLoginLog";

        public override bool IsValid() => UserId > 0 && SiteId > 0 && !string.IsNullOrEmpty(IPAddress);
    }
}