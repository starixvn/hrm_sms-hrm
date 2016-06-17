namespace Fanex.NexHR.ViewModels.Overtime
{
    using Fanex.NexHR.Models.Request;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class OvertimeReportViewModel
    {
        [Required]
        public string Employee { get; set; }

        [Required]
        public int EmpId { get; set; }

        public int Year { get; set; }

        public IList<RequestEntity> TotalRequest { get; set; }
    }
}
