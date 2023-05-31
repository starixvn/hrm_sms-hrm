namespace GO.Website.Controllers.Manpower
{
    using System.Threading.Tasks;
    using GO.Core.Models;
    using GO.Core.Services;
    using GO.Website.Criteria.Manpower;
    using GO.Website.Models;
    using GO.Website.Models.Manpower;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;

    [Route("Manpower/Employee/[action]")]
    public class EmployeeController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IBaseService baseService;

        public EmployeeController(
            IUserControlSession userControlSession,
            IBaseService baseService)
        {
            this.userControlSession = userControlSession;
            this.baseService = baseService;
        }

        public IActionResult Index()
        {
            return View("~/Views/Manpower/Employee/Index.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> GetEmployeeInfos([FromBody] GetEmployeeCriteria criteria)
        {
            var employeeInfos = await baseService.GetDataAsync<EmployeeInfo>(criteria);
            return Json(new { totalItems = criteria.TotalRows, data = employeeInfos });
        }

        public async Task<IActionResult> GetSites()
        {
            var criteria = new GetSiteCriteria();
            var sites = await baseService.GetDataAsync<Site>(criteria);
            return Json(sites);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateExclude([FromBody] UpdateExcludedEmployeeCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }
    }
}