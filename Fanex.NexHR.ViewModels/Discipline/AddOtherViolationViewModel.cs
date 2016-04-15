namespace Fanex.NexHR.ViewModels.Discipline
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Models.Discipline;

    public class AddOtherViolationViewModel
    {
        [Required(ErrorMessage = "Employee is required.")]
        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        [Required(ErrorMessage = "Violation Date is required.")]
        [Display(Name = "Violation Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime WorkDate { get; set; }

        [Required(ErrorMessage = "Other violation type is required.")]
        [Display(Name = "Violation Type")]
        public int OtherViolationTypeId { get; set; }

        [Display(Name = "Minus Score")]
        public float MinusScore { get; set; }

        public string Comment { get; set; }

        public IList<DisciplineTypeModel> OtherViolationTypes { get; set; }
    }
}