namespace Fanex.NexHR.Criteria.Request.KeepInView
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Attributes;
    using System;
    using Fanex.NexHR.Core.Enums;

    public class InsertKeepInViewRequestCriteria : CriteriaBase
    {
        [Output]
        public long RequestId { get; set; }

        public int EmpId { get; set; }

        public string Remark { get; set; }

        public int KeepInViewType { get; set; }

        public DateTime OnDate { get; set; }

        public bool IsHalfDay { get; set; }

        public int ShiftId { get; set; }

        [SpParam(Ignored = true)]
        public string ShiftName { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public int CreatedBy { get; set; }

        public int? VerifierId { get; set; }

        public int? ApproverId { get; set; }

        public string RefNo { get; set; }

        public override string GetSettingKey()
        {
            return KeepInViewType == (int)KeepInViewTypes.KeepInViewByDay
                ? "KeepInViewRequestByDay_Insert"
                : "KeepInViewRequestByHour_Insert";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
