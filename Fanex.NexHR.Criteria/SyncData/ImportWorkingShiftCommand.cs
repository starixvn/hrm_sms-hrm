namespace Fanex.NexHR.Criteria.SyncData
{
    using Fanex.Data.Repository;

    public class ImportWorkingShiftCommand : CriteriaBase
    {
        public string WorkingShiftXml { get; set; }

        public override string GetSettingKey() => "SyncData_WorkingShiftXml_Import";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(WorkingShiftXml);
    }
}
