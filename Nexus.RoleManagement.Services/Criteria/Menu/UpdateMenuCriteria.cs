namespace Nexus.RoleManagement.Services
{
    using Fanex.Data.Repository;

    public class UpdateMenuCriteria : CriteriaBase
    {
        public int MenuId { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public string Link { get; set; }

        public int? ParentId { get; set; }

        public int? FunctionId { get; set; }

        public int? Policy { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public override string GetSettingKey()
        {
            return "Menu_Update";
        }

        public override bool IsValid()
        {
            return MenuId > 0;
        }
    }
}
