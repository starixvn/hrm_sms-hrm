namespace Fanex.NexHR.Criteria.Request.ChangeShift
{
    using Fanex.Data.Repository;

    public class GetChangeShiftDetailToIgnoreByIdCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_GetChangeShiftDetailToIgnoreById";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}