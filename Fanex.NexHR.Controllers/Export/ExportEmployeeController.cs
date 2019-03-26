namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Criteria.Employee;
    using Models;
    using Services;
    using ViewModels;

    public class ExportEmployeeController : BaseExportController
    {
        private readonly IEmployeeService employeeService;
        private readonly IExportExcelEmployeeInfoService exportExcelEmployeeInfoService;

        public ExportEmployeeController(
            IEmployeeService employeeService,
            IExportExcelEmployeeInfoService exportExcelEmployeeInfoService)
        {
            this.employeeService = employeeService;
            this.exportExcelEmployeeInfoService = exportExcelEmployeeInfoService;
        }

        public ActionResult EmployeeBasicInfo(UserManagementViewModel model)
        {
            var criteria = new GetAllEmployeeCriteria
            {
                Query = model.Keyword,
                GroupIds = model.GroupIds,
                EnableUser = model.IsEnable,
                IsDeleteUser = model.IsDelete,
                TitleId = model.JobTitleId,
                IsLocalUser = model.IsLocal
            };

            IList<Employee> employees = employeeService.GetEmployees(criteria);
            exportExcelEmployeeInfoService.ExportEmployeeBasicInfo(employees);

            return new EmptyResult();
        }

        public ActionResult EmployeeDetailsInfo(UserManagementViewModel model)
        {
            var criteria = new GetAllEmployeeCriteria
            {
                Query = model.Keyword,
                GroupIds = model.GroupIds,
                EnableUser = model.IsEnable,
                IsDeleteUser = model.IsDelete,
                TitleId = model.JobTitleId,
                IsLocalUser = model.IsLocal
            };

            IList<Employee> employees = employeeService.GetEmployees(criteria);
            exportExcelEmployeeInfoService.ExportEmployeeDetailsInfo(employees);

            return new EmptyResult();
        }
    }
}