namespace Fanex.NexHR.ViewModels.Request
{
    using Fanex.NexHR.Models.Request;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class LateInEarlyOutRequestViewModel
    {
        public long RequestId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public string FromTime { get; set; }

        public string ToTime { get; set; }

        [Required]
        public string Employee { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "The Remark field is required.")]
        public string Remark { get; set; }

        [Required]
        [Range(1, 60, ErrorMessage = "The Minute field must be between 1 and 60")]
        public int Minute { get; set; }

        [Required]
        public short RequestType { get; set; }

        [Display(Name = "Verified by")]
        [Required(ErrorMessage = "The Verified by field is required.")]
        public int? VerifierId { get; set; }

        [Display(Name = "Approved by")]
        [Required(ErrorMessage = "The Approved by field is required.")]
        public int? ApproverId { get; set; }

        public decimal TotalRequestInMonth { get; set; }

        public VerifierAndApproverForRequest VerifierAndApprover { get; set; }

        public IList<DateTime> UnavailDates { get; set; }
    }
}
