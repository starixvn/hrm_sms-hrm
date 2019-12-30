namespace Fanex.NexHR.Criteria.BackgroundJob
{
    using Fanex.Data.Repository;

    public class UpdateWorkingHourPerDayCriteria : CriteriaBase
    {
        public int? EmployeeId { get; set; }

        public override string GetSettingKey() => "Auto_Postnatal_UpdateWorkingHour";

        public override bool IsValid() => true;
    }
}