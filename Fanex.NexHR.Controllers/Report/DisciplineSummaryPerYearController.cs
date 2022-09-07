namespace Fanex.NexHR.Controllers.Report
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Report;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;

    [HRMPermission(Policy.Read, FunctionKey.DisciplineSummaryPerYear)]
    public class DisciplineSummaryPerYearController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly IEmployeeService employeeService;
        private readonly IExportDisciplineSummaryPerYearService exportService;

        public DisciplineSummaryPerYearController(
            IUserControlSession userControlSession,
            IBaseService baseService,
            IEmployeeService employeeService,
            IExportDisciplineSummaryPerYearService exportService) : base(userControlSession)
        {
            this.baseService = baseService;
            this.employeeService = employeeService;
            this.exportService = exportService;
        }

        public ActionResult Index(DisciplineSummaryPerYearViewModel model)
        {
            const byte DateRange = 4;
            if (model != null)
            {
                model.ToYear = DateTime.Today.Year;
                model.FromYear = DateTime.Today.Year - DateRange;
            }

            return View("~/Views/Report/DisciplineSummaryPerYear/Index.cshtml", model);
        }

        [HttpPost]
        public ActionResult GetDisciplineSummaryGrid(DisciplineSummaryPerYearViewModel model)
        {
            IList<DisciplineSummaryPerYear> employees = GetDisciplineSummary(model);
            ViewBag.FromYear = model.FromYear;
            ViewBag.ToYear = model.ToYear;
            return PartialView("~/Views/Report/DisciplineSummaryPerYear/_Grid.cshtml", employees);
        }

        [HttpGet]
        public ActionResult ExportExcel(DisciplineSummaryPerYearViewModel model)
        {
            IList<DisciplineSummaryPerYear> employees = GetDisciplineSummary(model);
            byte[] bytes = exportService.Export(model.FromYear, model.ToYear, employees);
            return File(bytes, MimeType.XLSX, "5_Years_Discipline_Summary.xlsx");
        }

        private IList<DisciplineSummaryPerYear> GetDisciplineSummary(DisciplineSummaryPerYearViewModel model)
        {
            var criteria = new GetDisciplineSummaryPerYearCriteria
            {
                FromDate = new DateTime(model.FromYear, 1, 1),
                ToDate = new DateTime(model.ToYear + 1, 1, 1).AddDays(-1),
                TeamId = model.TeamId,
                IsLocal = model.IsLocal,
                EmployeeId = model.EmployeeId,
                ApproverId = EmployeeSession.EmpId,
                IsShowAll = model.IsShowAll
            };
            (var employees, var disciplines) = baseService.FetchMultiple<DisciplineSummaryPerYear, DisciplineSummaryPerYearDetail>(criteria);

            foreach(var employee in employees)
            {
                employee.LengthOfService = employeeService.CaculateLengthOfService(employee.JoinedDate, employee.MilitaryFromDate, employee.MilitaryToDate);
                employee.Disciplines = disciplines.Where(x => x.EmployeeId == employee.EmployeeId).ToList();
            }

            return employees.ToList();
        }
    }
}