namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;

    public class MyOverTimeViewModel
    {
        public DateTime Month { get; set; }

        [Display(Name = "Verified by")]
        public int? VerifierId { get; set; }

        [Display(Name = "Approved by")]
        public int? ApproverId { get; set; }

        public IList<OvertimeDaily> OvertimeRequest { get; set; }

        public string Status { get; set; }

        [Required]
        public string Employee { get; set; }

        [Required]
        public int EmpId { get; set; }

        public VerifierAndApproverForRequest VerifierAndApprover { get; set; }
    }
}