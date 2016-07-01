namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Lookup;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Criteria.Time;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.ExportExcel;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.AnnualLeave;
    using Fanex.NexHR.ViewModels.Calendar;
    using Fanex.NexHR.ViewModels.Report;
    using Fanex.NexHR.ViewModels.Request;
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;
    using NPOI.HSSF.UserModel;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;

    [ValidateInput(false)]
    public class ReportController : BaseController
    {
        private readonly IReportService reportService;
        private readonly ITeamService teamService;
        private readonly IExportExcelService exportExcelService;
        private readonly IEmployeeService employeeService;
        private readonly ILookupDataService lookupDataService;
        private IExportService exportService;

        public ReportController()
            : this(null, null, null, null, null)
        {
        }

        public ReportController(
            IReportService reportSevice = null,
            ITeamService teamService = null,
            IExportExcelService exportExcelService = null,
            IEmployeeService employeeService = null,
            ILookupDataService lookupDataService = null)
        {
            this.reportService = reportSevice ?? new ReportService();
            this.teamService = teamService ?? new TeamService();
            this.exportExcelService = exportExcelService ?? new ExportExcelService();
            this.employeeService = employeeService ?? new EmployeeService();
            this.lookupDataService = lookupDataService ?? new LookupDataService();
        }

        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Read, FunctionKey.ReportHome)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Attendance()
        {
            var getGroupCriteria = new GetGroupBySupervisorCriteria()
            {
                SupervisorId = CurrentUser.Id
            };
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = CurrentUser.Id };

            ViewBag.Groups = teamService.GetBySupervisor(getGroupCriteria);
            var model = new EmployeeAttendanceViewModel
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                GroupIds = string.Empty,
                DateOption = 0,
                ExcludeDeletedUsers = true,
                SupervisorId = CurrentUser.Id
            };

            Employee employee = employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            if (employee != null)
            {
                model.TeamName = employee.Team.Name;
                model.GroupIds = employee.Team.Id.ToString();
                model.IsMember = employee.IsMember();
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

        public ActionResult TimeRecord(DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                fromDate = DateTime.Now.Date;
                toDate = DateTime.Now.Date;
            }

            IList<Team> teams = GetAllTeam();

            var model = new TimeRecordViewModel
            {
                FromDate = fromDate.Value,
                ToDate = toDate.Value,
                Teams = teams,
                CurrentPage = Constants.CurrentPage,
                ItemsPerPage = Constants.ItemsPerPage
            };
            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.TimeRecord, Policy.Edit);

            return View(model);
        }

        public ActionResult TimeRecordDetail(TimeRecordViewModel model)
        {
            bool hasPermissionToEdit = CurrentUser.HasPermission(FunctionKey.TimeRecord, Policy.Edit);
            int? employeeId = null;
            if (!hasPermissionToEdit)
            {
                employeeId = this.CurrentUser.Id;
            }

            var criteria = new GetTimeRecordCriteria()
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

            var pagerOption = new PagerOptionModel()
            {
                CurrentPage = model.CurrentPage,
                ItemsPerPage = model.ItemsPerPage,
                TotalItem = (int)criteria.TotalItem
            };
            ViewBag.PagerOption = pagerOption;

            return PartialView("_TimeRecordDetail", timeRecords);
        }

        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Read, FunctionKey.VerifyRequestOfEmployee, FunctionKey.ApproveRequestOfEmployee)]
        public ActionResult HRRequestMonitoring()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            string viewRole = ((int)RoleSystem.HR).ToString();
            var model = new RequestsMonitoringViewModel
            {
                FromDate = new DateTime(year, month, 1),
                ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                MaxRequest = 2,
                DateOption = 0
            };

            ViewBag.Groups = GetAllTeam();
            ViewBag.ViewRole = viewRole;

            return View("RequestMonitoring", model);
        }

        /// [Authorize(Roles = "HR,Verifier,Approver")]
        [HRAuthorize(
            Policy.Read,
            FunctionKey.VerifyRequestOfEmployee,
            FunctionKey.ApproveRequestOfEmployee,
            FunctionKey.VerifyRequestOfMember,
            FunctionKey.ApproveRequestOfMember)]
        public ActionResult HRRequestMonitoringDetails(
            DateTime fromDate,
            DateTime toDate,
            int maxRequest,
            int? empId,
            string group,
            byte? isLocal,
            bool isExcel,
            string viewRole)
        {
            var getRequestMonitoringCriteria = new GetRequestMonitoringCriteria
            {
                FromDate = fromDate,
                ToDate = toDate,
                MaxRequest = maxRequest,
                Role = viewRole,
                EmployeeId = empId,
                LoginUserId = CurrentUser.Id,
                GroupIds = group,
                IsLocal = isLocal
            };
            List<RequestsMonitoring> requests = reportService.GetRequestMonitoring(getRequestMonitoringCriteria).ToList();
            ViewBag.MaxRequest = maxRequest;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            if (viewRole == ((int)RoleSystem.HR).ToString())
            {
                ViewBag.isLeader = false;
            }
            else
            {
                ViewBag.isLeader = true;
            }

            if (isExcel)
            {
                string fileName = "HrRequestStatistic_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
                var hssfworkbook = exportExcelService.ExportHRRequestStatistic(requests, fromDate, toDate);
                return ExcelFile(hssfworkbook, fileName);
            }

            return PartialView("RequestMonitoringDetails", requests);
        }

        /// [Authorize(Roles = "Verifier,Approver")]
        [HRAuthorize(Policy.Read, FunctionKey.VerifyRequestOfMember, FunctionKey.ApproveRequestOfMember)]
        public ActionResult LeaderRequestMonitoring()
        {
            string viewRole = string.Join(",", CurrentUser.NE_Roles.Select(i => i.RoleId));
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var model = new RequestsMonitoringViewModel
            {
                FromDate = new DateTime(year, month, 1),
                ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                MaxRequest = 1,
                DateOption = 0
            };

            ViewBag.FromDate = model.FromDate;
            ViewBag.ToDate = model.ToDate;
            ViewBag.isLeader = true;
            ViewBag.ViewRole = viewRole;
            ViewBag.Groups = GetAllTeam();

            return View("RequestMonitoring", model);
        }

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

            ViewBag.Groups = GetAllTeam();
            ViewBag.TeamIds = model.TeamIds;

            var getTotalWorkingHourCriteria = new GetReportWorkingHourSummaryCriteria()
            {
                Fromdate = model.FromDate,
                Todate = model.ToDate,
                EmpId = (model.EmployeeId != -1 & model.EmployeeId != 0) ? new int?(model.EmployeeId) : null,
                GroupIds = model.TeamIds,
                IsLocal = model.IsLocal
            };
            var totalWorkingHours = reportService.GetWorkingHoursList(getTotalWorkingHourCriteria);

            ViewBag.EntityList = totalWorkingHours;
            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.VerifyRequestOfEmployee, Policy.Read)
                || CurrentUser.HasPermission(FunctionKey.ApproveRequestOfEmployee, Policy.Read);

            return View(model);
        }

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
            var totalWorkingHourMonthly = reportService.GetTotalWorkingHourMonthly(getWorkingHourMonthlyCriteria);
            var firstRecord = totalWorkingHourMonthly != null && totalWorkingHourMonthly.Any() ? totalWorkingHourMonthly[0] : null;

            if (model.IsExportExcel)
            {
                string fileName = "TotalWorkingHourMonthly_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
                exportService = new TotalWorkingHourMonthlyExportService(totalWorkingHourMonthly);
                HSSFWorkbook hssfworkbook = exportService.Export();

                return ExcelFile(hssfworkbook, fileName);
            }

            ViewBag.EntityList = totalWorkingHourMonthly;
            ViewBag.FirstRecord = firstRecord;
            ViewBag.Groups = GetAllTeam();
            ViewBag.TeamIds = model.TeamIds;

            return View(model);
        }

        public ActionResult TotalWorkingHourDaily(
            DateTime? workDate,
            string teamIds,
            string employee,
            int? employeeId,
            bool isExportExcel = false,
            byte? isLocal = null)
        {
            var model = new TotalWorkingHourDailyViewModel
            {
                TeamIds = teamIds == null ? string.Empty : teamIds,
                Employee = employee,
                WorkDate = workDate.HasValue ? workDate.Value : DateTime.Now,
                EmployeeId = string.IsNullOrEmpty(employee) ? null : employeeId,
                IsExportExcel = isExportExcel,
                IsLocal = isLocal
            };

            if (model.IsExportExcel)
            {
                var criteria = new GetTotalWorkingHourDailyCriteria()
                {
                    EmpId = string.IsNullOrEmpty(employee) ? null : employeeId,
                    GroupIds = model.TeamIds,
                    IsLocal = isLocal,
                    WorkDate = model.WorkDate
                };
                var totalWorkingHourDaily = reportService.GetTotalWorkingHourDaily(criteria);
                string fileName = "TotalWorkingHourDaily_" + model.WorkDate.ToString("yyyyMMdd") + ".xls";

                exportService = new TotalWorkingHourDailyExportService(totalWorkingHourDaily);
                HSSFWorkbook hssfworkbook = exportService.Export();

                return ExcelFile(hssfworkbook, fileName);
            }
            else
            {
                ViewBag.Groups = GetAllTeam();
            }

            return View(model);
        }

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

            ViewBag.Groups = GetAllTeam();
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
                IList<TotalWorkingHourMonthly> totalWorkingHourWeekly = reportService.GetTotalWorkingHourWeekly(getTotalWorkingHourWeeklyCriteria);
                TotalWorkingHourMonthly firstRecord = totalWorkingHourWeekly != null && totalWorkingHourWeekly.Any() ? totalWorkingHourWeekly[0] : null;
                ViewBag.EntityList = totalWorkingHourWeekly;
                ViewBag.FirstRecord = firstRecord;

                if (isExcel)
                {
                    string fileName = "TotalWorkingHourMonthly_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
                    exportService = new TotalWorkingHourMonthlyExportService(totalWorkingHourWeekly);
                    HSSFWorkbook hssfworkbook = exportService.Export();
                    return ExcelFile(hssfworkbook, fileName);
                }
            }

            return View(model);
        }

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
            ViewBag.Groups = GetAllTeam();
            ViewBag.DefaultFromDate = model.FromDate;
            ViewBag.DefaultToDate = model.ToDate;
            var getAllJobTitleCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = Constants.JobTitle
            };
            IList<LookupData> listJobTitle = lookupDataService.GetAll(getAllJobTitleCriteria);
            ViewBag.JobTitles = listJobTitle.Select(j => new SelectListItem
            {
                Text = j.Name,
                Value = j.Id.ToString(),
                Selected = string.IsNullOrEmpty(j.Id.ToString())
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
                List<WorkingCalendarSummary> calendars = reportService.CalendarSummaryList(criteria).ToList();
                string fileName = "TotalWorkingHourSummary_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
                var hssfworkbook = exportExcelService.ExportWorkingCalendarSummary(calendars, fromDate.Value, toDate.Value);
                return ExcelFile(hssfworkbook, fileName);
            }

            return View(model);
        }

        public ActionResult UnpaidLeaveSummary(
            string teamIds,
            string employee,
            int? year = null,
            int? employeeId = null,
            int? category = null)
        {
            var model = new LeaveSummaryViewModel
            {
                Year = year.HasValue ? year.Value : DateTime.Now.Year,
                TeamIds = teamIds,
                Employee = employee,
                EmployeeId = employeeId.HasValue ? employeeId.Value : -1,
                Category = category
            };
            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.CanFilterUnpaidLeaveSummary, Policy.Read)
                || CurrentUser.HasPermission(FunctionKey.CanFilterUnpaidLeaveSummary, Policy.Edit)
                || CurrentUser.HasPermission(FunctionKey.CanFilterUnpaidLeaveSummary, Policy.Create)
                || CurrentUser.HasPermission(FunctionKey.CanFilterUnpaidLeaveSummary, Policy.Delete);

            return UnpaidLeaveSummary(model);
        }

        [HttpPost]
        public ActionResult UnpaidLeaveSummary(LeaveSummaryViewModel model)
        {
            if (model == null)
            {
                model = new LeaveSummaryViewModel
                {
                    Year = DateTime.Now.Year,
                    EmployeeId = -1
                };
            }

            var criteria = new GetUnpaidLeaveCriteria()
            {
                Year = model.Year,
                EmpId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                GroupIds = model.TeamIds,
                IsLocal = (byte?)model.Category,
                SupervisorId = CurrentUser.Id
            };
            IList<LeaveSummary> entityList = reportService.GetUnpaidLeave(criteria);

            var getGroupCriteria = new GetGroupBySupervisorCriteria()
            {
                SupervisorId = CurrentUser.Id
            };
            IList<Team> teams = teamService.GetBySupervisor(getGroupCriteria);
            ViewBag.Groups = teams;
            ViewBag.TeamIds = model.TeamIds;
            ViewBag.EntityList = entityList;

            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = reportService.GetLeaveByTeam(entityList, teams);
            ViewBag.LeaveSummaryByTeamList = leaveSummaryByTeamList;

            return View(model);
        }

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
            var model = new EmployeeSupervisorViewModel()
            {
                Groups = GetAllTeam()
            };

            return View(model);
        }

        public ActionResult EmployeeSupervisorDetails(EmployeeSupervisorViewModel model)
        {
            var criteria = new GetSupervisorReportCriteria()
            {
                EmployeeId = model.EmployeeId,
                GroupIds = model.GroupIds,
                IsLocal = model.IsLocal,
                ActionType = model.ActionType
            };
            IList<SupervisorReportModel> supervisors = reportService.GetSupervisorReport(criteria);

            return PartialView("_EmployeeSupervisorDetails", supervisors);
        }

        public ActionResult TeamSupervisor()
        {
            ViewBag.Groups = GetAllTeam();
            return View();
        }

        public ActionResult TeamSupervisorDetails(string groups, bool? isExcel)
        {
            var criteria = new GetAllTeamSupervisorCriteria()
            {
                GroupIds = groups,
            };
            IList<TeamSupervisorReportModel> supervisors = reportService.GetAllTeamSupervisor(criteria);

            if (isExcel.Value)
            {
                string fileName = "TeamSupervisor_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
                var hssfworkbook = exportExcelService.ExportTeamSupervisor(supervisors);
                return ExcelFile(hssfworkbook, fileName);
            }
            else
            {
                return PartialView(supervisors);
            }
        }

        [HttpGet]
        public ActionResult Manpower(int? year)
        {
            var model = new ManpowerViewModel()
            {
                Year = year.HasValue ? year.Value : DateTime.Now.Year
            };

            var criteria = new GetManpowerCriteria()
            {
                Year = model.Year
            };

            IList<ManpowerModel> manpowers = reportService.GetManpower(criteria);
            model.Manpowers = manpowers;

            return View(model);
        }

        [HttpGet]
        public ActionResult ManpowerCategory(int? year)
        {
            var model = new ManpowerCategoryViewModel()
            {
                Year = year.HasValue ? year.Value : DateTime.Now.Year
            };

            var criteria = new GetManpowerCategoryCriteria()
            {
                Year = model.Year
            };

            IList<ManpowerCategoryModel> manpowers = reportService.GetManpowerCategory(criteria);
            model.Manpowers = manpowers;

            return View(model);
        }

        private ActionResult ExcelFile(HSSFWorkbook hssfworkbook, string fileName)
        {
            MemoryStream output = new MemoryStream();
            hssfworkbook.Write(output);
            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
        }

        private IList<Team> GetAllTeam()
        {
            var criteria = new GetAllGroupCriteria();

            return teamService.GetAll(criteria);
        }
    }
}
