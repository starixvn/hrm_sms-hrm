namespace GO.Website.Criteria.Manpower
{
    using GO.Core.Models;

    public class UpdateFlowCommand : BaseNonQueryCommand
    {
        public string UnitName { get; set; }
        public string FlowXml { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Manpower_Flow_Update";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(UnitName) && ModifiedBy > 0;
    }
}