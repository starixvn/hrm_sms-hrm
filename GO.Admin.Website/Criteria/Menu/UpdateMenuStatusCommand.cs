namespace GO.Admin.Website.Criteria.Menu
{
    public class UpdateMenuStatusCommand : BaseNonQueryCommand
    {
        public int MenuItemId { get; set; }
        public bool IsActive { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Menu_UpdateStatus";

        public override bool IsValid() => MenuItemId > 0 && ModifiedBy > 0;
    }
}
