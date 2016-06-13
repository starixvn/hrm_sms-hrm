namespace Fanex.NexHR.Criteria.Request.Leave
{
    using System;
    using Fanex.Data.Repository;

    public class GetLeaveRequestDetailByIdCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_GetLeaveRequestDetailByRequestID";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}
