namespace Nexus.RoleManagement.Services
{
    using Fanex.Data.Repository;

    public class InsertMenuCriteria : CriteriaBase
    {
        public string Name { get; set; }

        public string Text { get; set; }

        public string Link { get; set; }

        public int? ParentId { get; set; }

        public int? FunctionId { get; set; }

        public int? Policy { get; set; }

        public int DisplayOrder { get; set; }

        public override string GetSettingKey()
        {
            return "Menu_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
