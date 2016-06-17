namespace Fanex.NexHR.Models.Overtime
{
    using Fanex.NexHR.Core;
    using System;
    using System.Collections.Generic;

    public class OvertimeRequest
    {
        public long RequestId { get; set; }

        public Employee Employee { get; set; }

        public DateTime Month { get; set; }

        public IList<OvertimeDaily> OvertimeDaily { get; set; }

        public decimal NormalHours { get; set; }

        public decimal OffHours { get; set; }

        public decimal PublicHours { get; set; }

        public Employee VerifiedBy { get; set; }

        public Employee ApprovedBy { get; set; }

        public Employee ProcessedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public DateTime? VerifiedDate { get; set; }

        public DateTime? ProcessedDate { get; set; }

        public byte CurrentStatus { get; set; }

        public byte VerifiedStatus { get; set; }

        public byte ApprovedStatus { get; set; }

        public byte ProcessedStatus { get; set; }

        public bool IsShowWarning { get; set; }

        public string GetCurrentStatusName()
        {
            return Utilities.GetCurrentStatusName(CurrentStatus);
        }
    }
}
