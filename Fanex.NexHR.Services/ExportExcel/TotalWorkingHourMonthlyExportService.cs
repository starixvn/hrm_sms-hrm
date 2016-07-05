namespace Fanex.NexHR.Services.ExportExcel
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Models;
    using NPOI.HSSF.UserModel;

    public class TotalWorkingHourMonthlyExportService : IExportService
    {
        private readonly IList<TotalWorkingHourMonthly> totalWorkingHours;

        public TotalWorkingHourMonthlyExportService(IList<TotalWorkingHourMonthly> totalWorkingHours)
        {
            this.totalWorkingHours = totalWorkingHours ?? new List<TotalWorkingHourMonthly>();
        }

        public TotalWorkingHourMonthlyExportService()
            : this(null)
        {
        }

        public HSSFWorkbook Export()
        {
            var firstRecord = totalWorkingHours != null && totalWorkingHours.Any() ? totalWorkingHours.First() : null;
            var totalWorkingHourMonthlyExporting = new TotalWorkingHourMonthlyExporting(totalWorkingHours, firstRecord);
            return totalWorkingHourMonthlyExporting.ExportTotalWorkingHourMonthly();
        }
    }
}