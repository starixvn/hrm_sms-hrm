namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Time;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.Report;

    [ValidateInput(false)]
    public class ReportController : BaseController
    {
        private readonly IReportService reportService;
        private readonly ITeamService teamService;

        public ReportController(
            ISessionService sessionService,
            IReportService reportSevice,
            ITeamService teamService) : base(sessionService)
        {
            this.reportService = reportSevice;
            this.teamService = teamService;
        }

        [HRMPermission(Policy.Read, FunctionKey.ReportHome)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Attendance()
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
        public ActionResult AttendanceDetails(EmployeeAttendanceViewModel model)
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

            return PartialView("_AttendanceDetails", employeeAttendances);
        }

        [HttpGet]
        public ActionResult Timesheet()
        {
            var model = new TimesheetViewModel
            {
                Year = DateTime.Now.Year,
                EmpId = sessionService.Employee.Id
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult TimesheetDetails(TimesheetViewModel model)
        {
            var fromDate = new DateTime(model.Year, model.Month, 1);
            DateTime toDate;

            toDate = (DateTime.Now.Month == model.Month && DateTime.Now.Year == model.Year) ?
                    DateTime.Now.Date : new DateTime(model.Year, model.Month, DateTime.DaysInMonth(model.Year, model.Month));

            var criteria = new GetEmployeeAttendanceCriteria
            {
                Fromtime = fromDate,
                Totime = toDate,
                AttendanceType = 0, //show All
                EmpId = model.EmpId,
                ExcludeUserDelete = 1
            };

            IList<EmployeeAttendanceModel> timesheets = reportService.GetEmployeeAttendanceReport(criteria);

            return PartialView("_TimesheetDetails", timesheets);
        }

        public ActionResult TimeRecord(DateTime? fromDate, DateTime? toDate)
        {
            var model = new TimeRecordViewModel
            {
                FromDate = (!fromDate.HasValue || !toDate.HasValue) ? DateTime.Now.Date : fromDate.Value,
                ToDate = (!fromDate.HasValue || !toDate.HasValue) ? DateTime.Now.Date : toDate.Value,
                Teams = teamService.GetAll(),
                CurrentPage = Constants.CurrentPage,
                ItemsPerPage = Constants.ItemsPerPage
            };
            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.TimeRecord, Policy.Host);

            return View(model);
        }

        public ActionResult TimeRecordDetail(TimeRecordViewModel model)
        {
            bool hasPermissionToEdit = sessionService.HasPermissionOnFunction(FunctionKey.TimeRecord, Policy.Host);
            int? employeeId = null;
            if (!hasPermissionToEdit)
            {
                employeeId = sessionService.Employee.Id;
            }

            var criteria = new GetTimeRecordCriteria
            {
                EmpId = model.EmployeeId == 0 ? employeeId : model.EmployeeId,
                GroupIds = model.TeamIds,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                IsLocal = model.IsLocal,
                PageIndex = model.CurrentPage - 1,
                ItemsPerPage = model.ItemsPerPage
            };
            IList<TimeRecordModel> timeRecords = reportService.GetTimeRecords(criteria);
            ViewBag.HasPermission = hasPermissionToEdit;

            var pagerOption = new PagerOptionModel
            {
                CurrentPage = model.CurrentPage,
                ItemsPerPage = model.ItemsPerPage,
                TotalItem = (int)criteria.TotalItem
            };
            ViewBag.PagerOption = pagerOption;

            return PartialView("_TimeRecordDetail", timeRecords);
        }

        public ActionResult WeekendLeaveSummary(WeekendLeaveViewModel model)
        {
            if (model != null)
            {
                var today = DateTime.Today;
                model.FromDate = new DateTime(today.Year, 1, 1);
                model.ToDate = today;
                model.Teams = teamService.GetAll();
                model.DateRange = Utilities.GetDateRangeByYear();
            }

            return View(model);
        }

        public ActionResult PartialListWeekendLeaveSummary(WeekendLeaveViewModel model)
        {
            var criteria = new GetWeekendLeaveSummaryCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupId = model.TeamId,
                IsLocal = model.IsLocal,
                EmployeeId = model.EmployeeId,
                MinTotalLeave = model.MinTotalLeave,
                SupervisorId = sessionService.Employee.Id
            };

            IList<WeekendLeaveSummary> weekendLeaves = reportService.GetWeekendLeaveSummary(criteria);

            return PartialView("_ListWeekendLeaveSummary", weekendLeaves);
        }

        public ActionResult WeekendLeaveDetail(WeekendLeaveViewModel model)
        {
            if (model != null)
            {
                var today = DateTime.Today;

                model.FromDate = new DateTime(today.Year, 1, 1);
                model.ToDate = today;
                model.Teams = teamService.GetAll();
                model.DateRange = Utilities.GetDateRangeByYear();
            }

            return View(model);
        }

        public ActionResult PartialListWeekendLeaveDetail(WeekendLeaveViewModel model)
        {
            var criteria = new GetWeekendLeaveDetailCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupId = model.TeamId,
                IsLocal = model.IsLocal,
                EmployeeId = model.EmployeeId,
                LeaveTypeId = model.LeaveTypeId,
                SupervisorId = sessionService.Employee.Id
            };

            IList<WeekendLeaveDetail> weekendLeaveDetails = reportService.GetWeekendLeaveDetail(criteria);

            return PartialView("_ListWeekendLeaveDetail", weekendLeaveDetails);
        }

        public ActionResult UnpaidLeaveSummary()
        {
            var model = new LeaveSummaryViewModel
            {
                Year = DateTime.Now.Year,
                EmployeeId = -1,
                Sections = teamService.GetBySupervisor(sessionService.Employee.Id),
                LeaveType = UnpaidLeaveType.UnpaidLeave
            };
            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.CanFilterUnpaidLeaveSummary, Policy.Read)
                || sessionService.HasPermissionOnFunction(FunctionKey.CanFilterUnpaidLeaveSummary, Policy.Host);

            return View(model);
        }

        public ActionResult GetUnpaidLeaveSummary(LeaveSummaryViewModel leavingSummaryModel)
        {
            var criteria = new GetUnpaidLeaveCriteria
            {
                Year = leavingSummaryModel.Year,
                EmpId = leavingSummaryModel.EmployeeId != -1 ? new int?(leavingSummaryModel.EmployeeId) : null,
                GroupIds = leavingSummaryModel.SectionIds,
                IsLocal = (byte?)leavingSummaryModel.Category,
                SupervisorId = sessionService.Employee.Id,
                LeaveType = (int)leavingSummaryModel.LeaveType
            };
            IList<LeaveSummary> entityList = reportService.GetUnpaidLeave(criteria);
            IList<Team> sections = teamService.GetBySupervisor(sessionService.Employee.Id);
            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = reportService.GetLeaveByTeam(entityList, sections);
            ViewBag.HeaderTitle = string.Format("{0} {1}", Enumerations.GetStringValue(leavingSummaryModel.LeaveType), leavingSummaryModel.Year);
            return PartialView("_ListLeaveSummary", leaveSummaryByTeamList);
        }

        public ActionResult EmployeeSupervisor()
        {
            var model = new EmployeeAndSectionSupervisorViewModel
            {
                Sections = teamService.GetAll()
            };

            return View(model);
        }

        public ActionResult EmployeeSupervisorDetails(EmployeeAndSectionSupervisorViewModel model)
        {
            var criteria = new GetSupervisorReportCriteria
            {
                EmployeeId = model.EmployeeId,
                GroupIds = model.SectionIds,
                IsLocal = model.IsLocal,
                ActionType = model.ActionType
            };
            IList<SupervisorReportModel> supervisors = reportService.GetSupervisorReport(criteria);

            return PartialView("_EmployeeSupervisorDetails", supervisors);
        }

        public ActionResult SectionSupervisor()
        {
            var model = new EmployeeAndSectionSupervisorViewModel
            {
                Sections = teamService.GetAll()
            };

            return View(model);
        }

        public ActionResult SectionSupervisorDetails(EmployeeAndSectionSupervisorViewModel model)
        {
            var criteria = new GetSectionSupervisorCriteria
            {
                GroupIds = model.SectionIds,
            };
            IList<SectionSupervisorReportModel> supervisors = reportService.GetSectionSupervisor(criteria);

            return PartialView("_SectionSupervisorDetails", supervisors);
        }
    }
}