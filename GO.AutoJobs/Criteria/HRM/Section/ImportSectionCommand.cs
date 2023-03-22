namespace GO.AutoJobs.Criteria.Team
{
    using GO.Core.Models;

    public class ImportSectionCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }

        public string Value { get; set; }

        public override string GetSettingKey() => "AutoJob_Section_Import";

        public override bool IsValid() => SiteId > 0 && !string.IsNullOrWhiteSpace(Value);
    }
}