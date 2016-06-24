namespace Nexus.RoleManagement.Services
{
    using Fanex.Data.Repository;

    public class GetMenusByUserIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override bool IsValid()
        {
            return UserId > 0;
        }

        public override string GetSettingKey()
        {
            return "Menu_GetMenusByUserId";
        }
    }
}
