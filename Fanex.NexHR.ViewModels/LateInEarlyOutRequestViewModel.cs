// --------------------------------------------------------------------------
// <copyright file="LateInEarlyOutRequestViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;

    /// <summary>
    /// LateInEarlyOutRequest ViewModel.
    /// </summary>
    public class LateInEarlyOutRequestViewModel : BaseRequestModel
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        public string FromTime { get; set; }

        public string ToTime { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [Range(1, 150, ErrorMessage = "The Minute field must be between 1 and 150")]
        public int Minute { get; set; }

        [Required]
        public short RequestType { get; set; }

        [Display(Name = "Verified by")]
        public List<SelectListItem> Verifier { get; set; }

        [Display(Name = "Approved by")]
        public List<SelectListItem> Approver { get; set; }

        [Required(ErrorMessage = "The Verifier field is required.")]
        public int? VerifierId { get; set; }

        [Required(ErrorMessage = "The Approver field is required.")]
        public int? ApproverId { get; set; }
    }
}
