namespace GO.Website.Criteria.Manpower
{
    using GO.Core.Models;

    public class DeleteFlowCommand : BaseNonQueryCommand
    {
        public string UnitName { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Manpower_Flow_Delete";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(UnitName) && ModifiedBy > 0;
    }
}