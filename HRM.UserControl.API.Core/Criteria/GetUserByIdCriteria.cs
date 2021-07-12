namespace HRM.UserControl.API.Core.Criteria
{
    using Fanex.Data.Repository;

    public class GetUserByIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override string GetSettingKey() => "API_User_GetById";

        public override bool IsValid() => UserId > 0;
    }
}