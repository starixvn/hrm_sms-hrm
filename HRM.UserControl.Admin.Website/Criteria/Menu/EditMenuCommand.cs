namespace HRM.UserControl.Admin.Website.Criteria.Menu
{
    public class EditMenuCommand : BaseNonQueryCommand
    {
        public short MenuItemId { get; set; }
        public short SiteId { get; set; }
        public short? FunctionId { get; set; }
        public string MenuItemName { get; set; }
        public string Description { get; set; }
        public int? ParentId { get; set; }
        public string URL { get; set; }
        public short DisplayOrder { get; set; }
        public int ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public override string GetSettingKey() => "Menu_Update";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MenuItemName) && ModifiedBy > 0;
    }
}
