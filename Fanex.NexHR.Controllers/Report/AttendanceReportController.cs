namespace Fanex.NexHR.Controllers.Report
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;

    public class AttendanceReportController : BaseController
    {
        private readonly IReportService reportService;
        private readonly ITeamService teamService;
        private readonly IExportExcelAttendanceReportService excelService;


        public AttendanceReportController(
            ISessionService sessionService,
            IReportService reportSevice,
            ITeamService teamService,
            IExportExcelAttendanceReportService excelService) : base(sessionService)
        {
            this.reportService = reportSevice;
            this.teamService = teamService;
            this.excelService = excelService;
        }

        public ActionResult Index()
        {
            ViewBag.Groups = teamService.GetBySupervisor(sessionService.Employee.Id);
            var model = new EmployeeAttendanceViewModel
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                GroupIds = string.Empty,
                DateOption = 0,
                ExcludeDeletedUsers = true,
                SupervisorId = sessionService.Employee.Id
            };

            return View(model);
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
                Groupids = model.GroupIds,
                ExcludeUserDelete = model.ExcludeDeletedUsers ? 1 : 0,
                IsLocal = model.IsLocal,
                IsRealDate = model.IsRealDate
            };

            IList<EmployeeAttendanceModel> employeeAttendances = reportService.GetEmployeeAttendanceReport(criteria);

            return PartialView("_Details", employeeAttendances);
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
                Groupids = model.GroupIds,
                ExcludeUserDelete = model.ExcludeDeletedUsers ? 1 : 0,
                IsLocal = model.IsLocal
            };
            IList<EmployeeAttendanceModel> employeeAttendances = reportService.GetEmployeeAttendanceReport(criteria);
            excelService.Export(employeeAttendances, model.FromDate, model.ToDate);

            return new EmptyResult();
        }
    }
}
