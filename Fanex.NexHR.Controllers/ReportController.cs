// --------------------------------------------------------------------------
// <copyright file="ReportController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Security;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.GroupSupervisor;
    using Fanex.NexHR.Criteria.Lookup;
    using Fanex.NexHR.Criteria.Report.ExtensionNumber;
    using Fanex.NexHR.Criteria.Report.Supervisor;
    using Fanex.NexHR.Criteria.Report.WorkingCalendar;
    using Fanex.NexHR.Criteria.Report.WorkingHour;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;
    using NPOI.HSSF.UserModel;

    /// <summary>
    /// Report Controller.
    /// </summary>
    [Authorize]
    public class ReportController : BaseController
    {
        private readonly IReportService _reportService;
        private readonly ITeamService _teamService;
        private readonly IExportExcelService _exportExcelService;
        private readonly IEmployeeService _employeeService;
        private readonly ILookupDataService _lookupDataService;

        public ReportController(
            IReportService reportSevice = null,
            ITeamService teamService = null,
            IExportExcelService exportExcelService = null,
            IEmployeeService employeeService = null,
            ILookupDataService lookupDataService = null)
        {
            this._reportService = reportSevice ?? new ReportService();
            this._teamService = teamService ?? new TeamService();
            this._exportExcelService = exportExcelService ?? new ExportExcelService();
            this._employeeService = employeeService ?? new EmployeeService();
            this._lookupDataService = lookupDataService ?? new LookupDataService();
        }

        public ReportController()
            : this(null, null, null, null)
        {
        }

        [Authorize(Roles = "HR")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// ActionResult Attendance.
        /// </summary>
        /// <param name="model">EmployeeAttendance ViewModel.</param>
        /// <returns>View EmployeeAttendance.</returns>
        public ActionResult Attendance(EmployeeAttendanceViewModel model)
        {
            var getAllGroupCriteria = new GetAllGroupCriteria();
            ViewBag.Groups = _teamService.GetAll(getAllGroupCriteria);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = model.CurrentUserId };
            var employee = _employeeService.GetEmployeeById(getEmployeeByIdCriteria);

            if (model.FromDate == DateTime.MinValue)
            {
                model = new EmployeeAttendanceViewModel
                {
                    FromDate = DateTime.Now,
                    ToDate = DateTime.Now,
                    GroupIds = string.Empty,
                    DateOption = 0,
                    ExcludeDeletedUsers = true,
                    IsMember = employee.IsMember(),
                    TeamName = employee.Team.Name,
                    CurrentUserId = this.CurrentUser.Id
                };
            }

            return View(model);
        }

        public ActionResult Timesheet()
        {
            var model = new TimesheetViewModel
            {
                Year = DateTime.Now.Year,
                EmpId = CurrentUser.Id
            };
            return View(model);
        }

        /// <summary>
        /// Action to get real time record.
        /// </summary>
        /// <param name="fromDate">The From Date.</param>
        /// <param name="toDate">The To Date.</param>
        /// <returns>Action Result.</returns>
        public ActionResult TimeRecord(DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                fromDate = DateTime.Now.Date;
                toDate = DateTime.Now.Date;
            }

            var getAllGroupCriteria = new GetAllGroupCriteria();
            IList<Team> teams = _teamService.GetAll(getAllGroupCriteria);

            var timeRecordViewModel = new TimeRecordViewModel
            {
                FromDate = fromDate.Value,
                ToDate = toDate.Value,
                Teams = teams
            };

            return View(timeRecordViewModel);
        }

        public ActionResult TimeRecordDetail(DateTime fromDate, DateTime toDate, string groupIds, int? employeeId = null, byte? isLocal = null)
        {
            if (!Roles.IsUserInRole("HR"))
            {
                employeeId = this.CurrentUser.Id;
            }

            IList<TimeRecordModel> timeRecords = _reportService.GetTimeRecords(employeeId, groupIds, fromDate, toDate, isLocal);

            return View(timeRecords);
        }

        /// <summary>
        /// Employee request monitoring in current month.
        /// </summary>
        /// <returns>Return View.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult HrRequestMonitoring()
        {
            var authorize = false;
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var entityList = new List<RequestsMonitoring>();

            var model = new RequestsMonitoringViewModel
            {
                FromDate = new DateTime(year, month, 1),
                ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                MaxRequest = 2,
                DateOption = 0
            };

            var getAllGroupCriteria = new GetAllGroupCriteria();
            ViewBag.Groups = _teamService.GetAll(getAllGroupCriteria);
            ViewBag.CurrentUserId = this.CurrentUser.Id;

            var listRole = CurrentUser.RoleList;
            if (listRole != null)
            {
                authorize = listRole.Any(x => x == (int)RoleSystem.HR);
            }

            if (authorize)
            {
                entityList = _reportService.GetRequestMonitoring(
                    model.FromDate,
                    model.ToDate,
                    model.MaxRequest,
                    (int)RoleSystem.HR,
                    null,
                    this.CurrentUser.Id,
                    string.Empty,
                    null).ToList();
                ViewBag.EntityList = entityList;
            }

            return View("RequestMonitoring", model);
        }

        /// <summary>
        /// ActionResult HumanResourceRequestMonitoring.
        /// </summary>
        /// <param name="currentUserId">Current user id.</param>
        /// <param name="fromDate">Query from date.</param>
        /// <param name="toDate">Query to date.</param>
        /// <param name="maxRequest">Max request.</param>
        /// <param name="empId">Employee Id.</param>
        /// <param name="group">Filter groups.</param>
        /// <param name="isLocal">Expat or local.</param>
        /// <param name="isExcel">Export to excel.</param>
        /// <returns>Action Result.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult HrRequestMonitoringDetails(
            int currentUserId,
            DateTime fromDate,
            DateTime toDate,
            int maxRequest,
            int? empId,
            string group,
            byte? isLocal,
            bool isExcel)
        {
            List<RequestsMonitoring> requests = _reportService.GetRequestMonitoring(
                fromDate,
                toDate,
                maxRequest,
                (int)RoleSystem.HR,
                empId,
                currentUserId,
                group,
                isLocal).ToList();
            ViewBag.MaxRequest = maxRequest;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.isLeader = false;

            if (isExcel)
            {
                string fileName = "HrRequestStatistic_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
                var hssfworkbook = _exportExcelService.ExportHrRequestStatistic(requests, fromDate, toDate);
                return ExcelFile(hssfworkbook, fileName);
            }

            return PartialView("RequestMonitoringDetails", requests);
        }

        /// <summary>
        /// ActionResult Verifier or Approver View Request Monitoring.
        /// </summary>
        /// <returns>Action with Role to check.</returns>
        [Authorize(Roles = "Verifier,Approver")]
        public ActionResult LeaderRequestMonitoring()
        {
            return LeaderRequestMonitoring(null);
        }

        /// <summary>
        /// ActionResult LeaderRequestMonitoring.
        /// </summary>
        /// <param name="model">RequestsMonitoring ViewModel.</param>
        /// <returns>View LeaderRequestMonitoring.</returns>
        [HttpPost]
        [Authorize(Roles = "Verifier,Approver")]
        public ActionResult LeaderRequestMonitoring(RequestsMonitoringViewModel model)
        {
            int currentUserrole = 0;
            var listRole = CurrentUser.RoleList;
            if (listRole != null)
            {
                if (listRole.Any(x => x == (int)RoleSystem.Approver))
                {
                    currentUserrole = (int)RoleSystem.Approver;
                }

                if (listRole.Any(x => x == (int)RoleSystem.Verifier))
                {
                    currentUserrole = (int)RoleSystem.Verifier;
                }
            }

            if (model == null)
            {
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                model = new RequestsMonitoringViewModel
                {
                    FromDate = new DateTime(year, month, 1),
                    ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                    MaxRequest = 1,
                    DateOption = 0
                };
            }

            ViewBag.FromDate = model.FromDate;
            ViewBag.ToDate = model.ToDate;
            ViewBag.isLeader = true;
            ViewBag.CurrentUserId = this.CurrentUser.Id;
            var getAllGroupCriteria = new GetAllGroupCriteria();
            ViewBag.Groups = _teamService.GetAll(getAllGroupCriteria);

            ViewBag.EntityList = new List<RequestsMonitoring>();
            ViewBag.EntityList = _reportService.GetRequestMonitoring(
                model.FromDate,
                model.ToDate,
                model.MaxRequest,
                currentUserrole,
                null,
                this.CurrentUser.Id,
                string.Empty,
                null);

            return View("RequestMonitoring", model);
        }

        /// <summary>
        /// ActionResult TotalWorkingHours.
        /// </summary>
        /// <param name="teamIds">Team's id.</param>
        /// <param name="employee">Employee name.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">Count to Date.</param>
        /// <param name="dateOption">Date Options.</param>
        /// <param name="employeeId">Employee Id.</param>
        /// <param name="isExportExcel">Is exported to Excel.</param>
        /// <param name="isLocal">Is Expat or local.</param>
        /// <returns>View TotalWorkingHours.</returns>
        public ActionResult TotalWorkingHours(
            string teamIds,
            string employee,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int? dateOption = null,
            int? employeeId = null,
            bool isExportExcel = false,
            byte? isLocal = null)
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;

            var model = new TotalWorkingHoursViewModel
            {
                TeamIds = teamIds == null ? string.Empty : teamIds,
                Employee = employee,
                FromDate = fromDate.HasValue ? fromDate.Value : new DateTime(year, month, 1),
                ToDate = toDate.HasValue ? toDate.Value : new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                EmployeeId = employeeId.HasValue ? employeeId.Value : -1,
                IsExportExcel = isExportExcel,
                DateOption = dateOption.HasValue ? dateOption.Value : 5,
                IsLocal = isLocal
            };

            return TotalWorkingHours(model);
        }

        /// <summary>
        /// ActionResult TotalWorkingHours.
        /// </summary>
        /// <param name="model">TotalWorkingHours ViewModel.</param>
        /// <returns>View of TotalWorkingHours.</returns>
        [HttpPost]
        public ActionResult TotalWorkingHours(TotalWorkingHoursViewModel model)
        {
            if (model == null)
            {
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                model = new TotalWorkingHoursViewModel
                {
                    FromDate = new DateTime(year, month, 1),
                    ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                    DateOption = 5,
                    IsExportExcel = false
                };
            }

            if (model.IsExportExcel)
            {
                ExportController export = new ExportController();
                return export.TotalWorkingHours(model);
            }

            var getAllGroupCriteria = new GetAllGroupCriteria();
            ViewBag.Groups = _teamService.GetAll(getAllGroupCriteria);
            ViewBag.TeamIds = model.TeamIds;

            var getTotalWorkingHourCriteria = new GetReportWorkingHourSummaryCriteria()
            {
                Fromdate = model.FromDate,
                Todate = model.ToDate,
                EmpId = (model.EmployeeId != -1 & model.EmployeeId != 0) ? new int?(model.EmployeeId) : null,
                GroupIds = model.TeamIds,
                IsLocal = model.IsLocal
            };
            var totalWorkingHours = _reportService.GetWorkingHoursList(getTotalWorkingHourCriteria);

            ViewBag.EntityList = totalWorkingHours;

            return View(model);
        }

        /// <summary>
        /// Totals the working hour monthly.
        /// </summary>
        /// <param name="month">The month will be filtered.</param>
        /// <param name="year">The year will be filtered.</param>
        /// <param name="teamIds">The team ids.</param>
        /// <param name="employee">The employee.</param>
        /// <param name="employeeId">The employee identifier.</param>
        /// <param name="isExportExcel">If set to <c>true</c> [is export excel].</param>
        /// <param name="isLocal">Expat or Local.</param>
        /// <returns>The view of TotalWorkingHourMonthly.</returns>
        public ActionResult TotalWorkingHourMonthly(
            int? month,
            int? year,
            string teamIds,
            string employee,
            int? employeeId,
            bool isExportExcel = false,
            byte? isLocal = null)
        {
            if (!month.HasValue)
            {
                month = DateTime.Now.Month;
            }

            if (!year.HasValue)
            {
                year = DateTime.Now.Year;
            }

            var model = new TotalWorkingHourMonthlyViewModel
            {
                TeamIds = teamIds == null ? string.Empty : teamIds,
                Employee = employee,
                Month = new DateTime(year.Value, month.Value, 1),
                EmployeeId = string.IsNullOrEmpty(employee) ? null : employeeId,
                IsExportExcel = isExportExcel,
                IsLocal = isLocal
            };

            var getWorkingHourMonthlyCriteria = new GetTotalWorkingHourMonthlyCriteria()
            {
                EmpId = string.IsNullOrEmpty(employee) ? null : employeeId,
                GroupIds = model.TeamIds,
                IsLocal = isLocal,
                ReportMonth = model.Month
            };
            var totalWorkingHourMonthly = _reportService.GetTotalWorkingHourMonthly(getWorkingHourMonthlyCriteria);
            var firstRecord = totalWorkingHourMonthly != null && totalWorkingHourMonthly.Any() ? totalWorkingHourMonthly[0] : null;

            if (model.IsExportExcel)
            {
                string fileName = "TotalWorkingHourMonthly_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
                var hssfworkbook = _exportExcelService.ExportTotalWorkingHourMonthly(totalWorkingHourMonthly, firstRecord);
                return ExcelFile(hssfworkbook, fileName);
            }

            ViewBag.EntityList = totalWorkingHourMonthly;
            ViewBag.FirstRecord = firstRecord;
            var getAllGroupCriteria = new GetAllGroupCriteria();
            ViewBag.Groups = _teamService.GetAll(getAllGroupCriteria);
            ViewBag.TeamIds = model.TeamIds;

            return View(model);
        }

        /// <summary>
        /// Total Working Hour Weekly.
        /// </summary>
        /// <param name="startDate">Query from Date.</param>
        /// <param name="endDate">Query to Date.</param>
        /// <param name="employee">Employee Display name.</param>
        /// <param name="teamIds">List of team id.</param>
        /// <param name="employeeId">Employee id.</param>
        /// <param name="isLocal">Expat or local.</param>
        /// <param name="requestData">Request data or not.</param>
        /// <param name="isExcel">Is exported excel.</param>
        /// <returns>The view of TotalWorkingHourWeekly.</returns>
        public ActionResult TotalWorkingHourWeekly(
            DateTime? startDate,
            DateTime? endDate,
            string employee,
            string teamIds,
            int? employeeId,
            byte? isLocal = null,
            bool requestData = false,
            bool isExcel = false)
        {
            if (startDate == null && endDate == null)
            {
                var dayDiff = DayOfWeek.Monday - DateTime.Now.DayOfWeek;
                startDate = DateTime.Now.AddDays(dayDiff);
                endDate = startDate.Value.AddDays(6);
            }

            var getAllGroupCriteria = new GetAllGroupCriteria();
            ViewBag.Groups = _teamService.GetAll(getAllGroupCriteria);
            ViewBag.TeamIds = teamIds;

            var model = new TotalWorkingHourWeeklyViewModel
            {
                EmployeeId = employeeId,
                Employee = employee,
                StartDate = startDate.Value.ToString("dd MMM, yyyy"),
                EndDate = endDate.Value.ToString("dd MMM, yyyy"),
                IsLocal = isLocal,
                TeamIds = teamIds
            };

            if (requestData)
            {
                var getTotalWorkingHourWeeklyCriteria = new GetTotalWorkingHourWeeklyCriteria()
                {
                    EmpId = employeeId,
                    Fromdate = startDate.Value,
                    Todate = endDate.Value,
                    GroupIds = teamIds,
                    IsLocal = isLocal
                };
                IList<TotalWorkingHourMonthly> totalWorkingHourWeekly = _reportService.GetTotalWorkingHourWeekly(getTotalWorkingHourWeeklyCriteria);
                TotalWorkingHourMonthly firstRecord = totalWorkingHourWeekly != null && totalWorkingHourWeekly.Any() ? totalWorkingHourWeekly[0] : null;
                ViewBag.EntityList = totalWorkingHourWeekly;
                ViewBag.FirstRecord = firstRecord;

                if (isExcel)
                {
                    string fileName = "TotalWorkingHourMonthly_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
                    var hssfworkbook = _exportExcelService.ExportTotalWorkingHourMonthly(totalWorkingHourWeekly, firstRecord);
                    return ExcelFile(hssfworkbook, fileName);
                }
            }

            return View(model);
        }

        /// <summary>
        /// Working Calendar Summary.
        /// </summary>
        /// <param name="employeeId">Employee id.</param>
        /// <param name="employee">Employee full name.</param>
        /// <param name="fromDate">Query from date.</param>
        /// <param name="toDate">Query to date.</param>
        /// <param name="teamIds">List of tem id.</param>
        /// <param name="jobTitleId">Job title id.</param>
        /// <param name="isLocal">Expat Or Local.</param>
        /// <param name="isExcel">View Only Or Export Excel.</param>
        /// <returns>Action Result.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public ActionResult WorkingCalendarSummary(
             int? employeeId,
             string employee,
             DateTime? fromDate,
             DateTime? toDate,
             string teamIds,
             string jobTitleId,
             byte? isLocal = null,
             bool isExcel = false)
        {
            if (fromDate == null && toDate == null)
            {
                fromDate = DateTime.Now.Date;
                toDate = DateTime.Now.Date;
            }

            var model = new WorkingCalendarSummaryViewModel()
            {
                Employee = employee,
                EmployeeId = employeeId,
                FromDate = fromDate.Value.ToString("dd MMM, yyyy"),
                ToDate = toDate.Value.ToString("dd MMM, yyyy"),
                IsLocal = isLocal,
                JobTitleId = jobTitleId,
                TeamIds = teamIds
            };
            var getAllGroupCriteria = new GetAllGroupCriteria();
            ViewBag.Groups = _teamService.GetAll(getAllGroupCriteria);
            ViewBag.DefaultFromDate = model.FromDate;
            ViewBag.DefaultToDate = model.ToDate;
            var getAllJobTitleCriteria = new GetAllLookupDataCriteria 
            { 
                LookupCode = LookupCodename.JobTitle
            };
            IList<LookupData> listJobTitle = _lookupDataService.GetAll(getAllJobTitleCriteria);
            ViewBag.JobTitles = listJobTitle.Select(j => new SelectListItem
            {
                Text = j.Name,
                Value = j.Id.ToString(),
                Selected = j.Id.ToString() == string.Empty
            }).ToList();
            if (isExcel)
            {
                var criteria = new GetEmployeeWorkingCalendarByDateCriteria()
                {
                    EmployeeId = model.EmployeeId,
                    FromDate = fromDate.Value,
                    ToDate = toDate.Value,
                    GroupIds = model.TeamIds,
                    IsLocal = model.IsLocal,
                    JobTitleId = model.JobTitleId
                };
                List<WorkingCalendarSummary> calendars = _reportService.CalendarSummaryList(criteria).ToList();
                string fileName = "TotalWorkingHourSummary_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
                var hssfworkbook = _exportExcelService.ExportWorkingCalendarSummary(calendars, fromDate.Value, toDate.Value);
                return ExcelFile(hssfworkbook, fileName);
            }

            return View(model);
        }

        /// <summary>
        /// ActionResult LeaveSummary.
        /// </summary>
        /// <param name="teamIds">Team's id.</param>
        /// <param name="employee">Employee name.</param>
        /// <param name="year">Current Year.</param>
        /// <param name="employeeId">Employee's Id.</param>
        /// <param name="minAnnualBalance">Min Annual Balance.</param>
        /// <returns>View LeaveSummary.</returns>
        public ActionResult LeaveSummary(
            string teamIds,
            string employee,
            int? year = null,
            int? employeeId = null,
            int? minAnnualBalance = null)
        {
            var model = new LeaveSummaryViewModel
            {
                Year = year.HasValue ? year.Value : DateTime.Now.Year,
                TeamIds = teamIds,
                Employee = employee,
                EmployeeId = employeeId.HasValue ? employeeId.Value : -1,
                MinAnnualBalance = minAnnualBalance.HasValue ? minAnnualBalance.Value : 0
            };

            return LeaveSummary(model);
        }

        /// <summary>
        /// ActionResult LeaveSummary.
        /// </summary>
        /// <param name="model">LeaveSummary ViewModel.</param>
        /// <returns>View LeaveSummary.</returns>
        [HttpPost]
        public ActionResult LeaveSummary(LeaveSummaryViewModel model)
        {
            if (model == null)
            {
                model = new LeaveSummaryViewModel
                {
                    Year = DateTime.Now.Year,
                    EmployeeId = -1
                };
            }

            IList<LeaveSummary> entityList = _reportService.GetAnnualLeaveBalanceReport(
                model.Year,
                empId: model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                groupIds: model.TeamIds,
                isLocal: null);

            IList<Team> teams = new List<Team>();
            var getAllGroupCriteria = new GetAllGroupCriteria();
            teams = _teamService.GetAll(getAllGroupCriteria);

            ViewBag.Groups = teams;
            ViewBag.TeamIds = model.TeamIds;
            ViewBag.EntityList = entityList;

            var leaveSummaryByTeamList = _reportService.GetAnnualLeaveByTeam(entityList, teams);

            ViewBag.LeaveSummaryByTeamList = leaveSummaryByTeamList;

            return View(model);
        }

        /// <summary>
        /// Extension Number Index.
        /// </summary>
        /// <returns>View ExtensionNo.</returns>
        public ActionResult ExtensionNo()
        {
            var authorize = false;
            var listRole = this.CurrentUser.RoleList;
            if (listRole != null)
            {
                authorize = listRole.Any(x => x == (int)RoleSystem.HR);
            }

            ViewBag.Authorize = authorize;
            IEnumerable<ExtensionNoModel> lstEntity = _reportService.GetAllExtensionNo(new GetAllExtensionNumberCriteria());
            var model = (from item in lstEntity
                         group item by item.TeamName into g
                         select new ExtensionNumberViewModel() { Name = g.Key, ListExt = g.ToList() }).ToList();

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult AddNewMember()
        {
            return View();
        }

        /// <summary>
        /// Add new extension member.
        /// </summary>
        /// <param name="model">The extension number model.</param>
        /// <returns>The view after add new number.</returns>
        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult AddNewMember(ExtensionNoModel model)
        {
            CheckExternalEmployee(model);
            if (ModelState.IsValid)
            {
                var criteria = new InsertExtensionNumberCriteria
                {
                    FullName = model.FullName,
                    NickName = model.NickName,
                    ExtensionNo = model.ExtensionNo,
                    IsInternal = model.IsInternal,
                    EmpID = model.EmpId,
                    GroupID = model.TeamId,
                };

                CommandResult commandResult = _reportService.AddNewExtensionNoMember(criteria);
                ViewBag.Requested = commandResult.Code == 0;
            }

            return View();
        }

        /// <summary>
        /// Edit extension number.
        /// </summary>
        /// <param name="isInternal">The boolean is internal or not.</param>
        /// <param name="empExtID">The extension Id.</param>
        /// <returns>THe Action Result.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult EditExtensionNumber(bool isInternal, int empExtID)
        {
            var getExtensionNoByIdcriteria = new GetExtensionNumberMemberByIdCriteria
            {
                IsInternal = isInternal,
                EmployeeExtensionNoId = empExtID
            };
            ExtensionNoModel model = _reportService.GetEmployeeExtensionNoById(getExtensionNoByIdcriteria);
            var getAllExtensionNoGroupInfo = new GetExtensionNumberGroupInfoCriteria
            {
                IsInternal = isInternal,
                GroupID = null
            };
            IEnumerable<ExtensionNoModel> groups = _reportService.GetAllExtensionGroupInfo(getAllExtensionNoGroupInfo);

            if (groups != null)
            {
                ViewBag.Group = groups.Select(x => new SelectListItem
                {
                    Text = x.TeamName,
                    Value = x.TeamId.ToString()
                }).ToList();
            }

            return View(model);
        }

        /// <summary>
        /// ActionResult EditExtensionNumber.
        /// </summary>
        /// <param name="model">ExtensionNoModel model.</param>
        /// <returns>View Index when edit successful.</returns>
        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult EditExtensionNumber(ExtensionNoModel model)
        {
            CheckExternalEmployee(model);
            var getAllExtensionNoGroupInfo = new GetExtensionNumberGroupInfoCriteria
            {
                IsInternal = model.IsInternal,
                GroupID = null
            };
            IEnumerable<ExtensionNoModel> group = _reportService.GetAllExtensionGroupInfo(getAllExtensionNoGroupInfo);
            ViewBag.Group = group.Select(x => new SelectListItem
            {
                Text = x.TeamName,
                Value = x.TeamId.ToString()
            }).ToList();
            if (ModelState.IsValid)
            {
                var updateExtensionNoCriteria = new UpdateExtensionNumberMemberCriteria
                {
                    FullName = model.FullName,
                    NickName = model.NickName,
                    ExtensionNo = model.ExtensionNo,
                    IsInternal = model.IsInternal,
                    ExtID = model.EmpExtID,
                    EmpID = model.EmpId,
                    GroupID = model.TeamId
                };
                CommandResult commandResult = _reportService.UpdateExtensionNoMember(updateExtensionNoCriteria);
                ViewBag.Updated = commandResult.Code == 0;
                return RedirectToAction("ExtensionNo");
            }
            else
            {
                return View();
            }
        }

        public ActionResult ManageExternalGroup()
        {
            var getAllExtensionNoGroupInfo = new GetExtensionNumberGroupInfoCriteria
            {
                IsInternal = false,
                GroupID = null
            };
            IEnumerable<ExtensionNoModel> group = _reportService.GetAllExtensionGroupInfo(getAllExtensionNoGroupInfo);
            return View(group);
        }

        /// <summary>
        /// Method excel report for Overtime.
        /// </summary>
        /// <param name="month">Request month.</param>
        /// <param name="year">Request year.</param>
        /// <returns>View OvertimeReport.</returns>
        public ActionResult OvertimeReport(int? month, int? year)
        {
            if (!month.HasValue)
            {
                month = DateTime.Now.Month;
            }

            if (!year.HasValue)
            {
                year = DateTime.Now.Year;
            }

            ViewBag.Month = month.Value;
            ViewBag.Year = year.Value;

            return View();
        }

        public ActionResult EmployeeSupervisor()
        {
            var getAllGroupCriteria = new GetAllGroupCriteria();
            ViewBag.Groups = _teamService.GetAll(getAllGroupCriteria);
            return View();
        }

        /// <summary>
        /// Employee Supervisor Details.
        /// </summary>
        /// <param name="employeeId">Employee Id.</param>
        /// <param name="groups">Filter by group.</param>
        /// <param name="supervisorType">Supervisor type.</param>
        /// <param name="isExcel">Export Excel Or query data.</param>
        /// <param name="isLocal">Expat or local.</param>
        /// <returns>Action Result.</returns>
        public ActionResult EmployeeSupervisorDetails(int? employeeId, string groups, int? supervisorType, bool? isExcel, byte? isLocal = null)
        {
            var getSupervisorReportCriteria = new GetSupervisorReportCriteria()
            {
                EmployeeId = employeeId,
                GroupIds = groups,
                IsLocal = isLocal,
                ActionType = supervisorType
            };
            List<SupervisorReportGroupModel> supervisors = _reportService.GetSupervisorReport(getSupervisorReportCriteria);

            if (isExcel.Value)
            {
                string fileName = "EmployeeSupervisor_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
                var hssfworkbook = _exportExcelService.ExportEmployeeSupervisor(supervisors);

                return ExcelFile(hssfworkbook, fileName);
            }
            else
            {
                return PartialView(supervisors);
            }
        }

        public ActionResult TeamSupervisor()
        {
            var getAllGroupCriteria = new GetAllGroupCriteria();
            ViewBag.Groups = _teamService.GetAll(getAllGroupCriteria);
            return View();
        }

        /// <summary>
        /// Method Get Team Supervisors Details.
        /// </summary>
        /// <param name="groups">Query according to specific groups.</param>
        /// <param name="isExcel">Is Export Excel.</param>
        /// <returns>Action Result.</returns>
        public ActionResult TeamSupervisorDetails(string groups, bool? isExcel)
        {
            var criteria = new GetAllTeamSupervisorCriteria()
            {
                GroupIds = groups,
            };
            List<TeamSupervisorReportModel> supervisors = _reportService.GetAllTeamSupervisor(criteria);

            if (isExcel.Value)
            {
                string fileName = "TeamSupervisor_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
                var hssfworkbook = _exportExcelService.ExportTeamSupervisor(supervisors);
                return ExcelFile(hssfworkbook, fileName);
            }
            else
            {
                return PartialView(supervisors);
            }
        }

        /// <summary>
        /// Method Check External Employee.
        /// </summary>
        /// <param name="model">ExtensionNoModel model.</param>
        private void CheckExternalEmployee(ExtensionNoModel model)
        {
            if (model.IsInternal == false)
            {
                foreach (var modelError in ModelState)
                {
                    string propertyName = modelError.Key;

                    if (propertyName.Contains("EmpId"))
                    {
                        ModelState[propertyName].Errors.Clear();
                    }
                }
            }
        }

        private ActionResult ExcelFile(HSSFWorkbook hssfworkbook, string fileName)
        {
            MemoryStream output = new MemoryStream();
            hssfworkbook.Write(output);
            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
        }
    }
}
