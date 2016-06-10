namespace Fanex.NexHR.Criteria.Request.LateInEarlyOut
{
    using System;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Attributes;

    public class InsertLateInEarlyOutRequestCriteria : CriteriaBase
    {
        [Output]
        public long RequestId { get; set; }

        public int EmpId { get; set; }

        public short RequestType { get; set; }

        public string Remark { get; set; }

        public DateTime OnDate { get; set; }

        public int LateInMins { get; set; }

        public int EarlyOutMins { get; set; }

        public int CreatedBy { get; set; }

        public int? VerifierId { get; set; }

        public int? ApproverId { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_Insert_LateInEarlyOutRequest";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
