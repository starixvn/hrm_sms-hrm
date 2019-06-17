namespace Fanex.NexHR.Criteria
{
    using Fanex.Data.Repository;

    public class GetOvertimeRequestByIdCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey() => "Overtime_GeById";

        public override bool IsValid() => RequestId > 0;
    }
}