// --------------------------------------------------------------------------
// <copyright file="KeepInViewRequestViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Paul.Nguyen" date="2013.11.12">Initialize</change>
// </history>

namespace Fanex.NexHR.ViewModels.Request
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    /// <summary>
    /// LeaveRequest ViewModel.
    /// </summary>
    public class KeepInViewRequestViewModel
    {
        public long RequestId { get; set; }

        [Required]
        public string Employee { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        public string RefNo { get; set; }

        public bool HalfDay { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public string FromTime { get; set; }

        public string ToTime { get; set; }

        public bool CrossDay { get; set; }

        [AllowHtml]
        public string Remark { get; set; }

        public string Shift { get; set; }

        [Required]
        public int ShiftId { get; set; }

        public int? VerifierId { get; set; }

        public int? ApproverId { get; set; }

        public IList<SelectListItem> VerifierList { get; set; }

        public IList<SelectListItem> ApproverList { get; set; }
    }
}
