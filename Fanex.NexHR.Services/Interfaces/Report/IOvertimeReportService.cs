namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Report.Overtime;

    public interface IOvertimeReportService
    {
        IEnumerable<YearlyOvertime> GetYearlyOvertime(GetYearlyOvertimeParam param);

        IEnumerable<DailyOvertimeDetails> GetMonthlyOvertimeDetails(int employeeId, DateTime fromDate, DateTime toDate);

        Tuple<IEnumerable<DateRange>, IEnumerable<DailyOvertimeDetails>> GetMonthlyOvertime(
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null);

        IEnumerable<DailyOvertimeDetails> GetDailyOvertimeDetails(GetDailyOvertimeDetailsParam param);

        byte[] ExportYearlyOvertime(IEnumerable<YearlyOvertime> overtimes, GetYearlyOvertimeParam param);

        byte[] ExportMonthlyOvertime(IEnumerable<DateRange> dateRanges, IEnumerable<DailyOvertimeDetails> dailyOvertimes);

        byte[] ExportDailyOvertimeDetails(IEnumerable<DailyOvertimeDetails> overtimes, GetDailyOvertimeDetailsParam param);
    }
}