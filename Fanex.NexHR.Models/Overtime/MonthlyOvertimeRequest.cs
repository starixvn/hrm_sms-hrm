namespace Fanex.NexHR.Models.Overtime
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Request;

    public class MonthlyOvertimeRequest
    {
        public long RequestId { get; set; }
        public DateTime Month { get; set; }
        public Requester Requester { get; set; }
        public Requester Verifier { get; set; }
        public Requester Approver { get; set; }
        public decimal NormalHours { get; set; }
        public decimal OffHours { get; set; }
        public decimal PublicHours { get; set; }
        public decimal TotalHours => NormalHours + OffHours + PublicHours;
        public RequestStatus Status { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public bool IsWarning { get; set; }

        public IEnumerable<DailyOvertimeRequest> DailyOvertimes { get; set; } = new List<DailyOvertimeRequest>();
    }
}