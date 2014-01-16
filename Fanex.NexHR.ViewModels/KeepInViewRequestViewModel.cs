// --------------------------------------------------------------------------
// <copyright file="KeepInViewRequestViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Paul.Nguyen" date="2013.11.12">Initialize</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.NexHR.Models;

    /// <summary>
    /// LeaveRequest ViewModel.
    /// </summary>
    public class KeepInViewRequestViewModel : BaseRequestModel
    {
        public string RefNo { get; set; }

        public bool HalfDay { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public string FromTime { get; set; }

        public string ToTime { get; set; }

        public bool CrossDay { get; set; }

        public string Shift { get; set; }

        [Required]
        public int ShiftId { get; set; }

        public int? VerifierId { get; set; }

        public int? ApproverId { get; set; }

        public IList<SupervisorNameModel> VerifierList { get; set; }

        public IList<SupervisorNameModel> ApproverList { get; set; }
    }
}
