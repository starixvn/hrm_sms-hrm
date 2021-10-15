namespace HRM.UserControl.API.Core.Criteria.User
{
    using HRM.UserControl.Core.Models;

    public class InsertLogUserLoginCommand : BaseCommandNonQuery
    {
        public int UserId { get; set; }

        public short SiteId { get; set; }

        public string IPAddress { get; set; }

        public string Browser { get; set; }

        public string Domain { get; set; }

        public string Platform { get; set; }

        public override string GetSettingKey() => "User_InsertLogUserLogin";

        public override bool IsValid() => UserId > 0 && SiteId > 0 && !string.IsNullOrEmpty(IPAddress);
    }
}