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
    using Criteria.Report;
    using Criteria.Supervisor;
    using Criteria.Time;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Services;
    using Models;
    using Models.Report;
    using Models.Supervisor;
    using NPOI.HSSF.UserModel;
    using Services;
    using Services.ExportExcel;
    using ViewModels;
    using ViewModels.Report;

    [ValidateInput(false)]
    public class ReportController : BaseController
    {
        private readonly IReportService reportService;
        private readonly ITeamService teamService;
        private readonly IExportExcelService exportExcelService;
        private readonly ILookupDataService lookupDataService;

        public ReportController(
            ISessionService sessionService,
            IReportService reportSevice,
            ITeamService teamService,
            IExportExcelService exportExcelService,
            ILookupDataService lookupDataService) : base(sessionService)
        {
            this.reportService = reportSevice;
            this.teamService = teamService;
            this.exportExcelService = exportExcelService;
            this.lookupDataService = lookupDataService;
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

        [HttpGet]
        public ActionResult TotalWorkingHours()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;

            var model = new ReportFilterViewModel
            {
                FromDate = new DateTime(year, month, 1),
                ToDate = DateTime.Now,
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

            ViewBag.Groups = teamService.GetAll();
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
            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.ProcessRequest, Policy.Host);

            return View(model);
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

        public ActionResult TotalWorkingHourDaily()
        {
            var model = new TotalWorkingHourDailyViewModel
            {
                WorkDate = DateTime.Now,
                Sections = teamService.GetAll()
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
                Sections = teamService.GetAll()
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

                ViewBag.Groups = teamService.GetAll();
                ViewBag.DefaultFromDate = model.FromDate;
                ViewBag.DefaultToDate = model.ToDate;
                IList<LookupData> listJobTitles = lookupDataService.GetAll(Constants.JobTitle);
                ViewBag.JobTitles = listJobTitles.Select(j => new SelectListItem
                {
                    Text = j.Name,
                    Value = j.Id.ToString(),
                    Selected = string.IsNullOrEmpty(j.Id.ToString())
                }).ToList();

                return View(model);
            }
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
    }
}