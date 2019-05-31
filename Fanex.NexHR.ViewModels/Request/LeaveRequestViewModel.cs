namespace Fanex.NexHR.ViewModels.Request
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using System.Linq;

    public class LeaveRequestViewModel : IValidatableObject
    {
        public long RequestId { get; set; }

        [Required]
        public string Employee { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [AllowHtml]
        public string Remark { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.##}")]
        public decimal TotalDays { get; set; }

        public int LeaveTypeId { get; set; }

        public string FromTime { get; set; }

        public int FromTimeHour { get; set; }

        public int FromTimeMinute { get; set; }

        public int ToTimeHour { get; set; }

        public int ToTimeMinute { get; set; }

        public bool IsFromTimeAfterDay { get; set; }

        public string ToTime { get; set; }

        public bool IsToTimeAfterDay { get; set; }

        public bool HalfDayOff { get; set; }

        public string LeaveTypeName { get; set; }

        public string LeaveTypeDescription
        {
            get
            {
                return LeaveTypeList.FirstOrDefault(t => t.Id == LeaveTypeId).Description;

            }
        }

        public string RefNo { get; set; }

        public int VerifierId { get; set; }

        public int ApproverId { get; set; }

        public decimal TotalWeekendLeave { get; set; }

        public IEnumerable<DateTime> UnavailDates { get; set; }

        public IList<LeaveType> LeaveTypeList { get; set; }

        public VerifierAndApproverForRequest VerifierAndApprover { get; set; }

        public EmployeeLeaveBalance EmployeeLeaveBalance { get; set; }

        public RequestStatus Status { get; set; }

        public DateTime MinDate { get; set; }

        public DateTime MaxDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IList<ValidationResult> result = new List<ValidationResult>();

            var diff = ToDate.Subtract(FromDate);
            if (diff.TotalDays < 0)
            {
                var validation = new ValidationResult(
                    Resources.Messages.InvalidToDate,
                    new[] { "ToDate" });
                result.Add(validation);
            }

            if (HalfDayOff && (TotalDays % 1 == 0))
            {
                var validation = new ValidationResult(
                    Resources.Messages.InvalidTotalDays,
                    new[] { "TotalDays" });
                result.Add(validation);
            }

            return result;
        }
    }
}
