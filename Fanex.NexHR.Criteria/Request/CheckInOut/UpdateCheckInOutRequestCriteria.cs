namespace Fanex.NexHR.Criteria.Request.CheckInOut
{
    using System;
    using Fanex.Data.Repository;

    public class UpdateCheckInOutRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public int EmpId { get; set; }

        public int RequestType { get; set; }

        public string Remark { get; set; }

        public DateTime OnDate { get; set; }

        public DateTime RequestTime { get; set; }

        public byte ReasonId { get; set; }

        public int? VerifierId { get; set; }

        public int? ApproverId { get; set; }

        public int ModifiedBy { get; set; }

        public string RefNo { get; set; }

        [SpParam(Ignored = true)]
        public string Shift { get; set; }

        public override string GetSettingKey()
        {
            return "CheckInOutRequest_Update";
        }

        public override bool IsValid()
        {
            return EmpId > 0 && ReasonId > 0 && (RequestTime - OnDate).Days >= 0 && (RequestTime - OnDate).Days <= 1;
        }
    }
}
