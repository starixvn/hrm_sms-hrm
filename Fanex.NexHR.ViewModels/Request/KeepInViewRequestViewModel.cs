namespace Fanex.NexHR.ViewModels.Request
{
    using Fanex.NexHR.Models.Request;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class KeepInViewRequestViewModel
    {
        public long RequestId { get; set; }

        [Required]
        public string Employee { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        public string RefNo { get; set; }

        public short KeepInViewType { get; set; }

        public bool HalfDay { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public string FromTime { get; set; }

        public string ToTime { get; set; }

        public bool CrossDay { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "The Remark field is required.")]
        public string Remark { get; set; }

        [Display(Name = "Verified by")]
        [Required(ErrorMessage = "The Verified by field is required.")]
        public int? VerifierId { get; set; }

        [Display(Name = "Approved by")]
        [Required(ErrorMessage = "The Approved by field is required.")]
        public int? ApproverId { get; set; }

        public VerifierAndApproverForRequest VerifierAndApprover { get; set; }

        public IList<DateTime> UnavailDates { get; set; }
    }
}
