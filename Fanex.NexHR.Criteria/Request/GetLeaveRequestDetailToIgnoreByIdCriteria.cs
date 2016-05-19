namespace Fanex.NexHR.Criteria.Request
{
    using Fanex.Data.Repository;

    public class GetLeaveRequestDetailToIgnoreByIdCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_GetLeaveRequestDetailToIgnoreById";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}