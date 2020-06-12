namespace Fanex.NexHR.Controllers.Report
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Model;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using Newtonsoft.Json;

    public class AttendanceReportController : BaseController
    {
        private readonly IReportService reportService;
        private readonly IExportExcelAttendanceReportService excelService;
        private readonly ITeamService teamService;

        public AttendanceReportController(
            ISessionService sessionService,
            IReportService reportSevice,
            IExportExcelAttendanceReportService excelService,
            ITeamService teamService) : base(sessionService)
        {
            this.reportService = reportSevice;
            this.excelService = excelService;
            this.teamService = teamService;
        }

        public ActionResult Index()
        {
            var model = new EmployeeAttendanceViewModel
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                TeamIds = string.Join(",", teamService.GetAll().Select(x => x.Id)),
                DateOption = 0,
                ExcludeDeletedUsers = true,
                SupervisorId = sessionService.Employee.Id,
            };

            return View("~/Views/Report/Attendance/Index.cshtml", model);
        }

        [HttpPost]
        public ActionResult Details(EmployeeAttendanceViewModel model)
        {
            var criteria = new GetEmployeeAttendanceCriteria
            {
                SupervisorId = model.SupervisorId,
                Fromtime = model.FromDate,
                Totime = model.ToDate,
                AttendanceType = model.AttendanceType,
                EmpId = model.EmployeeId,
                TeamIds = model.TeamIds,
                ExcludeUserDelete = model.ExcludeDeletedUsers ? 1 : 0,
                IsLocal = model.IsLocal,
                IsRealDate = model.IsRealDate
            };

            IList<EmployeeAttendanceModel> employeeAttendances = reportService.GetEmployeeAttendanceReport(criteria);

            employeeAttendances = SortAttendances(model.Sort, employeeAttendances);

            return PartialView("~/Views/Report/Attendance/_Details.cshtml", employeeAttendances);
        }

        private static IList<EmployeeAttendanceModel> SortAttendances(string sort, IList<EmployeeAttendanceModel> employeeAttendances)
        {
            IList<EmployeeAttendanceModel> memberOvertimeSorted;

            if (sort != null)
            {
                var sorts = JsonConvert.DeserializeObject<List<SortModel>>(sort);
                memberOvertimeSorted = Utilities.MultipleSort(employeeAttendances, sorts);
            }
            else
            {
                memberOvertimeSorted = employeeAttendances;
            }

            return memberOvertimeSorted;
        }

        public ActionResult ExportExcel(EmployeeAttendanceViewModel model)
        {
            var criteria = new GetEmployeeAttendanceCriteria
            {
                SupervisorId = model.SupervisorId,
                Fromtime = model.FromDate,
                Totime = model.ToDate,
                AttendanceType = model.AttendanceType,
                EmpId = model.EmployeeId,
                TeamIds = model.TeamIds,
                ExcludeUserDelete = model.ExcludeDeletedUsers ? 1 : 0,
                IsLocal = model.IsLocal,
                IsRealDate = model.IsRealDate
            };
            IList<EmployeeAttendanceModel> employeeAttendances = reportService.GetEmployeeAttendanceReport(criteria);
            if (model.Sort != null)
            {
                var sorts = JsonConvert.DeserializeObject<List<SortModel>>(model.Sort);

                employeeAttendances = Utilities.MultipleSort(employeeAttendances, sorts);
            }

            excelService.Export(employeeAttendances, model.FromDate, model.ToDate);

            return new EmptyResult();
        }
    }
}