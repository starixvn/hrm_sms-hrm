namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Controllers.Request;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Discipline;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Discipline;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.ExportExcel;
    using Fanex.NexHR.Services.Implement;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.Discipline;
    using Fanex.NexHR.ViewModels.Request;
    using NPOI.HSSF.UserModel;

    public class ExportController : RequestController
    {
        private const string EmployeeBasicInfoTemplatePath = @"~\ExcelTemplates\EmployeeBasicInfoTemplate.xls";
        private const string EmployeeDetailsInfoTemplatePath = @"~\ExcelTemplates\EmployeeDetailsInfoTemplate.xls";
        private const string DisciplineSummaryTemplatePath = @"~\ExcelTemplates\DisciplineSummaryReport.xls";
        private const string DisciplineDetailTemplatePath = @"~\ExcelTemplates\DisciplineDetailReport.xls";
        private const string WorkingTimeRequestTemplatePath = @"~\ExcelTemplates\WorkingTimeRequestTemplate.xls";
        private const string EmployeeSupervisorTemplatePath = @"~\ExcelTemplates\EmployeeSupervisorTemplate.xls";
        private const string SectionSupervisorTemplatePath = @"~\ExcelTemplates\SectionSupervisorTemplate.xls";
        private const string ManpowerTemplatePath = @"~\ExcelTemplates\ManpowerReportTemplate.xls";
        private const string ManpowerCategoryTemplatePath = @"~\ExcelTemplates\ManpowerCategoryReportTemplate.xls";
        private const string OvertimeTemplatePath = @"~\ExcelTemplates\OvertimeReportTemplate.xls";
        private const string YearlyOvertimeTemplatePath = @"~\ExcelTemplates\YearlyOvertimeReportTemplate.xls";

        private readonly IReportService reportService;
        private readonly IExportExcelService exportExcelService;
        private readonly IRequestService requestService;
        private readonly ITeamService teamService;
        private readonly IOvertimeRequestService overtimeService;
        private readonly IEmployeeService employeeService;
        private readonly IWorkingCalendarService workingCalendarService;
        private readonly IDisciplineService disciplineService;
        private IExportService exportService;

        public ExportController()
            : this(null, null, null, null, null, null, null, null)
        {
        }

        public ExportController(
            IReportService reportService,
            IExportExcelService exportExcelService,
            IRequestService requestService,
            ITeamService teamService,
            IOvertimeRequestService overtimeService,
            IEmployeeService employeeService,
            IWorkingCalendarService workingCalendarService,
            IDisciplineService disciplineService)
        {
            this.reportService = reportService ?? new ReportService();
            this.exportExcelService = exportExcelService ?? new ExportExcelService();
            this.requestService = requestService ?? new RequestService();
            this.teamService = teamService ?? new TeamService();
            this.overtimeService = overtimeService ?? new OvertimeRequestService();
            this.employeeService = employeeService ?? new EmployeeService();
            this.workingCalendarService = workingCalendarService ?? new WorkingCalendarService();
            this.disciplineService = disciplineService ?? new DisciplineService();
        }

        public ActionResult TotalWorkingHours(TotalWorkingHoursViewModel model)
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            if (model == null)
            {
                model = new TotalWorkingHoursViewModel
                {
                    FromDate = new DateTime(year, month, 1),
                    ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                    DateOption = 5,
                };
            }

            string fileName = "TotalWorkingHours_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            var getTotalWorkingHourCriteria = new GetReportWorkingHourSummaryCriteria()
            {
                Fromdate = model.FromDate,
                Todate = model.ToDate,
                EmpId = (model.EmployeeId != -1 & model.EmployeeId != 0) ? new int?(model.EmployeeId) : null,
                GroupIds = model.TeamIds,
                IsLocal = model.IsLocal
            };

            IList<TotalWorkingHours> totalWorkingHours = reportService.GetWorkingHoursList(getTotalWorkingHourCriteria);

            exportService = new TotalWorkingHourExportService(totalWorkingHours, model.FromDate, model.ToDate);
            HSSFWorkbook hssfworkbook = exportService.Export();
            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult Attendance(EmployeeAttendanceViewModel model)
        {
            string fileName = "AttendanceReport_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            var criteria = new GetEmployeeAttendanceCriteria()
            {
                SupervisorId = model.SupervisorId,
                Fromtime = model.FromDate,
                Totime = model.ToDate,
                AttendanceType = model.AttendanceType,
                EmpId = model.EmpId,
                Groupids = model.GroupIds,
                ExcludeUserDelete = model.ExcludeDeletedUsers ? 1 : 0,
                IsLocal = model.IsLocal
            };
            IList<EmployeeAttendanceModel> employeeAttendance = reportService.GetEmployeeAttendanceReport(criteria);

            HSSFWorkbook hssfworkbook = exportExcelService.ExportAttendance(employeeAttendance, model.FromDate, model.ToDate);

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult LeaveSummary(LeaveSummaryViewModel model)
        {
            string fileName = "LeaveSummary_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            byte? isLocal = null;
            if (model.Category != null)
            {
                isLocal = (byte)model.Category;
            }

            var criteria = new GetAnnualLeaveBalanceCriteria()
            {
                Year = model.Year,
                EmpId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                GroupIds = model.SectionIds,
                IsLocal = isLocal,
                Role = string.Empty,
                SupervisorId = null
            };
            IList<LeaveSummary> leaveSummary = reportService.GetAnnualLeaveBalanceReport(criteria);
            var getAllGroupCriteria = new GetAllGroupCriteria();
            IList<Team> teams = teamService.GetAll(getAllGroupCriteria);
            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = reportService.GetLeaveByTeam(leaveSummary, teams);

            HSSFWorkbook hssfworkbook = exportExcelService.ExportLeaveSummary(leaveSummaryByTeamList, model.Year);

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult UnpaidLeaveSummary(LeaveSummaryViewModel model)
        {
            string fileName = "UnpaidLeaveSummary_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            byte? isLocal = null;
            if (model.Category != null)
            {
                isLocal = (byte)model.Category;
            }

            var criteria = new GetUnpaidLeaveCriteria()
            {
                Year = model.Year,
                EmpId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                GroupIds = model.SectionIds,
                IsLocal = isLocal,
                SupervisorId = null
            };
            IList<LeaveSummary> leaveSummary = reportService.GetUnpaidLeave(criteria);
            var getAllGroupCriteria = new GetAllGroupCriteria();
            IList<Team> teams = teamService.GetAll(getAllGroupCriteria);
            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = reportService.GetLeaveByTeam(leaveSummary, teams);

            HSSFWorkbook hssfworkbook = exportExcelService.ExportUnpaidLeaveSummary(leaveSummaryByTeamList, model.Year);

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult OvertimeReport(OvertimeDetailReportViewModel model)
        {
            var criteria = new GetOvertimeExcelReportDetailCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                EmployeeID = model.EmpId,
                JobTitleId = model.JobTitleIds,
                GroupIds = model.SectionIds
            };
            IList<OvertimeDaily> overtimeDetails = overtimeService.OTExcelReportDetails(criteria).OrderBy(x => x.OvertimeDate).ToList();
            string templatePath = Server.MapPath(OvertimeTemplatePath);
            exportExcelService.ExportOvertimeReport(overtimeDetails, model.FromDate, model.ToDate, templatePath);

            return new EmptyResult();
        }

        public ActionResult YearlyOvertimeReport(YearlyOvertimeReportViewModel model)
        {
            IList<OvertimeAnnualReportViewModel> entity = null;
            if (model.Calendar == 1)
            {
                var getYearlyOvertimeReportCriteria = new GetYearlyOvertimeReportCriteria
                {
                    SupervisorId = this.CurrentUser.Id,
                    Year = model.Year,
                    GroupIds = model.SectionIds,
                    EmployeeId = model.EmpId,
                    Status = null
                };
                entity = this.overtimeService.GetYearlyOvertimeReport(getYearlyOvertimeReportCriteria);
            }
            else
            {
                var criteria = new GetYearlyOvertimeReportByNormalCalendarCriteria
                {
                    Year = model.Year,
                    GroupIds = model.SectionIds,
                    EmployeeId = model.EmpId,
                    Status = null
                };
                entity = this.overtimeService.GetYearlyOvertimeReportByNormalCalendar(criteria);
            }

            string templatePath = Server.MapPath(YearlyOvertimeTemplatePath);
            exportExcelService.ExportYearlyOvertimeReport(entity, model.Year, model.Calendar, templatePath);

            return new EmptyResult();
        }

        public ActionResult EmployeeBasicInfo(UserManagementViewModel model)
        {
            string fileName = "EmployeeBasicInformation_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            var criteria = new GetAllEmployeeCriteria()
            {
                Query = model.Keyword,
                GroupIds = model.GroupIds,
                EnableUser = model.IsEnable,
                IsDeleteUser = model.IsDelete,
                TitleId = model.JobTitleId,
                IsLocalUser = model.IsLocal
            };

            IList<Employee> employees = employeeService.GetEmployees(criteria);
            string templatePath = Server.MapPath(EmployeeBasicInfoTemplatePath);
            exportExcelService.ExportEmployeeBasicInfo(employees, templatePath);

            return new EmptyResult();
        }

        public ActionResult EmployeeDetailsInfo(UserManagementViewModel model)
        {
            var criteria = new GetAllEmployeeCriteria()
            {
                Query = model.Keyword,
                GroupIds = model.GroupIds,
                EnableUser = model.IsEnable,
                IsDeleteUser = model.IsDelete,
                TitleId = model.JobTitleId,
                IsLocalUser = model.IsLocal
            };

            IList<Employee> employees = employeeService.GetEmployees(criteria);
            string templatePath = Server.MapPath(EmployeeDetailsInfoTemplatePath);
            exportExcelService.ExportEmployeeDetailsInfo(employees, templatePath);

            return new EmptyResult();
        }

        public ActionResult EmployeeWorkingCalendar(DateTime fromDate, DateTime toDate, string group, string title)
        {
            string fileName = "EmployeeWorkingCalendar_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            var criteria = new GetWorkingCalendarWeeklyCriteria()
            {
                EmpId = null,
                Fromtime = fromDate,
                Totime = toDate,
                GroupIds = group,
                JobTitleId = title,
                IsLocal = null
            };
            var entity = workingCalendarService.GetWorkingCalendars(criteria);
            HSSFWorkbook hssfworkbook = exportExcelService.ExportEmployeeWorkingCalendar(entity, fromDate, toDate);

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult EmployeeOvertimeReport(int? year, int? month, int? calendar, int? empId)
        {
            var requestDate = new DateTime(year.Value, month.Value, 1);
            var fromDate = new DateTime();
            var toDate = new DateTime();

            if (calendar == 0)
            {
                var dayInMonth = DateTime.DaysInMonth(year.Value, month.Value);
                fromDate = requestDate;
                toDate = new DateTime(year.Value, month.Value, dayInMonth);
            }
            else
            {
                DateTime prevMonthDate = requestDate.Date.AddMonths(-1);
                fromDate = new DateTime(prevMonthDate.Year, prevMonthDate.Month, 10);
                toDate = new DateTime(requestDate.Year, requestDate.Month, 9);
            }

            string fileName = "EmployeeOvertimeRecord_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            var criteria = new GetOvertimeRecordCriteria
            {
                Month = requestDate,
                EmployeeID = empId,
                Calendar = calendar.Value
            };
            IList<OvertimeExcelReport> overtimes = overtimeService.OvertimeRecord(criteria);
            HSSFWorkbook hssfworkbook = exportExcelService.ExportEmployeeOvertimeRecord(overtimes, calendar, fromDate, toDate);

            return ExcelFile(hssfworkbook, fileName);
        }

        /// [Authorize(Roles = "Verifier, Approver")]
        /// [CustomAuthorize,FunctionKey.ExportMemberWorkingTimeRequest Policy.Read)]
        public ActionResult MemberWorkingTimeRequest(RequestViewModel model)
        {
            model.ToDate = Utilities.AddTimeToDate(model.ToDate, 23, 59, 59);

            var criteria = new GetRequestCriteria()
            {
                EmpId = model.EmployeeId == -1 ? null : new int?(model.EmployeeId),
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupIds = model.TeamIds,
                Status = GetStatus(model.StatusId),
                RequestType = model.RequestType,
                IsQueryByCreatedDate = !model.IsFilterByAffectedDate,
                SupervisorId = CurrentUser.Id,
                IsLocal = model.IsLocal.HasValue ? (model.IsLocal.Value == 1) : (bool?)null
            };

            IList<RequestEntity> requestList = requestService.GetRequests(criteria);
            HSSFWorkbook hssfworkbook = exportExcelService.ExportWorkingTimeRequest(requestList);
            string fileName = "MemberWorkingTimeRequest_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            return ExcelFile(hssfworkbook, fileName);
        }

        /// [Authorize(Roles = "HR")]
        /// [CustomAuthorize,FunctionKey.ExportHRWorkingTimeRequest Policy.Read)]
        public ActionResult HRViewWorkingTimeRequest(RequestViewModel model)
        {
            model.ToDate = Utilities.AddTimeToDate(model.ToDate, 23, 59, 59);

            var criteria = new GetRequestCriteria()
            {
                SupervisorId = CurrentUser.Id,
                EmpId = model.EmployeeId == -1 ? null : new int?(model.EmployeeId),
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupIds = model.TeamIds,
                Status = GetStatus(model.StatusId),
                RequestType = model.RequestType,
                IsQueryByCreatedDate = !model.IsFilterByAffectedDate,
                RequestViewType = RequestViewType.HRSpace,
                IsLocal = model.IsLocal.HasValue ? (model.IsLocal.Value == 1) : (bool?)null
            };

            IList<RequestEntity> requestList = requestService.GetRequests(criteria);
            HSSFWorkbook hssfworkbook = exportExcelService.ExportWorkingTimeRequest(requestList);
            string fileName = "WorkingTimeRequest_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult DailyViewWorkingTimeRequest(RequestViewModel model)
        {
            model.ToDate = Utilities.AddTimeToDate(model.ToDate, 23, 59, 59);

            var criteria = new GetRequestCriteria()
            {
                EmpId = model.EmployeeId == -1 ? null : new int?(model.EmployeeId),
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupIds = model.TeamIds,
                Status = GetStatus(model.StatusId),
                RequestType = model.RequestType,
                IsQueryByCreatedDate = !model.IsFilterByAffectedDate,
                RequestViewType = RequestViewType.CompanySpace,
                SupervisorId = CurrentUser.Id,
                IsLocal = model.IsLocal.HasValue ? (model.IsLocal.Value == 1) : (bool?)null
            };

            IList<RequestEntity> requestList = requestService.GetRequests(criteria);
            HSSFWorkbook hssfworkbook = exportExcelService.ExportWorkingTimeRequest(requestList);
            string fileName = "DailyWorkingTimeRequest_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult Manpower(int year)
        {
            var criteria = new GetManpowerCriteria()
            {
                Year = year
            };

            IList<ManpowerModel> manpowers = reportService.GetManpower(criteria);
            string templatePath = Server.MapPath(ManpowerTemplatePath);
            exportExcelService.ExportManpower(manpowers, year, templatePath);

            return new EmptyResult();
        }

        public ActionResult ManpowerCategory(int year)
        {
            var criteria = new GetManpowerCategoryCriteria()
            {
                Year = year
            };

            IList<ManpowerCategoryModel> manpowers = reportService.GetManpowerCategory(criteria);
            string templatePath = Server.MapPath(ManpowerCategoryTemplatePath);
            exportExcelService.ExportManpowerCategory(manpowers, year, templatePath);

            return new EmptyResult();
        }

        public ActionResult ExportDisciplineSummaryReport(DisciplineSummaryViewModel model)
        {
            var criteria = new GetDisciplineSummaryCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupId = model.TeamId,
                IsLocal = model.IsLocal,
                EmployeeId = model.EmployeeId,
                ReasonId = model.ReasonId,
                ApproverId = CurrentUser.Id,
                EmployeeName = model.Employee,
                GroupName = model.TeamName
            };

            IList<DisciplineSummaryModel> disciplines = disciplineService.GetDisciplineSummary(criteria);

            string templatePath = Server.MapPath(DisciplineSummaryTemplatePath);
            disciplineService.ExportDisciplineSummaryReport(criteria, disciplines, templatePath);

            return new EmptyResult();
        }

        public ActionResult ExportDisciplineDetailReport(DisciplineSummaryViewModel model)
        {
            var criteria = new GetDisciplineDetailCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupId = model.TeamId,
                IsLocal = model.IsLocal,
                EmployeeId = model.EmployeeId,
                ReasonId = model.ReasonId,
                ApproverId = CurrentUser.Id,
                EmployeeName = model.Employee,
                GroupName = model.TeamName
            };

            IList<DisciplineDetailModel> disciplines = disciplineService.GetDisciplineDetail(criteria);

            string templatePath = Server.MapPath(DisciplineDetailTemplatePath);
            disciplineService.ExportDisciplineDetailReport(criteria, disciplines, templatePath);

            return new EmptyResult();
        }

        public ActionResult ExportEmployeeSupervisor(EmployeeAndSectionSupervisorViewModel model)
        {
            var criteria = new GetSupervisorReportCriteria()
            {
                EmployeeId = model.EmployeeId,
                GroupIds = model.SectionIds,
                IsLocal = model.IsLocal,
                ActionType = model.ActionType
            };
            IList<SupervisorReportModel> supervisors = reportService.GetSupervisorReport(criteria);

            string templatePath = Server.MapPath(EmployeeSupervisorTemplatePath);
            exportExcelService.ExportEmployeeSupervisor(supervisors, templatePath);

            return new EmptyResult();
        }

        public ActionResult ExportSectionSupervisor(EmployeeAndSectionSupervisorViewModel model)
        {
            var criteria = new GetSectionSupervisorCriteria()
            {
                GroupIds = model.SectionIds,
            };
            IList<SectionSupervisorReportModel> supervisors = reportService.GetSectionSupervisor(criteria);

            string templatePath = Server.MapPath(SectionSupervisorTemplatePath);
            exportExcelService.ExportSectionSupervisor(supervisors, templatePath);

            return new EmptyResult();
        }

        public ActionResult ExportTotalWorkingHourWeekly(TotalWorkingHourWeeklyViewModel model)
        {
            string fileName = "TotalWorkingHourWeekly_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            var criteria = new GetTotalWorkingHourWeeklyCriteria()
            {
                EmpId = model.EmployeeId,
                FromDate = DateTime.Parse(model.FromDate),
                ToDate = DateTime.Parse(model.ToDate),
                GroupIds = model.SectionIds,
                IsLocal = model.IsLocal
            };
            IList<TotalWorkingHourMonthly> totalWorkingHours = reportService.GetTotalWorkingHourWeekly(criteria);
            exportService = new TotalWorkingHourMonthlyExportService(totalWorkingHours);
            HSSFWorkbook hssfworkbook = exportService.Export();
            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult ExportTotalWorkingHourMonthly(TotalWorkingHourMonthlyViewModel model)
        {
            string fileName = "TotalWorkingHourMonthly_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            var criteria = new GetTotalWorkingHourMonthlyCriteria
            {
                EmpId = model.EmployeeId,
                GroupIds = model.SectionIds,
                IsLocal = model.IsLocal,
                ReportMonth = new DateTime(model.Year, model.Month, 1)
            };

            IList<TotalWorkingHourMonthly> totalWorkingHours = reportService.GetTotalWorkingHourMonthly(criteria);
            exportService = new TotalWorkingHourMonthlyExportService(totalWorkingHours);
            HSSFWorkbook hssfworkbook = exportService.Export();
            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult ExportTotalWorkingHourDaily(TotalWorkingHourDailyViewModel model)
        {
            string fileName = "TotalWorkingHourDaily_" + model.WorkDate.ToString("yyyyMMdd") + ".xls";
            var criteria = new GetTotalWorkingHourDailyCriteria
            {
                EmpId = model.EmployeeId,
                GroupIds = model.SectionIds,
                IsLocal = model.IsLocal,
                WorkDate = model.WorkDate
            };

            IList<TotalWorkingHourMonthly> totalWorkingHours = reportService.GetTotalWorkingHourDaily(criteria);
            exportService = new TotalWorkingHourDailyExportService(totalWorkingHours);
            HSSFWorkbook hssfworkbook = exportService.Export();
            return ExcelFile(hssfworkbook, fileName);
        }

        private ActionResult ExcelFile(HSSFWorkbook hssfworkbook, string fileName)
        {
            MemoryStream output = new MemoryStream();
            hssfworkbook.Write(output);

            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
        }
    }
}