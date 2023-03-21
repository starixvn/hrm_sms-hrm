namespace GO.AutoJobs.Criteria.Employee
{
    using HRM.UserControl.Core.Models;

    public class ImportEmployeeCommand : BaseNonQueryCommand
    {
        public short SiteId { get; set; }

        public string Value { get; set; }

        public override string GetSettingKey() => "AutoJob_Employee_Import";

        public override bool IsValid() => SiteId > 0 && !string.IsNullOrWhiteSpace(Value);
    }
}