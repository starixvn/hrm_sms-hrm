namespace Fanex.NexHR.ViewModels.Overtime
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Overtime;

    public class OvertimeExcelReportViewModel
    {
        public string Name { get; set; }

        public List<OvertimeExcelReport> ListOvertimeEntity { get; set; }
    }
}
