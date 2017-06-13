namespace Fanex.NexHR.Controllers
{
    using Core;
    using Core.Enums;
    using Criteria;
    using Criteria.AnnualLeave;
    using Criteria.Calendar;
    using Criteria.Employee;
    using Criteria.Report;
    using Criteria.Request;
    using Criteria.Supervisor;
    using Criteria.Team;
    using Models;
    using Models.Request;
    using Models.Supervisor;
    using Nexus.RoleManagement.Services;
    using NPOI.HSSF.UserModel;
    using Request;
    using Services;
    using Services.ExportExcel;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using ViewModels;
    using ViewModels.Request;
    using Web.Providers;

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Ok.")]
    public class ExportController : RequestController
    {
        private readonly IReportService reportService;
        private readonly IExportExcelService exportExcelService;
        private readonly IRequestService requestService;
        private readonly ITeamService teamService;
        private readonly IOvertimeRequestService overtimeService;
        private readonly IEmployeeService employeeService;
        private readonly IWorkingCalendarService workingCalendarService;
        private readonly IExportExcelAttendanceReportService exportExcelAttendanceReportService;
        private readonly IExportExcelEmployeeInfoService exportExcelEmployeeInfoService;
        private readonly IExportExcelEmployeeSupervisor exportExcelEmployeeSupervisor;
        private readonly IExportExcelManpowerCategoryService exportExcelManpowerCategoryService;
        private readonly IExportExcelManpowerService exportExcelManpowerService;
        private readonly IExportExcelOvertimeReportService exportExcelOvertimeReportService;
        private readonly IExportExcelSectionSupervisorService exportExcelSectionSupervisorService;
        private readonly IExportExcelUnpaidLeaveSummaryReportService exportExcelUnpaidLeaveSummaryReportService;
        private readonly IExportExcelYearlyOvertimeReportService exportExcelYearlyOvertimeReportService;

        public ExportController()
            : this(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null)
        {
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Ok.")]
        public ExportController(
            IReportService reportService,
            IExportExcelService exportExcelService,
            IRequestService requestService,
            ITeamService teamService,
            IOvertimeRequestService overtimeService,
            IEmployeeService employeeService,
            IWorkingCalendarService workingCalendarService,
            IExportExcelAttendanceReportService exportExcelAttendanceReportService,
            IExportExcelEmployeeInfoService exportExcelEmployeeInfoService,
            IExportExcelEmployeeSupervisor exportExcelEmployeeSupervisor,
            IExportExcelManpowerCategoryService exportExcelManpowerCategoryService,
            IExportExcelManpowerService exportExcelManpowerService,
            IExportExcelOvertimeReportService exportExcelOvertimeReportService,
            IExportExcelSectionSupervisorService exportExcelSectionSupervisorService,
            IExportExcelUnpaidLeaveSummaryReportService exportExcelUnpaidLeaveSummaryReportService,
            IExportExcelYearlyOvertimeReportService exportExcelYearlyOvertimeReportService)
        {
            this.reportService = reportService ?? new ReportService();
            this.exportExcelService = exportExcelService ?? new ExportExcelService();
            this.requestService = requestService ?? new RequestService();
            this.teamService = teamService ?? new TeamService();
            this.overtimeService = overtimeService ?? new OvertimeRequestService();
            this.employeeService = employeeService ?? new EmployeeService();
            this.workingCalendarService = workingCalendarService ?? new WorkingCalendarService();
            this.exportExcelAttendanceReportService = exportExcelAttendanceReportService ?? new ExportExcelAttendanceReportService();
            this.exportExcelEmployeeInfoService = exportExcelEmployeeInfoService ?? new ExportExcelEmployeeInfoService();
            this.exportExcelEmployeeSupervisor = exportExcelEmployeeSupervisor ?? new ExportExcelEmployeeSupervisor();
            this.exportExcelManpowerCategoryService = exportExcelManpowerCategoryService ?? new ExportExcelManpowerCategoryService();
            this.exportExcelManpowerService = exportExcelManpowerService ?? new ExportExcelManpowerService();
            this.exportExcelOvertimeReportService = exportExcelOvertimeReportService ?? new ExportExcelOvertimeReportService();
            this.exportExcelSectionSupervisorService = exportExcelSectionSupervisorService ?? new ExportExcelSectionSupervisorService();
            this.exportExcelUnpaidLeaveSummaryReportService = exportExcelUnpaidLeaveSummaryReportService ?? new ExportExcelUnpaidLeaveSummaryReportService();
            this.exportExcelYearlyOvertimeReportService = exportExcelYearlyOvertimeReportService ?? new ExportExcelYearlyOvertimeReportService();
        }

        public ActionResult Attendance(EmployeeAttendanceViewModel model)
        {
            var criteria = new GetEmployeeAttendanceCriteria()
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
            exportExcelAttendanceReportService.Export(employeeAttendances, model.FromDate, model.ToDate);

            return new EmptyResult();
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
            var criteria = new GetUnpaidLeaveCriteria()
            {
                Year = model.Year,
                EmpId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                GroupIds = model.SectionIds,
                IsLocal = (byte?)model.Category,
                SupervisorId = CurrentUser.Id
            };
            IList<LeaveSummary> leaveSummary = reportService.GetUnpaidLeave(criteria);
            IList<Team> teams = teamService.GetAll(new GetAllGroupCriteria());
            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = reportService.GetLeaveByTeam(leaveSummary, teams);
            exportExcelUnpaidLeaveSummaryReportService.Export(leaveSummaryByTeamList, model.Year);

            return new EmptyResult();
        }

        public ActionResult OvertimeReport(OvertimeDetailReportViewModel model)
        {
            var criteria = new GetOvertimeExcelReportDetailCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                EmployeeId = model.EmployeeId,
                JobTitleId = model.JobTitleIds,
                GroupIds = model.SectionIds
            };
            IList<OvertimeDaily> overtimeDetails = overtimeService.OTExcelReportDetails(criteria).OrderBy(x => x.OvertimeDate).ToList();
            exportExcelOvertimeReportService.Export(overtimeDetails, model.FromDate, model.ToDate);

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
                    EmployeeId = model.EmployeeId,
                    Status = null
                };
                entity = overtimeService.GetYearlyOvertimeReport(getYearlyOvertimeReportCriteria);
            }
            else
            {
                var criteria = new GetYearlyOvertimeReportByNormalCalendarCriteria
                {
                    Year = model.Year,
                    GroupIds = model.SectionIds,
                    EmployeeId = model.EmployeeId,
                    Status = null
                };
                entity = overtimeService.GetYearlyOvertimeReportByNormalCalendar(criteria);
            }

            exportExcelYearlyOvertimeReportService.Export(entity, model.Year, model.Calendar);

            return new EmptyResult();
        }

        public ActionResult EmployeeBasicInfo(UserManagementViewModel model)
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
            exportExcelEmployeeInfoService.ExportEmployeeBasicInfo(employees);

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
            exportExcelEmployeeInfoService.ExportEmployeeDetailsInfo(employees);

            return new EmptyResult();
        }

        public ActionResult EmployeeWorkingCalendar(WorkingCalendarFilterViewModel model)
        {
            string fileName = "EmployeeWorkingCalendar_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            var criteria = new GetWorkingCalendarWeeklyCriteria()
            {
                EmpId = model.EmployeeId,
                Fromtime = model.FromDate,
                Totime = model.ToDate,
                GroupIds = model.SectionIds,
                JobTitleId = model.JobTitleIds,
                IsLocal = model.IsLocal
            };

            IList<WorkingCalendarsViewModel> workingCalendars = workingCalendarService.GetWorkingCalendars(criteria);
            HSSFWorkbook hssfworkbook = exportExcelService.ExportEmployeeWorkingCalendar(workingCalendars, model.FromDate, model.ToDate);

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult EmployeeOvertimeReport(int? year, int? month, int? calendar, int? empId)
        {
            var requestDate = new DateTime(year.Value, month.Value, 1);
            DateTime fromDate, toDate;

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
                EmployeeId = empId,
                Calendar = calendar.Value
            };
            IList<OvertimeExcelReport> overtimes = overtimeService.OvertimeRecord(criteria);
            HSSFWorkbook hssfworkbook = exportExcelService.ExportEmployeeOvertimeRecord(overtimes, calendar, fromDate, toDate);

            return ExcelFile(hssfworkbook, fileName);
        }

        [HRAuthorize(Policy.Host, FunctionKey.VerifyRequest, FunctionKey.ApproveRequest)]
        public ActionResult MemberWorkingTimeRequest(RequestViewModel model)
        {
            model.ToDate = Utilities.AddTimeToDate(model.ToDate, 23, 59, 59);
            model.ViewType = RequestViewType.PersonalSpace;
            var criteria = new GetRequestCriteria()
            {
                EmpId = model.EmployeeId == -1 ? null : new int?(model.EmployeeId),
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupIds = model.TeamIds,
                Status = GetStatus(model.StatusId),
                RequestType = model.RequestType,
                IsQueryByCreatedDate = !model.IsFilterByAffectedDate,
                RequestViewType = model.ViewType,
                SupervisorId = CurrentUser.Id,
                IsLocal = model.IsLocal.HasValue ? (model.IsLocal.Value == 1) : (bool?)null
            };

            IList<RequestEntity> requestList = requestService.GetRequests(criteria);
            HSSFWorkbook hssfworkbook = exportExcelService.ExportWorkingTimeRequest(requestList, model.PageTitle);
            string fileName = "MemberRequest_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            return ExcelFile(hssfworkbook, fileName);
        }

        [HRAuthorize(Policy.Host, FunctionKey.ProcessRequest)]
        public ActionResult HRViewWorkingTimeRequest(RequestViewModel model)
        {
            model.ToDate = Utilities.AddTimeToDate(model.ToDate, 23, 59, 59);
            model.ViewType = RequestViewType.HRSpace;

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
                RequestViewType = model.ViewType,
                IsLocal = model.IsLocal.HasValue ? (model.IsLocal.Value == 1) : (bool?)null
            };

            IList<RequestEntity> requestList = requestService.GetRequests(criteria);
            HSSFWorkbook hssfworkbook = exportExcelService.ExportWorkingTimeRequest(requestList, model.PageTitle);
            string fileName = "EmployeeRequest_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult DailyViewWorkingTimeRequest(RequestViewModel model)
        {
            model.ToDate = Utilities.AddTimeToDate(model.ToDate, 23, 59, 59);
            model.ViewType = RequestViewType.CompanySpace;
            var criteria = new GetRequestCriteria()
            {
                EmpId = model.EmployeeId == -1 ? null : new int?(model.EmployeeId),
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupIds = model.TeamIds,
                Status = GetStatus(model.StatusId),
                RequestType = model.RequestType,
                IsQueryByCreatedDate = !model.IsFilterByAffectedDate,
                RequestViewType = model.ViewType,
                SupervisorId = CurrentUser.Id,
                IsLocal = model.IsLocal.HasValue ? (model.IsLocal.Value == 1) : (bool?)null
            };

            IList<RequestEntity> requestList = requestService.GetRequests(criteria);
            HSSFWorkbook hssfworkbook = exportExcelService.ExportWorkingTimeRequest(requestList, model.PageTitle);
            string fileName = "DailyRequest_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult Manpower(int year)
        {
            var criteria = new GetManpowerCriteria()
            {
                Year = year
            };

            IList<ManpowerModel> manpowers = reportService.GetManpower(criteria);
            exportExcelManpowerService.Export(manpowers, year);

            return new EmptyResult();
        }

        public ActionResult ManpowerCategory(int year)
        {
            var criteria = new GetManpowerCategoryCriteria()
            {
                Year = year
            };

            IList<ManpowerCategoryModel> manpowers = reportService.GetManpowerCategory(criteria);
            exportExcelManpowerCategoryService.Export(manpowers, year);

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
            exportExcelEmployeeSupervisor.Export(supervisors);

            return new EmptyResult();
        }

        public ActionResult ExportSectionSupervisor(EmployeeAndSectionSupervisorViewModel model)
        {
            var criteria = new GetSectionSupervisorCriteria()
            {
                GroupIds = model.SectionIds,
            };
            IList<SectionSupervisorReportModel> supervisors = reportService.GetSectionSupervisor(criteria);
            exportExcelSectionSupervisorService.Export(supervisors);

            return new EmptyResult();
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