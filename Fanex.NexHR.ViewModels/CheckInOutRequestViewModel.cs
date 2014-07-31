// --------------------------------------------------------------------------
// <copyright file="CheckInOutRequestViewModel.cs" company="Nexcel Solutions Vietnam">
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
    using System.Web.Mvc;

    /// <summary>
    /// CheckInOutRequest ViewModel.
    /// </summary>
    public class CheckInOutRequestViewModel : BaseRequestModel, IValidatableObject
    {
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ForDate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, 23, ErrorMessage = "Hour is less than 23")]
        public int Hour { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, 59, ErrorMessage = "Minutes is less than 59")]
        public int Minute { get; set; }

        [Required]
        public short RequestType { get; set; }

        public string Shift { get; set; }

        public string RefNO { get; set; }

        [Display(Name = "Verified by")]
        public List<SelectListItem> Verifier { get; set; }

        [Display(Name = "Approved by")]
        public List<SelectListItem> Approver { get; set; }

        [Required(ErrorMessage = "The Verifier field is required.")]
        public int? VerifierId { get; set; }

        [Required(ErrorMessage = "The Approver field is required.")]
        public int? ApproverId { get; set; }

        public DateTime AttendantDate { get; set; }

        /// <summary>
        /// Method Validate.
        /// </summary>
        /// <param name="validationContext">Validation Context.</param>
        /// <returns>List Validation results.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IList<ValidationResult> result = new List<ValidationResult>();

            if (string.IsNullOrEmpty(Shift))
            {
                var validation = new ValidationResult(
                    "Invalid shift",
                    new[] { "Shift" });
                result.Add(validation);
            }

            if (ForDate.Date > DateTime.Today)
            {
                var validation = new ValidationResult(
                    Resources.Messages.NoFutureCheckInOutRequest,
                    new[] { "ForDate" });
                result.Add(validation);
            }

            if (string.IsNullOrEmpty(Shift))
            {
                var validation = new ValidationResult(
                    "Invalid shift",
                    new[] { "Shift" });
                result.Add(validation);
            }

            return result;
        }
    }
}
