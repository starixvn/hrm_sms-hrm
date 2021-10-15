namespace HRM.UserControl.API.Core.Criteria.User
{
    using Fanex.Data.Repository;

    public class GetUserByIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override string GetSettingKey() => "User_GetById";

        public override bool IsValid() => UserId > 0;
    }
}