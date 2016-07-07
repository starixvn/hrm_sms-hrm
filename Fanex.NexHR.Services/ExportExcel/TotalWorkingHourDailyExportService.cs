namespace Fanex.NexHR.Services.ExportExcel
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Models;
    using NPOI.HSSF.UserModel;

    public class TotalWorkingHourDailyExportService : IExportService
    {
        private readonly IList<TotalWorkingHourMonthly> totalWorkingHourDaily;

        public TotalWorkingHourDailyExportService(IList<TotalWorkingHourMonthly> totalWorkingHourDaily)
        {
            this.totalWorkingHourDaily = totalWorkingHourDaily ?? new List<TotalWorkingHourMonthly>();
        }

        public TotalWorkingHourDailyExportService()
            : this(null)
        {
        }

        public HSSFWorkbook Export()
        {
            var firstRecord = totalWorkingHourDaily != null && totalWorkingHourDaily.Any() ? totalWorkingHourDaily.First() : null;

            var totalWorkingHourMonthlyExporting = new TotalWorkingHourMonthlyExporting(totalWorkingHourDaily, firstRecord);
            return totalWorkingHourMonthlyExporting.ExportTotalWorkingHourDaily();
        }
    }
}