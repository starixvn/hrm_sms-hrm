namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Report.Overtime;

    public interface IOvertimeReportService
    {
        IEnumerable<YearlyOvertime> GetYearlyOvertime(GetYearlyOvertimeParam param);

        IEnumerable<DailyOvertimeDetails> GetMonthlyOvertimeDetails(int employeeId, DateTime fromDate, DateTime toDate);

        IEnumerable<DailyOvertimeDetails> GetDailyOvertimeDetails(GetDailyOvertimeDetailsParam param);

        byte[] ExportYearlyOvertime(IEnumerable<YearlyOvertime> overtimes, GetYearlyOvertimeParam param);

        byte[] ExportDailyOvertimeDetails(IEnumerable<DailyOvertimeDetails> overtimes, GetDailyOvertimeDetailsParam param);
    }
}