namespace Fanex.NexHR.Criteria.Overtime
{
    using Fanex.Data.Repository;

    public class GetOvertimeRequestByIdCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_GeById";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}
