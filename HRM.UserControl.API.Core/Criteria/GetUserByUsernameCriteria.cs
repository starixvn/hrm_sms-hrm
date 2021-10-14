namespace HRM.UserControl.API.Core.Criteria
{
    using Fanex.Data.Repository;

    internal class GetUserByUsernameCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public override string GetSettingKey() => "User_GetByUsername";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Username);
    }
}