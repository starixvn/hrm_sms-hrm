namespace HRM.UserControl.WebAPI.Criteria.SyncData
{
    using HRM.UserControl.Core.Models;

    public class CreateUserCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public override string GetSettingKey() => "API_SyncData_InsertUser";

        public override bool IsValid() => SiteId > 0;
    }
}