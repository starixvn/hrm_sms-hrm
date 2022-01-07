namespace HRM.UserControl.WebAPI.Criteria.User
{
    using Fanex.Data.Repository;

    public class GetUserByCodeCriteria : CriteriaBase
    {
        public string Code { get; set; }

        public override string GetSettingKey() => "API_User_GetByCode";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Code);
    }
}