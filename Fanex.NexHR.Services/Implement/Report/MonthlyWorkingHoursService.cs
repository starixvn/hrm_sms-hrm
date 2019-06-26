namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models.Report.MonthlyWorkingHours;
    using Fanex.NexHR.Repositories;

    public class MonthlyWorkingHoursService : IMonthlyWorkingHoursService
    {
        private readonly IMonthlyWorkingHoursRepository repository;

        public MonthlyWorkingHoursService(IMonthlyWorkingHoursRepository repository)
        {
            this.repository = repository;
        }

        public IEnumerable<MonthlyWorking> GetMonthlyWorkingHours(GetMonthlyWorkingHoursParam param)
        {
            var criteria = new GetMonthlyWorkingHoursCriteria
            {
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                GroupIds = param.SectionIds,
                EmployeeId = param.EmployeeId,
                IsLocal = param.IsLocal
            };

            return repository.GetMonthlyWorkingHours(criteria);
        }
    }
}