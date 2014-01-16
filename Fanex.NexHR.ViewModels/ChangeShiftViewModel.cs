// --------------------------------------------------------------------------
// <copyright file="ChangeShiftViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Web.Mvc;

    /// <summary>
    /// ChangeShift ViewModel.
    /// </summary>
    public class ChangeShiftViewModel : BaseRequestModel, IValidatableObject
    {
        protected readonly string ShiftRequests = "<Request><OnDate>{0}</OnDate><FromShift>{1}</FromShift><ToShift>{2}</ToShift><Remark>{3}</Remark></Request>";

        public long RequestId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ForDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        [Required]
        public int FromShift { get; set; }

        public string FromShiftName { get; set; }

        [Required]
        public int ToShift { get; set; }

        public string ToShiftName { get; set; }

        public bool IsPlanned { get; set; }

        public List<ShiftDetailsViewModel> ShiftDetails { get; set; }

        [Display(Name = "Verified by")]
        public List<SelectListItem> Verifier { get; set; }

        [Display(Name = "Approved by")]
        public List<SelectListItem> Approver { get; set; }

        public int? VerifierId { get; set; }

        public int? ApproverId { get; set; }

        public int? RequestType { get; set; }

        public string Value
        {
            get
            {
                var value = new StringBuilder("<Requests>");

                if (ShiftDetails != null)
                {
                    foreach (var item in ShiftDetails)
                    {
                        value.AppendFormat(ShiftRequests, Convert.ToDateTime(item.Date).ToString("MM/dd/yyyy"), item.FromShift, item.ToShift, item.Remark); // 1 -> read
                    }
                }

                return value.Append("</Requests>").ToString();
            }
        }

        public string FormattedStatus { get; set; }

        /// <summary>
        /// Method Validate.
        /// </summary>
        /// <param name="validationContext">Validation context.</param>
        /// <returns>List Validation result.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IList<ValidationResult> result = new List<ValidationResult>();

            if (FromShift == 0)
            {
                var validation = new ValidationResult(
                    "Invalid FromShift value",
                    new[] { "FromShift" });
                result.Add(validation);
            }

            if (ToShift == 0)
            {
                var validation = new ValidationResult(
                    "Invalid ToShift value",
                    new[] { "ToShift" });
                result.Add(validation);
            }

            if (ToShift == FromShift)
            {
                var validation = new ValidationResult(
                    Resources.Messages.ToShiftMustDifferentFromFromShift,
                    new[] { "ToShift" });
                result.Add(validation);
            }

            return result;
        }
    }
}
