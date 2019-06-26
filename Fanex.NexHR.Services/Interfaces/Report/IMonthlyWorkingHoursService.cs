namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Report.MonthlyWorkingHours;

    public interface IMonthlyWorkingHoursService
    {
        IEnumerable<MonthlyWorking> GetMonthlyWorkingHours(GetMonthlyWorkingHoursParam param);
    }
}