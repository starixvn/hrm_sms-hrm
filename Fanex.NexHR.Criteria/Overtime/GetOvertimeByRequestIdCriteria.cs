namespace Fanex.NexHR.Criteria
{
    using Fanex.Data.Repository;

    public class GetOvertimeByRequestIdCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_GetRequestByRequestID";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}