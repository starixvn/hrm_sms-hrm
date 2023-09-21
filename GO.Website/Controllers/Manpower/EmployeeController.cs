namespace GO.Website.Controllers.Manpower
{
    using System;
    using System.Threading.Tasks;
    using Filters;
    using GO.Core.Services;
    using Constants;
    using GO.Website.Criteria.Manpower;
    using Models;
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
            var isHost = userControlSession.HasPermissionOnFunction(FunctionKey.ManpowerForecast_EmployeeManagement, Policy.Host);
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
            var dataViews = GetOrgChart(sections.ToList());
            return Json(dataViews);
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
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        private static IEnumerable<MultiSelectViewModel> GetOrgChart(
            IList<Section> sections)
        {
            var department = new List<Section>();
            foreach (var item in sections.Where(item => department.All(x => !string.Equals(x.DepartmentName, item.DepartmentName, StringComparison.OrdinalIgnoreCase))))
            {
                department.Add(item);
            }

            var multiSelectViewModel = department.Select(d => new MultiSelectViewModel
            {
                Id = d.DepartmentId.ToString(),
                Text = d.DepartmentName,
                Items = GetMultiSelectViewModel(sections, d)
            });

            return multiSelectViewModel;
        }

        private static List<MultiSelectViewModel> GetMultiSelectViewModel(IList<Section> sections, Section department)
        {
            var multiSelectViewModel = new List<MultiSelectViewModel>();
            foreach (var item in sections.Where(item => 
                         multiSelectViewModel.All(x => !string.Equals(x.Text, item.SecTionName, StringComparison.OrdinalIgnoreCase)) && !string.IsNullOrWhiteSpace(item.SecTionName)
                         && string.Equals(item.DepartmentName, department.DepartmentName, StringComparison.OrdinalIgnoreCase)))
            {
                multiSelectViewModel.Add(new MultiSelectViewModel
                {
                    Id= string.Join(", ", sections.Where(s => !string.IsNullOrWhiteSpace(s.SecTionName)
                                                              && string.Equals(s.DepartmentName, department.DepartmentName, StringComparison.OrdinalIgnoreCase)
                                                              && string.Equals(item.SecTionName, s.SecTionName, StringComparison.OrdinalIgnoreCase)).Select(z => z.SectionId).ToArray()),
                    Text = item.SecTionName,
                    ParentId = department.DepartmentId,
                    IsLeaf = true
                });
            }
            return multiSelectViewModel;
        }
    }
}