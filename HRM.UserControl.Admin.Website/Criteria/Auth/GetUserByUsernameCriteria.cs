namespace HRM.UserControl.Admin.Website.Criteria.Auth
{
    using Fanex.Data.Repository;

    public class GetUserByUsernameCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public override string GetSettingKey() => "User_GetByUsername";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Username);
    }
}