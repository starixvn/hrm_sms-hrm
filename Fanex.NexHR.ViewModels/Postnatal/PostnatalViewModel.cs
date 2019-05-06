namespace Fanex.NexHR.ViewModels.Postnatal
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PostnatalViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "The Employee is required")]
        public int EmployeeId { get; set; }

        [Display(Name = "Employee")]
        public string Employee { get; set; }

        [Required(ErrorMessage = "From Date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "To Date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime ToDate { get; set; }
    }
}