namespace Fanex.NexHR.ViewModels.Request
{
    using Fanex.NexHR.Models.Request;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class ChangeShiftViewModel
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
        public DateTime FromDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        public IList<ChangeShiftDetailViewModel> ShiftDetails { get; set; }

        [Display(Name = "Verified by")]
        public List<SelectListItem> Verifier { get; set; }

        [Display(Name = "Approved by")]
        public List<SelectListItem> Approver { get; set; }

        public int? VerifierId { get; set; }

        public int? ApproverId { get; set; }

        public int RequestType { get; set; }

        public RequestStatus Status { get; set; }

        public decimal TotalRequestInMonth { get; set; }

        public VerifierAndApproverForRequest VerifierAndApprover { get; set; }
    }
}
