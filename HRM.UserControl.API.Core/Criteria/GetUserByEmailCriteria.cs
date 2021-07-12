namespace HRM.UserControl.API.Core.Criteria
{
    using Fanex.Data.Repository;

    public class GetUserByEmailCriteria : CriteriaBase
    {
        public string Email { get; set; }

        public override string GetSettingKey() => "API_User_GetByEmail";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Email);
    }
}