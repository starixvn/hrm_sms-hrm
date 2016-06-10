namespace Fanex.NexHR.Criteria.Request.ChangeShift
{
    using Fanex.Data.Repository;

    public class GetChangeShiftRequestDetailByIdCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_GetChangeShiftTypeDetailByRequestID";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}