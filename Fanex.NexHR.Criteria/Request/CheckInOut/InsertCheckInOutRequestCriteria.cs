namespace Fanex.NexHR.Criteria.Request.CheckInOut
{
    using System;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Attributes;

    public class InsertCheckInOutRequestCriteria : CriteriaBase
    {
        [Output]
        public long RequestId { get; set; }

        public int EmpId { get; set; }

        public int RequestType { get; set; }

        public string Remark { get; set; }

        public DateTime OnDate { get; set; }

        public DateTime RequestTime { get; set; }

        public byte ReasonId { get; set; }

        public int CreatedBy { get; set; }

        public int? VerifierId { get; set; }

        public int? ApproverId { get; set; }

        public string RefNo { get; set; }

        [SpParam(Ignored = true)]
        public string Shift { get; set; }

        public override string GetSettingKey()
        {
            return "CheckInOutRequest_Insert";
        }

        public override bool IsValid()
        {
            return EmpId > 0 && (RequestTime - OnDate).Days >= 0 && (RequestTime - OnDate).Days <= 1;
        }
    }
}
