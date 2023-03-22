namespace GO.AutoJobs.Criteria.UnitHead
{
    using GO.Core.Models;

    internal class UpdateUnitHeadScheduleCommand : BaseNonQueryCommand
    {
        public string Value { get; set; }

        public override string GetSettingKey() => "AutoJob_UnitHead_UpdateSchedule";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Value);
    }
}