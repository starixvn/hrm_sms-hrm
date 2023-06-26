namespace GO.Website.Controllers
{
    using System.Threading.Tasks;
    using GO.Core.Services;
    using GO.Website.Criteria;
    using GO.Website.Criteria.Manpower.Period;
    using GO.Website.Models.Manpower;
    using GO.Website.Models.Manpower.Period;
    using Microsoft.AspNetCore.Mvc;

    public class CommonController : BaseController
    {
        private readonly IBaseService baseService;

        public CommonController(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IActionResult> SearchEmployees(SearchEmployeeCriteria criteria)
        {
            var employees = await baseService.GetDataAsync<EmployeeInfo>(criteria);

            return Json(employees);
        }

        public async Task<IActionResult> GetManpowerPeriods()
        {
            var criteria = new GetPeriodCriteria { IsActive = 1 };
            var periods = await baseService.GetDataAsync<ManpowerPeriod>(criteria);

            return Json(periods);
        }
    }
}