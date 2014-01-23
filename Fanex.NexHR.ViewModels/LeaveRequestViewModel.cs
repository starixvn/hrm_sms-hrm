// --------------------------------------------------------------------------
// <copyright file="LeaveRequestViewModel.cs" company="Nexcel Solutions Vietnam">
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
    using System.Web.Mvc;
    using Fanex.NexHR.Models;

    /// <summary>
    /// LeaveRequest ViewModel.
    /// </summary>
    public class LeaveRequestViewModel : BaseRequestModel, IValidatableObject
    {
        public long RequestId { get; set; }

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

        public bool IsPlanned { get; set; }

        public string FromTime { get; set; }

        public string ToTime { get; set; }

        public bool HalfDayOff { get; set; }

        public string LeaveTypeName { get; set; }

        public string RefNo { get; set; }

        public int VerifierId { get; set; }

        public int ApproverId { get; set; }

        public decimal TotalRequest { get; set; }

        public IEnumerable<DateTime> UnavailDates { get; set; }

        public IList<LeaveType> LeaveTypeList { get; set; }

        public IList<SelectListItem> VerifierList { get; set; }

        public IList<SelectListItem> ApproverList { get; set; }

        public EmployeeAnnualBalance EmployeeBalance { get; set; }

        public string FormattedStatus { get; set; }

        /// <summary>
        /// Method Validate.
        /// </summary>
        /// <param name="validationContext">Validation Context.</param>
        /// <returns>List Validation results.</returns>
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
