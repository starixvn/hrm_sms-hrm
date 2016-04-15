namespace Fanex.NexHR.Services.ExportExcel
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Models;
    using NPOI.HSSF.UserModel;

    public class TotalWorkingHourMonthlyExportService : IExportService
    {
        private readonly IList<TotalWorkingHourMonthly> totalWorkingHourMonthly;

        public TotalWorkingHourMonthlyExportService(IList<TotalWorkingHourMonthly> totalWorkingHourMonthly)
        {
            this.totalWorkingHourMonthly = totalWorkingHourMonthly ?? new List<TotalWorkingHourMonthly>();
        }

        public TotalWorkingHourMonthlyExportService()
            : this(null)
        { 
        }

        public HSSFWorkbook Export()
        {
            var firstRecord = totalWorkingHourMonthly != null && totalWorkingHourMonthly.Any() ? totalWorkingHourMonthly[0] : null;

            var totalWorkingHourMonthlyExporting = new TotalWorkingHourMonthlyExporting(totalWorkingHourMonthly, firstRecord);
            return totalWorkingHourMonthlyExporting.ExportTotalWorkingHourMonthly();
        }
    }
}
