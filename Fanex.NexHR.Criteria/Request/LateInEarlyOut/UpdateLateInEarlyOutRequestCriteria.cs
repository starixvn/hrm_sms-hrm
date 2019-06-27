namespace Fanex.NexHR.Criteria.Request.LateInEarlyOut
{
    using System;
    using Fanex.Data.Repository;

    public class UpdateLateInEarlyOutRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public int EmpId { get; set; }

        public int RequestType { get; set; }

        public string Remark { get; set; }

        public DateTime OnDate { get; set; }

        public int LateInMins { get; set; }

        public int EarlyOutMins { get; set; }

        public byte ReasonId { get; set; }

        public int ModifiedBy { get; set; }

        public int? VerifierId { get; set; }

        public int? ApproverId { get; set; }

        public override string GetSettingKey()
        {
            return "LateInEarlyOutRequest_Update";
        }

        public override bool IsValid()
        {
            return EmpId > 0 && RequestId > 0;
        }
    }
}
