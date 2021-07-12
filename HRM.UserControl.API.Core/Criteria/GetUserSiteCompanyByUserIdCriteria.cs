namespace HRM.UserControl.API.Core.Criteria
{
    using Fanex.Data.Repository;

    public class GetUserSiteCompanyByUserIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "User_GetUserSiteCompany_ByUserId";

        public override bool IsValid() => UserId > 0;
    }
}