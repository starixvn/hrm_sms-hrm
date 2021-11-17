namespace HRM.UserControl.API.Core.Criteria.User
{
    using HRM.UserControl.Core.Models;

    public class InsertLogUserLoginCommand : BaseNonQueryCommand
    {
        public int UserId { get; set; }
        public short SiteId { get; set; }
        public string IP { get; set; }
        public string Browser { get; set; }
        public string Domain { get; set; }
        public string Platform { get; set; }

        public override string GetSettingKey() => "Log_UserLogin_Insert";

        public override bool IsValid() => UserId > 0 && SiteId > 0;
    }
}