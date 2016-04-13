namespace Fanex.NexHR.ViewModels.Discipline
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class OtherViolationViewModel
    {
        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        [Display(Name = "Violation Date")]
        public DateTime WorkDate { get; set; }

        [Display(Name = "Violation Type")]
        public int OtherViolationTypeId { get; set; }

        [Display(Name = "Minus Score")]
        public float Score { get; set; }

        public string Comment { get; set; }
    }
}