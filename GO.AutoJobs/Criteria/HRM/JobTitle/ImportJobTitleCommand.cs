namespace GO.AutoJobs.Criteria.HRM
{
    using GO.Core.Models;

    public class ImportJobTitleCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }

        public string Value { get; set; }

        public override string GetSettingKey() => "AutoJob_JobTitle_Import";

        public override bool IsValid() => SiteId > 0 && !string.IsNullOrWhiteSpace(Value);
    }
}