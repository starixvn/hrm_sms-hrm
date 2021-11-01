namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Session;

    [ValidateInput(false)]
    public class ReportController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IReportService reportService;
        private readonly ITeamService teamService;
        private readonly ICommonDataService commonDataService;

        public ReportController(
            ISessionService sessionService,
            IUserControlSession userControlSession,
            IReportService reportSevice,
            ICommonDataService commonDataService,
            ITeamService teamService) : base(sessionService)
        {
            this.userControlSession = userControlSession;
            this.reportService = reportSevice;
            this.teamService = teamService;
            this.commonDataService = commonDataService;
        }

        [HRMPermission(Policy.Read, FunctionKey.ReportHome)]
        public ActionResult Index()
        {
            IEnumerable<FunctionPermission> functions = userControlSession.Functions.Where(x => x.IsRead || x.IsHost);
            return View(functions);
        }

        [HttpGet]
        public ActionResult Timesheet()
        {
            var model = new TimesheetViewModel
            {
                Year = DateTime.Now.Year,
                EmpId = sessionService.Employee.Id,
                IsHavePrivilegeCreateCheckInOutRecord = IsPrivilegeCreateCheckInOut()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult TimesheetDetails(TimesheetViewModel model)
        {
            var today = DateTime.Today;
            var fromDate = new DateTime(model.Year, model.Month, 1);
            var toDate = (today.Month == model.Month && today.Year == model.Year) ?
                    today.Date : new DateTime(model.Year, model.Month, DateTime.DaysInMonth(model.Year, model.Month));

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

        public ActionResult TimeRecord(TimeRecordViewModel model)
        {
            model.CurrentPage = Constants.CurrentPage;
            model.ItemsPerPage = Constants.ItemsPerPage;
            model.TeamIds = string.Join(",", teamService.GetBySupervisor(sessionService.Employee.Id).Select(x => x.Id));

            ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.TimeRecord, Policy.Read)
                || userControlSession.HasPermissionOnFunction(FunctionKey.TimeRecord, Policy.Host);

            return View(model);
        }

        [HttpPost]
        public ActionResult TimeRecordDetail(TimeRecordViewModel model)
        {
            int? employeeId = null;
            if (!userControlSession.HasPermissionOnFunction(FunctionKey.TimeRecord, Policy.Read)
                && !userControlSession.HasPermissionOnFunction(FunctionKey.TimeRecord, Policy.Host))
            {
                employeeId = sessionService.Employee.Id;
            }
            var criteria = new GetTimeRecordCriteria
            {
                EmpId = model.EmployeeId == 0 ? employeeId : model.EmployeeId,
                TeamIds = model.TeamIds,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                IsLocal = model.IsLocal,
                PageIndex = model.CurrentPage - 1,
                ItemsPerPage = model.ItemsPerPage,
                SupervisorId = sessionService.Employee.Id
            };
            IList<TimeRecordModel> timeRecords = reportService.GetTimeRecords(criteria);
            ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.TimeRecord, Policy.Host);

            var pagerOption = new PagerOptionModel
            {
                CurrentPage = model.CurrentPage,
                ItemsPerPage = model.ItemsPerPage,
                TotalItem = criteria.TotalItem ?? 0
            };
            ViewBag.PagerOption = pagerOption;

            return PartialView("_TimeRecordDetail", timeRecords);
        }

        public ActionResult WeekendLeaveSummary(WeekendLeaveViewModel model)
        {
            if (model != null)
            {
                var currentYear = DateTime.Today.Year;
                model.FromDate = new DateTime(currentYear, 1, 1);
                model.ToDate = new DateTime(currentYear + 1, 1, 1).AddDays(-1);
                model.DateRange = Utilities.GetDateRangeByYear();
            }

            return View("WeekendLeaveSummary", model);
        }

        public ActionResult PartialListWeekendLeaveSummary(WeekendLeaveViewModel model)
        {
            var criteria = new GetWeekendLeaveSummaryCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                TeamId = model.TeamId,
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
                model.DateRange = Utilities.GetDateRangeByYear();
            }

            return View("WeekendLeaveDetail", model);
        }

        public ActionResult PartialListWeekendLeaveDetail(WeekendLeaveViewModel model)
        {
            var criteria = new GetWeekendLeaveDetailCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                TeamId = model.TeamId,
                IsLocal = model.IsLocal,
                EmployeeId = model.EmployeeId,
                LeaveTypeId = model.LeaveTypeId,
                SupervisorId = sessionService.Employee.Id
            };

            IList<WeekendLeaveDetail> weekendLeaveDetails = reportService.GetWeekendLeaveDetail(criteria);

            return PartialView("_ListWeekendLeaveDetail", weekendLeaveDetails);
        }

        [HRMPermission(Policy.Read, FunctionKey.UnpaidLeaveSummary)]
        public ActionResult UnpaidLeaveSummary()
        {
            var model = new LeaveSummaryViewModel
            {
                Year = DateTime.Now.Year,
                EmployeeId = -1,
                TeamIds = string.Join(",", teamService.GetAll().Select(x => x.Id))
            };

            ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.CanFilterUnpaidLeaveSummary, Policy.Read)
                || userControlSession.HasPermissionOnFunction(FunctionKey.CanFilterUnpaidLeaveSummary, Policy.Host);

            return View(model);
        }

        [HRMPermission(Policy.Read, FunctionKey.UnpaidLeaveSummary)]
#pragma warning disable S1541 // Methods and properties should not be too complex
#pragma warning disable S1067 // Expressions should not be too complex
        public ActionResult GetUnpaidLeaveSummary(LeaveSummaryViewModel model)
#pragma warning restore S1541 // Methods and properties should not be too complex
        {
            var criteria = new GetUnpaidLeaveCriteria
            {
                Year = model.Year,
                EmpId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                TeamIds = model.TeamIds,
                IsLocal = (byte?)model.Category,
                SupervisorId = sessionService.Employee.Id,
                LeaveType = model.LeaveType
            };

            string[] leaveTypeIds = !string.IsNullOrWhiteSpace(model.LeaveType) ? model.LeaveType.Split(',') : new string[] { };
            IList<LeaveType> leaveTypes = commonDataService.GetLeaveType();
            string leaveType = string.Join(", ", leaveTypes.Where(x => leaveTypeIds.Any(y => x.Id.ToString() == y)).Select(x => x.Name));
            ViewBag.HeaderTitle = string.Format("{0} {1}", leaveType, model.Year);

            var leaveSumaries = reportService.GetUnpaidLeave(criteria);
            if (model.Period.Equals("1"))
            {
                model.LeaveSumaries = leaveSumaries.Where(x => x.Jan > 0 || x.Feb > 0 || x.Mar > 0 || x.Apr > 0 || x.May > 0 || x.Jun > 0).ToList();
            }
            else if (model.Period.Equals("2"))
            {
                model.LeaveSumaries = leaveSumaries.Where(x => x.Jul > 0 || x.Aug > 0 || x.Sep > 0 || x.Oct > 0 || x.Nov > 0 || x.Dec > 0).ToList();
            }
            else
            {
                model.LeaveSumaries = leaveSumaries;
            }

            return PartialView("_UnpaidLeaveSummaryDetails", model);
        }
#pragma warning restore S1067 // Expressions should not be too complex

        public ActionResult EmployeeSupervisor()
        {
            var model = new EmployeeAndTeamSupervisorViewModel();
            model.TeamIds = model.TeamIds ?? string.Join(",", teamService.GetAll().Select(x => x.Id));

            return View("EmployeeSupervisor", model);
        }

        public ActionResult EmployeeSupervisorDetails(EmployeeAndTeamSupervisorViewModel model)
        {
            var criteria = new GetSupervisorReportCriteria
            {
                EmployeeId = model.EmployeeId,
                TeamIds = model.TeamIds,
                IsLocal = model.IsLocal,
                ActionType = model.ActionType
            };
            IList<SupervisorReportModel> supervisors = reportService.GetSupervisorReport(criteria);

            return PartialView("_EmployeeSupervisorDetails", supervisors);
        }

        public ActionResult TeamSupervisor()
        {
            var model = new EmployeeAndTeamSupervisorViewModel();
            model.TeamIds = model.TeamIds ?? string.Join(",", teamService.GetAll().Select(x => x.Id));

            return View("TeamSupervisor", model);
        }

        public ActionResult TeamSupervisorDetails(EmployeeAndTeamSupervisorViewModel model)
        {
            var criteria = new GetTeamSupervisorCriteria
            {
                TeamIds = model.TeamIds,
            };
            IList<TeamSupervisorReportModel> supervisors = reportService.GetTeamSupervisor(criteria);

            return PartialView("_TeamSupervisorDetails", supervisors);
        }

        public CustomJsonResult GetUnpaidLeaveType()
        {
            IList<LeaveType> leaveTypes = commonDataService.GetLeaveType();
            IEnumerable<MultiSelectViewModel> multiSelectViewModel = leaveTypes.Where(x => x.Id == (int)LeaveTypes.MaternityLeave
                || x.Id == (int)LeaveTypes.PaternityLeave
                || x.Id == (int)LeaveTypes.UnpaidLeave
                || x.Id == (int)LeaveTypes.UnpaidLeaveInsurance).Select(u => new MultiSelectViewModel
                {
                    Id = u.Id,
                    Text = u.Name,
                    IsLeaf = true,
                });

            return new CustomJsonResult { Data = multiSelectViewModel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private bool IsPrivilegeCreateCheckInOut()
        {
            var criteria = new GetPrivilegeCreateCheckInOutRecordCriteria
            {
                EmpId = sessionService.Employee.Id
            };

            var isPrivilegeCreateCheckInOut = reportService.GetIsPrivilegeCreateCheckInOut(criteria);
            return isPrivilegeCreateCheckInOut;
        }
    }
}