namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Report.Overtime;

    public interface IOvertimeReportService
    {
        IEnumerable<YearlyOvertime> GetYearlyOvertime(GetYearlyOvertimeParam param);

        byte[] ExportYearlyOvertime(IEnumerable<YearlyOvertime> overtimes, GetYearlyOvertimeParam param);
    }
}