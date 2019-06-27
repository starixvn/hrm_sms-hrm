namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Report.MonthlyWorkingHours;

    public interface IMonthlyWorkingHoursService
    {
        IEnumerable<MonthlyWorking> GetMonthlyWorkingHours(GetMonthlyWorkingHoursParam param);

        byte[] ExportExcel(DateTime fromDate, DateTime toDate, IEnumerable<MonthlyWorking> monthlyWorkings);
    }
}