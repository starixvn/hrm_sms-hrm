namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Report.Overtime;

    public interface IOvertimeReportService
    {
        IEnumerable<YearlyOvertime> GetYearlyOvertime(GetYearlyOvertimeParam param);

        IEnumerable<DailyOvertimeDetails> GetMonthlyOvertimeDetails(int employeeId, DateTime fromDate, DateTime toDate);

        byte[] ExportYearlyOvertime(IEnumerable<YearlyOvertime> overtimes, GetYearlyOvertimeParam param);
    }
}