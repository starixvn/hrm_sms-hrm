namespace GO.Website.Controllers.Manpower
{
    using System.Threading.Tasks;
    using GO.Admin.Website.Filters;
    using GO.Core.Models;
    using GO.Core.Services;
    using GO.Website.Constants;
    using GO.Website.Criteria.Manpower;
    using GO.Website.Models;
    using GO.Website.Models.Manpower;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;

    [Route("Manpower/Employee/[action]")]
    [GOPermission(FunctionKey = FunctionKey.Manpower_EmployeeManagement, Policy = Policy.Read)]
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
            bool isHost = userControlSession.HasPermissionOnFunction(FunctionKey.Manpower_EmployeeManagement, Policy.Host);
            return View("~/Views/Manpower/Employee/Index.cshtml", isHost);
        }

        [HttpPost]
        public async Task<IActionResult> GetEmployeeInfos([FromBody] GetEmployeeCriteria criteria)
        {
            var employeeInfos = await baseService.GetDataAsync<EmployeeInfo>(criteria);
            return Json(new { totalItems = criteria.TotalRows, data = employeeInfos });
        }

        public async Task<IActionResult> GetCompanies()
        {
            var criteria = new GetCompanyCriteria();
            var companies = await baseService.GetDataAsync<Company>(criteria);
            return Json(companies);
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.Manpower_EmployeeManagement, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateExclude([FromBody] UpdateExcludedEmployeeCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }
    }
}