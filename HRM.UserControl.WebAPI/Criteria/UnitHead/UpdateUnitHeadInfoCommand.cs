namespace HRM.UserControl.WebAPI.Criteria.UnitHead
{
    using HRM.UserControl.Core.Models;

    public class UpdateUnitHeadInfoCommand : BaseNonQueryCommand
    {
        public string Value { get; set; }
        public short SiteId { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "API_UnitHead_UpdateInfo";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Value) && SiteId > 0 && ModifiedBy > 0;
    }
}