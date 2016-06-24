namespace Nexus.RoleManagement.Services
{
    using Fanex.Data.Repository;

    public class GetMenuByIdCriteria : CriteriaBase
    {
        public int MenuId { get; set; }

        public override string GetSettingKey()
        {
            return "Menu_GetMenuById";
        }

        public override bool IsValid()
        {
            return MenuId > 0;
        }
    }
}
