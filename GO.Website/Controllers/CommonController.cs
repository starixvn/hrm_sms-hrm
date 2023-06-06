namespace GO.Website.Controllers
{
    using System.Threading.Tasks;
    using GO.Core.Services;
    using GO.Website.Criteria;
    using GO.Website.Models.Manpower;
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
    }
}