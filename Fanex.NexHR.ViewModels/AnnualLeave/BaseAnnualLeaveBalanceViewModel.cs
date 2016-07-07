namespace Fanex.NexHR.ViewModels.AnnualLeave
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class BaseAnnualLeaveBalanceViewModel : IValidatableObject
    {
        [Required]
        public decimal TotalDays { get; set; }

        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Affect from")]
        public DateTime AffectedFrom { get; set; }

        public decimal DeductDays
        {
            get
            {
                return this.TotalDays < 0 ? -this.TotalDays : 0;
            }
        }

        public decimal CreditDays
        {
            get
            {
                return this.TotalDays > 0 ? this.TotalDays : 0;
            }
        }

        public int LocalLeave { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IList<ValidationResult> result = new List<ValidationResult>();

            if (this.TotalDays == 0 || this.TotalDays % (decimal)0.5 != 0)
            {
                var validation = new ValidationResult(
                    Resources.Messages.InvalidInputData,
                    new[] { "TotalDays" });
                result.Add(validation);
            }

            return result;
        }
    }
}
