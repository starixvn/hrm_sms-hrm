namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using Core;
    using Core.Enums;
    using Criteria;
    using Criteria.Lookup;
    using Criteria.Report;
    using Criteria.Request;
    using Criteria.Supervisor;
    using Criteria.Team;
    using Criteria.Time;
    using Models;
    using Models.Report;
    using Models.Supervisor;
    using Nexus.RoleManagement.Services;
    using NPOI.HSSF.UserModel;
    using Services;
    using Services.ExportExcel;
    using Services.Interfaces;
    using ViewModels;
    using ViewModels.Report;
    using ViewModels.Request;
    using Web.Providers;
    using Resources;

    [ValidateInput(false)]
    public class ReportController : BaseController
    {
        private readonly IReportService reportService;
        private readonly ITeamService teamService;
        private readonly IExportExcelService exportExcelService;
        private readonly ILookupDataService lookupDataService;

        public ReportController()
            : this(null, null, null, null)
        {
        }

        public ReportController(
            IReportService reportSevice,
            ITeamService teamService,
            IExportExcelService exportExcelService,
            ILookupDataService lookupDataService)
        {
            this.reportService = reportSevice ?? new ReportService();
            this.teamService = teamService ?? new TeamService();
            this.exportExcelService = exportExcelService ?? new ExportExcelService();
            this.lookupDataService = lookupDataService ?? new LookupDataService();
        }

        [HRAuthorize(Policy.Read, FunctionKey.ReportHome)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Attendance()
        {
            var getGroupCriteria = new GetGroupBySupervisorCriteria
            {
                SupervisorId = CurrentUser.Id
            };

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
                EmpId = CurrentUser.Id
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
                AttendanceType = model.TimeSheetType,
                EmpId = model.EmpId,
                ExcludeUserDelete = 1
            };

            IList<EmployeeAttendanceModel> timesheets = reportService.GetEmployeeAttendanceReport(criteria);

            return PartialView("_TimesheetDetails", timesheets);
        }

        public ActionResult TimeRecord(DateTime? fromDate, DateTime? toDate)
        {
            IList<Team> teams = GetAllTeam();

            var model = new TimeRecordViewModel
            {
                FromDate = (!fromDate.HasValue || !toDate.HasValue) ? DateTime.Now.Date : fromDate.Value,
                ToDate = (!fromDate.HasValue || !toDate.HasValue) ? DateTime.Now.Date : toDate.Value,
                Teams = teams,
                CurrentPage = Constants.CurrentPage,
                ItemsPerPage = Constants.ItemsPerPage
            };
            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.TimeRecord, Policy.Host);

            return View(model);
        }

        public ActionResult TimeRecordDetail(TimeRecordViewModel model)
        {
            bool hasPermissionToEdit = CurrentUser.HasPermission(FunctionKey.TimeRecord, Policy.Host);
            int? employeeId = null;
            if (!hasPermissionToEdit)
            {
                employeeId = this.CurrentUser.Id;
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

        [HRAuthorize(Policy.Read, FunctionKey.RequestStatisticEmployee)]
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

        [HRAuthorize(Policy.Read, FunctionKey.RequestStatisticMember, FunctionKey.RequestStatisticEmployee)]
        public ActionResult HRRequestMonitoringDetails(RequestMonitoringViewModel requestFilter)
        {
            var getRequestMonitoringCriteria = new GetRequestMonitoringCriteria
            {
                FromDate = requestFilter.FromDate,
                ToDate = requestFilter.ToDate,
                MaxRequest = requestFilter.MaxRequest,
                Role = requestFilter.ViewRole,
                EmployeeId = requestFilter.EmpId,
                LoginUserId = CurrentUser.Id,
                GroupIds = requestFilter.Group,
                IsLocal = requestFilter.IsLocal
            };
            List<RequestsMonitoring> requests = reportService.GetRequestMonitoring(getRequestMonitoringCriteria).ToList();
            ViewBag.MaxRequest = requestFilter.MaxRequest;
            ViewBag.FromDate = requestFilter.FromDate;
            ViewBag.ToDate = requestFilter.ToDate;

            ViewBag.isLeader = requestFilter.ViewRole != ((int)RoleSystem.HR).ToString();

            if (requestFilter.IsExcel)
            {
                string fileName = "HrRequestStatistic_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
                var hssfworkbook = exportExcelService.ExportHRRequestStatistic(requests, requestFilter.FromDate, requestFilter.ToDate);
                return ExcelFile(hssfworkbook, fileName);
            }

            return PartialView("RequestMonitoringDetails", requests);
        }

        [HRAuthorize(Policy.Read, FunctionKey.RequestStatisticMember)]
        public ActionResult LeaderRequestMonitoring()
        {
            string viewRole = string.Join(",", CurrentUser.Roles.Select(i => i.RoleId));
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

        [HttpGet]
        public ActionResult TotalWorkingHours()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;

            var model = new ReportFilterViewModel
            {
                FromDate = new DateTime(year, month, 1),
                ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                EmployeeId = -1,
                DateOption = 5
            };

            return TotalWorkingHours(model);
        }

        [HttpPost]
        public ActionResult TotalWorkingHours(ReportFilterViewModel workingHoursModel)
        {
            ReportFilterViewModel model = workingHoursModel;

            if (model == null)
            {
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                model = new ReportFilterViewModel
                {
                    FromDate = new DateTime(year, month, 1),
                    ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                    DateOption = 5,
                    IsExportExcel = false
                };
            }

            ViewBag.Groups = GetAllTeam();
            ViewBag.TeamIds = model.TeamIds;

            var getTotalWorkingHourCriteria = new GetReportWorkingHourSummaryCriteria
            {
                Fromdate = model.FromDate,
                Todate = model.ToDate,
                EmpId = (model.EmployeeId != -1 && model.EmployeeId != 0) ? new int?(model.EmployeeId) : null,
                GroupIds = model.TeamIds,
                IsLocal = model.IsLocal
            };
            var totalWorkingHours = reportService.GetWorkingHoursList(getTotalWorkingHourCriteria);

            ViewBag.EntityList = totalWorkingHours;
            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.ProcessRequest, Policy.Host);

            return View(model);
        }

        public ActionResult WeekendLeaveSummary(WeekendLeaveViewModel model)
        {
            if (model != null)
            {
                var today = DateTime.Today;
                int currentMonth = today.Month;
                model.FromDate = new DateTime(today.Year, currentMonth, 1);
                model.ToDate = today;
                model.Teams = GetAllTeam();
                model.DateRange = Utilities.GetDateRangeType();
                model.MinTotalLeave = 1;
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
                MinTotalLeave = model.MinTotalLeave
            };

            IList<WeekendLeaveSummary> weekendLeaves = reportService.GetWeekendLeaveSummary(criteria);

            return PartialView("_ListWeekendLeaveSummary", weekendLeaves);
        }

        public ActionResult WeekendLeaveDetail(WeekendLeaveViewModel model)
        {
            if (model != null)
            {
                var today = DateTime.Today;
                int currentMonth = today.Month;

                model.FromDate = new DateTime(today.Year, currentMonth, 1);
                model.ToDate = today;
                model.Teams = GetAllTeam();
                model.DateRange = Utilities.GetDateRangeType();
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
                LeaveTypeId = model.LeaveTypeId
            };

            IList<WeekendLeaveDetail> weekendLeaveDetails = reportService.GetWeekendLeaveDetail(criteria);

            return PartialView("_ListWeekendLeaveDetail", weekendLeaveDetails);
        }

        [HttpGet]
        public ActionResult TotalWorkingHourMonthly()
        {
            var today = DateTime.Today;
            var model = new TotalWorkingHourMonthlyViewModel
            {
                Month = today.Month,
                Year = today.Year,
                Sections = GetAllTeam()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult TotalWorkingHourMonthlyDetails(TotalWorkingHourMonthlyViewModel model)
        {
            var criteria = new GetTotalWorkingHourMonthlyCriteria
            {
                EmpId = model.EmployeeId,
                GroupIds = model.SectionIds,
                IsLocal = model.IsLocal,
                ReportMonth = new DateTime(model.Year, model.Month, 1)
            };

            IList<TotalWorkingHourMonthly> totalWorkingHours = reportService.GetTotalWorkingHourMonthly(criteria);
            TotalWorkingHourMonthly firstRecord = null;
            if (totalWorkingHours != null && totalWorkingHours.Any())
            {
                firstRecord = totalWorkingHours.First();
                totalWorkingHours.RemoveAt(0);
            }

            ViewBag.FirstRecord = firstRecord;

            return PartialView("_TotalWorkingHourMonthlyDetails", totalWorkingHours);
        }

        public ActionResult TotalWorkingHourDaily()
        {
            var model = new TotalWorkingHourDailyViewModel
            {
                WorkDate = DateTime.Now,
                Sections = GetAllTeam()
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult TotalWorkingHourWeekly()
        {
            var dayDiff = DayOfWeek.Monday - DateTime.Now.DayOfWeek;
            var model = new TotalWorkingHourWeeklyViewModel
            {
                FromDate = DateTime.Now.AddDays(dayDiff).ToDateMonthYearFormatedDate(),
                ToDate = DateTime.Now.AddDays(dayDiff).AddDays(6).ToDateMonthYearFormatedDate(),
                Sections = GetAllTeam()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult TotalWorkingHourWeeklyDetails(TotalWorkingHourWeeklyViewModel model)
        {
            var criteria = new GetTotalWorkingHourWeeklyCriteria
            {
                EmpId = model.EmployeeId,
                FromDate = DateTime.Parse(model.FromDate),
                ToDate = DateTime.Parse(model.ToDate),
                GroupIds = model.SectionIds,
                IsLocal = model.IsLocal
            };
            IList<TotalWorkingHourMonthly> totalWorkingHours = reportService.GetTotalWorkingHourWeekly(criteria);
            TotalWorkingHourMonthly firstRecord = null;
            if (totalWorkingHours != null && totalWorkingHours.Any())
            {
                firstRecord = totalWorkingHours.First();
                totalWorkingHours.RemoveAt(0);
            }

            ViewBag.FirstRecord = firstRecord;

            return PartialView("_TotalWorkingHourWeeklyDetails", totalWorkingHours);
        }

        public ActionResult WorkingCalendarSummary(ReportFilterViewModel filterModel)
        {
            if (filterModel.IsExportExcel)
            {
                var criteria = new GetEmployeeWorkingCalendarByDateCriteria
                {
                    EmployeeId = filterModel.EmployeeId,
                    FromDate = filterModel.FromDate,
                    ToDate = filterModel.ToDate,
                    GroupIds = filterModel.TeamIds,
                    IsLocal = filterModel.IsLocal,
                    JobTitleId = filterModel.JobTitleIds
                };
                List<WorkingCalendarSummary> calendars = reportService.CalendarSummaryList(criteria).ToList();
                string fileName = "TotalWorkingHourSummary_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
                var hssfworkbook = exportExcelService.ExportWorkingCalendarSummary(calendars, filterModel.FromDate, filterModel.ToDate);
                return ExcelFile(hssfworkbook, fileName);
            }
            else
            {
                var model = new WorkingCalendarSummaryViewModel
                {
                    Employee = filterModel.Employee,
                    EmployeeId = filterModel.EmployeeId,
                    FromDate = DateTime.Now.Date.ToString(Constants.DateFormat),
                    ToDate = DateTime.Now.Date.ToString(Constants.DateFormat),
                    IsLocal = filterModel.IsLocal
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

                return View(model);
            }
        }

        public ActionResult UnpaidLeaveSummary(
            string sectionIds,
            string employee,
            int? year = null,
            int? employeeId = null,
            int? category = null)
        {
            var model = new LeaveSummaryViewModel
            {
                Year = year.HasValue ? year.Value : DateTime.Now.Year,
                SectionIds = sectionIds,
                Employee = employee,
                EmployeeId = employeeId.HasValue ? employeeId.Value : -1,
                Category = category
            };
            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.CanFilterUnpaidLeaveSummary, Policy.Read)
                || CurrentUser.HasPermission(FunctionKey.CanFilterUnpaidLeaveSummary, Policy.Host);

            return UnpaidLeaveSummary(model);
        }

        [HttpPost]
        public ActionResult UnpaidLeaveSummary(LeaveSummaryViewModel leavingSummaryModel)
        {
            var model = leavingSummaryModel;
            if (model == null)
            {
                model = new LeaveSummaryViewModel
                {
                    Year = DateTime.Now.Year,
                    EmployeeId = -1
                };
            }

            var criteria = new GetUnpaidLeaveCriteria
            {
                Year = model.Year,
                EmpId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                GroupIds = model.SectionIds,
                IsLocal = (byte?)model.Category,
                SupervisorId = CurrentUser.Id
            };
            IList<LeaveSummary> entityList = reportService.GetUnpaidLeave(criteria);

            var getGroupCriteria = new GetGroupBySupervisorCriteria
            {
                SupervisorId = CurrentUser.Id
            };
            IList<Team> sections = teamService.GetBySupervisor(getGroupCriteria);
            model.Sections = sections;
            ViewBag.EntityList = entityList;

            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = reportService.GetLeaveByTeam(entityList, sections);
            ViewBag.LeaveSummaryByTeamList = leaveSummaryByTeamList;

            return View(model);
        }

        public ActionResult OvertimeReport(int? month, int? year)
        {
            ViewBag.Month = (month.HasValue) ? month.Value : DateTime.Now.Month;
            ViewBag.Year = (year.HasValue) ? year.Value : DateTime.Now.Year;

            return View();
        }

        public ActionResult EmployeeSupervisor()
        {
            var model = new EmployeeAndSectionSupervisorViewModel
            {
                Sections = GetAllTeam()
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
                Sections = GetAllTeam()
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

        [HttpGet]
        public ActionResult Manpower(int? year)
        {
            var model = new ManpowerViewModel
            {
                Year = year.HasValue ? year.Value : DateTime.Now.Year
            };

            var criteria = new GetManpowerCriteria
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
            var model = new ManpowerCategoryViewModel
            {
                Year = year.HasValue ? year.Value : DateTime.Now.Year
            };

            var criteria = new GetManpowerCategoryCriteria
            {
                Year = model.Year
            };

            IList<ManpowerCategoryModel> manpowers = reportService.GetManpowerCategory(criteria);
            model.Manpowers = manpowers;

            return View(model);
        }

        private ActionResult ExcelFile(HSSFWorkbook hssfworkbook, string fileName)
        {
            using (var output = new MemoryStream())
            {
                hssfworkbook.Write(output);
                return File(output.ToArray(), "application/vnd.ms-excel", fileName);
            }
        }

        private IList<Team> GetAllTeam()
        {
            var criteria = new GetAllGroupCriteria();

            return teamService.GetAll(criteria);
        }
    }
}