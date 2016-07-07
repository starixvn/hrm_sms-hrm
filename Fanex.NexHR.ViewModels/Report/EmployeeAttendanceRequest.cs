namespace Fanex.NexHR.ViewModels.Report
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using System;

    public class EmployeeAttendanceRequest
    {
        public RequestTypeModel RequestType { get; set; }

        public string Remark { get; set; }

        // Leave
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedDateTime
        {
            get
            {
                return CreatedDate.HasValue ? CreatedDate.Value.ToShortFormatedDate() : string.Empty;
            }
        }

        public decimal? TotalDays { get; set; }

        public LeaveType LeaveType { get; set; }

        public DateTime? FromTime { get; set; }

        public DateTime? ToTime { get; set; }

        // ChangeShift
        public string FromShift { get; set; }

        public string ToShift { get; set; }

        // LateIn-EarlyOut
        public int? TotalMins { get; set; }
    }
}
