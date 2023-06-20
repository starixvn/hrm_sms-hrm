namespace GO.Website.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GO.Core.Services;
    using GO.Website.Criteria.Manpower;
    using GO.Website.Models.Manpower;
    using GO.Website.Models.Manpower.Period;

    public class ManpowerForecastService : IManpowerForecastService
    {
        private readonly IBaseService baseService;

        public ManpowerForecastService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<(ManpowerPeriod, IEnumerable<AnnualRequestFormDetail>)> GetDataForCreateAnnualRequestAsync(int userId)
        {
            ManpowerPeriod period = null;
            var criteria = new GetDataForCreateAnnualRequestCriteria
            {
                UserId = userId
            };
            (var periods, var details, var jlmappings) = await baseService.FetchMultipleAsync<ManpowerPeriod, AnnualRequestFormDetail, JobLevelMapping>(criteria);
            if (periods.Any())
            {
                period = periods.First();
                IEnumerable<EmployeeInfo> employees = await GetEmployeeByPeriodAsync(period.PeriodId);
                foreach (var detail in details)
                {
                    var joblevels = jlmappings.Where(x => x.Parent == detail.JobLevel).ToList();
                    var emps = employees.Where(x => x.SectionName == detail.SectionName && joblevels.Any(jl => jl.Child == x.JobLevel)).ToList();

                    detail.LocalCurrentHeadcount = emps.Count(x => x.IsLocal);
                    detail.ExpatCurrentHeadcount = emps.Count(x => !x.IsLocal);
                }
            }

            return (period, details);
        }

        protected virtual async Task<IEnumerable<EmployeeInfo>> GetEmployeeByPeriodAsync(short periodId)
        {
            var criteria = new GetEmployeeByPeriodCriteria
            {
                PeriodId = periodId
            };
            return await baseService.GetDataAsync<EmployeeInfo>(criteria);
        }
    }
}