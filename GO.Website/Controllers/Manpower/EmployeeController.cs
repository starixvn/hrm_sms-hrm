namespace GO.Website.Controllers.Manpower
{
    using System.Threading.Tasks;
    using GO.Website.Filters;
    using GO.Core.Models;
    using GO.Core.Services;
    using GO.Website.Constants;
    using GO.Website.Criteria.Manpower;
    using GO.Website.Models;
    using GO.Website.Models.Manpower;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;
    using GO.Website.Criteria.Report;
    using Fanex.NexHR.ViewModels;
    using System.Collections.Generic;
    using System.Linq;

    [Route("Manpower/Employee/[action]")]
    [GOPermission(FunctionKey = FunctionKey.ManpowerForecast_EmployeeManagement, Policy = Policy.Read)]
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
            bool isHost = userControlSession.HasPermissionOnFunction(FunctionKey.ManpowerForecast_EmployeeManagement, Policy.Host);
            return View("~/Views/Manpower/Employee/Index.cshtml", isHost);
        }

        [HttpPost]
        public async Task<IActionResult> GetEmployeeInfos([FromBody] GetEmployeeCriteria criteria)
        {
            var employeeInfos = await baseService.GetDataAsync<EmployeeInfo>(criteria);
            return Json(new { totalItems = criteria.TotalRows, data = employeeInfos});
        }

        [HttpGet]
        public async Task<IActionResult> GetSectionInfos()
        {
            var sections = await baseService.GetDataAsync<Section>(new GetSectionCriteria());
            IEnumerable<MultiSelectViewModel> orgchart = GetOrgChart(sections);
            return Json(orgchart);
        }

        public async Task<IActionResult> GetCompanies()
        {
            var criteria = new GetCompanyCriteria();
            var companies = await baseService.GetDataAsync<Company>(criteria);
            return Json(companies);
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.ManpowerForecast_EmployeeManagement, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateExclude([FromBody] UpdateExcludedEmployeeCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        private static IEnumerable<MultiSelectViewModel> GetOrgChart(
            IEnumerable<Section> sections)
        {
            var department = new List<Section>();
            foreach(var item in sections)
            {
                if(!department.Any(x => x.DepartmentName.ToLowerInvariant() == item.DepartmentName.ToLowerInvariant()))
                {
                    department.Add(item);
                }
            }

            var multiSelectViewModel = department.Select(d => new MultiSelectViewModel
            {
                Id = d.DepartmentId.ToString(),
                Text = d.DepartmentName,
                Items = GetMultiSelectViewModel(sections, d)
            });

            return multiSelectViewModel;
        }

        private static List<MultiSelectViewModel> GetMultiSelectViewModel(IEnumerable<Section> sections, Section department)
        {
            var multiSelectViewModel = new List<MultiSelectViewModel>();
            foreach(var item in sections)
            {
                if(!multiSelectViewModel.Any(x => x.Text.ToLowerInvariant() == item.SecTionName.ToLowerInvariant()) 
                    && !string.IsNullOrWhiteSpace(item.SecTionName)
                    && item.DepartmentName.ToLowerInvariant() == department.DepartmentName.ToLowerInvariant()){
                    multiSelectViewModel.Add(new MultiSelectViewModel
                    {
                        Id= string.Join(", ", sections.Where(s => !string.IsNullOrWhiteSpace(s.SecTionName)
                        && s.DepartmentName.ToLowerInvariant() == department.DepartmentName.ToLowerInvariant()
                        && item.SecTionName.ToLowerInvariant() == s.SecTionName.ToLowerInvariant()).Select(z => z.SectionId).ToArray()),
                        Text = item.SecTionName,
                        ParentId = department.DepartmentId,
                        IsLeaf = true
                    });
                }
            }
            return multiSelectViewModel;
        }
    }
}