namespace Fanex.NexHR.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class TimesheetViewModel
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public int? EmpId { get; set; }

        [Display(Name = "Request type")]
        public short TimeSheetType { get; set; }
    }
}