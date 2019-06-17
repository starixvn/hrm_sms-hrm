namespace Fanex.NexHR.ViewModels.Information
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Resources;

    public class EmployeeRelationshipViewModel
    {
        [Required]
        [StringLength(200, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.ValidateStringLengthMessage))]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Birthday")]
        public DateTime DateOfBirth { get; set; }

        [RegularExpression(@"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$", ErrorMessage = "Phone number is invalid")]
        [Display(Name = "Phone")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.ValidateStringLengthMessage))]
        public string Phone { get; set; }

        public int Gender { get; set; }

        public Relationship Relationship { get; set; }

        public bool IsExistEmergency { get; set; }
    }
}