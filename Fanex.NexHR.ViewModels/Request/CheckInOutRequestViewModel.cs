namespace Fanex.NexHR.ViewModels.Request
{
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Resources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class CheckInOutRequestViewModel : IValidatableObject
    {
        public long RequestId { get; set; }

        [Required]
        public string Employee { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime AttendantDate { get; set; }

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
        public int RequestTypeId { get; set; }

        public string RequestTypeName
        {
            get
            {
                return RequestTypeId == (int)RequestType.CheckIn ? Labels.CheckIn : Labels.CheckOut;
            }
        }

        public byte ReasonId { get; set; }

        public string Shift { get; set; }

        public string RefNO { get; set; }

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
