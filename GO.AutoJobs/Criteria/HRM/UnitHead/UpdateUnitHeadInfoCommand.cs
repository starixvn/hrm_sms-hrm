namespace GO.AutoJobs.Criteria.UnitHead
{
    using GO.Core.Models;

    public class UpdateUnitHeadInfoCommand : BaseNonQueryCommand
    {
        public string Value { get; set; }

        public override string GetSettingKey() => "AutoJob_UnitHead_UpdateInfo";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Value);
    }
}