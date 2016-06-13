namespace Fanex.NexHR.Criteria.Request.Leave
{
    using System;
    using Fanex.Data.Repository;

    public class ResetLeaveRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public int EmpId { get; set; }

        [SpParam(Ignored = true)]
        public string Reason { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_ResetRequest_byMember";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}
