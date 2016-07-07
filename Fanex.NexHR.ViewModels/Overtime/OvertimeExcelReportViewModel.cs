namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class OvertimeExcelReportViewModel
    {
        public string Name { get; set; }

        public List<OvertimeExcelReport> ListOvertimeEntity { get; set; }
    }
}