namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Report.MonthlyWorkingHours;

    public interface IEmployeeWorkingHoursService
    {
        IEnumerable<MonthlyWorking> GetMonthlyWorkingHours(GetMonthlyWorkingHoursParam param);

        byte[] ExportMonthlyWorkingHours(DateTime fromDate, DateTime toDate, IEnumerable<MonthlyWorking> monthlyWorkings);

        byte[] ExportWeeklyWorkingHours(DateTime fromDate, DateTime toDate, IEnumerable<MonthlyWorking> monthlyWorkings);

        byte[] ExportDailyWorkingHours(DateTime date, IEnumerable<MonthlyWorking> monthlyWorkings);
    }
}