namespace GO.AutoJobs.Criteria.HRM
{
    using GO.Core.Models;

    public class ImportDepartmentFromDBCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }

        public string DbName { get; set; }

        public override string GetSettingKey() => "AutoJob_Department_ImportFromDB";

        public override bool IsValid() => SiteId > 0 && !string.IsNullOrWhiteSpace(DbName);
    }
}