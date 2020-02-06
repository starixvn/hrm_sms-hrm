namespace Fanex.NexHR.ViewModels.Information
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.NexHR.Resources;

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    public class EmployeeEmergencyViewModel
    {
        [Required]
        [StringLength(200, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Birthday")]
        public DateTime BirthDay { get; set; }

        [Required]
        [RegularExpression(@"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$", ErrorMessage = "Phone number is invalid")]
        [Display(Name = "Phone")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
        public string Phone { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
        public string Relationship { get; set; }

        public int Gender { get; set; }
    }
}