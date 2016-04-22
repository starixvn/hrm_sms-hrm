// --------------------------------------------------------------------------
// <copyright file="ExportController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

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
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Discipline;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Overtime;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Discipline;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Implement;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.AnnualLeave;
    using Fanex.NexHR.ViewModels.Discipline;
    using Fanex.NexHR.ViewModels.Overtime;
    using Fanex.NexHR.ViewModels.Report;
    using Fanex.NexHR.ViewModels.Request;
    using NPOI.HSSF.UserModel;

    public class ExportController : RequestController
    {
        private const string DisciplineSummaryTemplatePath = @"~\ExcelTemplates\DisciplineSummaryReport.xls";
        private const string DisciplineDetailTemplatePath = @"~\ExcelTemplates\DisciplineDetailReport.xls";

        private readonly IReportService _reportService;
        private readonly IExportExcelService _exportService;
        private readonly IRequestService _requestService;
        private readonly ITeamService _teamService;
        private readonly IOvertimeRequestService _overtimeService;
        private readonly IEmployeeService _employeeService;
        private readonly IWorkingCalendarService _workingCalendarService;
        private readonly IDisciplineService _disciplineService;

        public ExportController(
            IReportService reportService = null,
            IExportExcelService exportService = null,
            IRequestService requestService = null,
            ITeamService teamService = null,
            IOvertimeRequestService overtimeService = null,
            IEmployeeService employeeService = null,
            IWorkingCalendarService workingCalendarService = null,
            IDisciplineService disciplineService = null)
        {
            _reportService = reportService ?? new ReportService();
            _exportService = exportService ?? new ExportExcelService();
            _requestService = requestService ?? new RequestService();
            _teamService = teamService ?? new TeamService();
            _overtimeService = overtimeService ?? new OvertimeRequestService();
            _employeeService = employeeService ?? new EmployeeService();
            _workingCalendarService = workingCalendarService ?? new WorkingCalendarService();
            _disciplineService = disciplineService ?? new DisciplineService();
        }

        public ExportController()
            : this(null, null, null, null, null, null, null)
        {
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

            IList<TotalWorkingHours> listTotalWorkingHours = _reportService.GetWorkingHoursList(getTotalWorkingHourCriteria);

            var hssfworkbook = _exportService.ExportTotalWorkingHours(listTotalWorkingHours, model.FromDate, model.ToDate);

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
            var employeeAttendance = _reportService.GetEmployeeAttendanceReport(criteria);

            var hssfworkbook = _exportService.ExportAttendance(employeeAttendance, model.FromDate, model.ToDate);

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
                GroupIds = model.TeamIds,
                IsLocal = isLocal,
                Role = string.Empty,
                SupervisorID = null
            };
            IList<LeaveSummary> leaveSummary = _reportService.GetAnnualLeaveBalanceReport(criteria);
            var getAllGroupCriteria = new GetAllGroupCriteria();
            IList<Team> teams = _teamService.GetAll(getAllGroupCriteria);
            var leaveSummaryByTeamList = _reportService.GetLeaveByTeam(leaveSummary, teams);

            var hssfworkbook = _exportService.ExportLeaveSummary(leaveSummaryByTeamList, model.Year);

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult OvertimeReport(DateTime fromDate, DateTime toDate, int? empId, string jobTitle, string groups)
        {
            string fileName = "OvertimeReport_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            var criteria = new GetOvertimeExcelReportDetailCriteria
            {
                FromDate = fromDate,
                ToDate = toDate,
                EmployeeID = empId,
                JobTitleId = jobTitle,
                GroupIds = groups
            };
            IList<OvertimeRequest> entity = _overtimeService.OTExcelReportDetails(criteria).OrderBy(x => x.OvertimeDate).ToList();
            var hssfworkbook = _exportService.ExportOvertimeReport(entity, fromDate, toDate);

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult YearlyOvertimeReport(int year, string team, int? empId, int? calendar)
        {
            string fileName = "YearlyOvertimeReport_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            IList<OvertimeAnnualReportViewModel> entity = null;
            if (calendar == 1)
            {
                var getYearlyOvertimeReportCriteria = new GetYearlyOvertimeReportCriteria
                {
                    SupervisorID = this.CurrentUser.Id,
                    Year = year,
                    GroupIds = team,
                    EmployeeId = empId,
                    Status = null,
                    Role = this.CurrentUser.Roles
                };
                entity = this._overtimeService.GetYearlyOvertimeReport(getYearlyOvertimeReportCriteria);
            }
            else
            {
                var criteria = new GetYearlyOvertimeReportByNormalCalendarCriteria
                {
                    SupervisorID = this.CurrentUser.Id,
                    Year = year,
                    GroupIds = team,
                    EmployeeId = empId,
                    Status = null,
                    Role = this.CurrentUser.Roles
                };
                entity = this._overtimeService.GetYearlyOvertimeReportByNormalCalendar(criteria);
            }

            var hssfworkbook = _exportService.ExportYearlyOvertimeReport(entity, year, calendar);

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult EmployeeBasicInfo(string query, string groupIds, bool? enable, bool? isDelete, int? titleId, bool? isLocal = null)
        {
            string fileName = "EmployeeBasicInformation_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            var getEmployeeCriteria = new GetAllEmployeeCriteria()
            {
                Query = query,
                GroupIds = groupIds,
                EnableUser = enable,
                IsDeleteUser = isDelete,
                TitleId = titleId,
                IsLocalUser = isLocal
            };

            var entity = _employeeService.GetEmployees(getEmployeeCriteria);
            var hssfworkbook = _exportService.ExportEmployeeBasicInfo(entity);

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult EmployeeDetailsInfo(string query, string groupIds, bool? enable, bool? isDelete, int? titleId, bool? isLocal = null)
        {
            string fileName = "EmployeeDetailsInformation_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            var getEmployeeCriteria = new GetAllEmployeeCriteria()
            {
                Query = query,
                GroupIds = groupIds,
                EnableUser = enable,
                IsDeleteUser = isDelete,
                TitleId = titleId,
                IsLocalUser = isLocal
            };
            var entity = _employeeService.GetEmployees(getEmployeeCriteria);
            var hssfworkbook = _exportService.ExportEmployeeDetailsInfo(entity);

            return ExcelFile(hssfworkbook, fileName);
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
            var entity = _workingCalendarService.GetWorkingCalendars(criteria);
            var hssfworkbook = _exportService.ExportEmployeeWorkingCalendar(entity, fromDate, toDate);

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult EmployeeOvertimeRecord(int? year, int? month, int? calendar, int? empId)
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
            var getOvertimeRecordCriteria = new GetOvertimeRecordCriteria
            {
                Month = requestDate,
                EmployeeID = empId,
                Calendar = calendar.Value
            };
            List<OvertimeExcelReport> entity = _overtimeService.OvertimeRecord(getOvertimeRecordCriteria).ToList();

            var hssfworkbook = _exportService.ExportEmployeeOvertimeRecord(entity, calendar, fromDate, toDate);

            return ExcelFile(hssfworkbook, fileName);
        }

        [Authorize(Roles = "Verifier, Approver")]
        public ActionResult MemberWorkingTimeRequest(RequestViewModel model)
        {
            string fileName = "WorkingTimeRequest_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            model.ToDate = Utilities.AddTimeToDate(model.ToDate, 23, 59, 59);

            bool? isLocal = null;
            var criteria = new GetRequestCriteria()
            {
                EmpId = model.EmployeeId == -1 ? null : new int?(model.EmployeeId),
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupIds = model.TeamIds,
                Status = GetStatus(model.StatusId),
                RequestType = model.RequestType,
                IsQueryByCreatedDate = !model.IsFilterByAffectedDate,
                Role = CheckRoleAccordingToRequestType(RequestViewType.PersonalSpace),
                SupervisorId = this.CurrentUser.Id,
                IsLocal = model.IsLocal.HasValue ? (model.IsLocal.Value == 1) : isLocal
            };

            IList<RequestEntity> requestList = _requestService.GetRequests(criteria);

            var hssfworkbook = _exportService.ExportWorkingTimeRequest(requestList);
            return ExcelFile(hssfworkbook, fileName);
        }

        [Authorize(Roles = "HR")]
        public ActionResult HRViewWorkingTimeRequest(RequestViewModel model)
        {
            string fileName = "WorkingTimeRequest_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            model.ToDate = Utilities.AddTimeToDate(model.ToDate, 23, 59, 59);

            bool? isLocal = null;
            var criteria = new GetRequestCriteria()
            {
                SupervisorId = this.CurrentUser.Id,
                EmpId = model.EmployeeId == -1 ? null : new int?(model.EmployeeId),
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupIds = model.TeamIds,
                Status = GetStatus(model.StatusId),
                RequestType = model.RequestType,
                IsQueryByCreatedDate = !model.IsFilterByAffectedDate,
                Role = CheckRoleAccordingToRequestType(RequestViewType.HRSpace),
                IsLocal = model.IsLocal.HasValue ? (model.IsLocal.Value == 1) : isLocal
            };

            IList<RequestEntity> requestList = _requestService.GetRequests(criteria);
            var hssfworkbook = _exportService.ExportWorkingTimeRequest(requestList);
            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult DailyViewWorkingTimeRequest(RequestViewModel model)
        {
            string fileName = "WorkingTimeRequest_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            model.ToDate = Utilities.AddTimeToDate(model.ToDate, 23, 59, 59);

            bool? isLocal = null;
            var criteria = new GetRequestCriteria()
            {
                EmpId = model.EmployeeId == -1 ? null : new int?(model.EmployeeId),
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupIds = model.TeamIds,
                Status = GetStatus(model.StatusId),
                RequestType = model.RequestType,
                IsQueryByCreatedDate = !model.IsFilterByAffectedDate,
                Role = CheckRoleAccordingToRequestType(RequestViewType.CompanySpace),
                SupervisorId = this.CurrentUser.Id,
                IsLocal = model.IsLocal.HasValue ? (model.IsLocal.Value == 1) : isLocal
            };

            IList<RequestEntity> requestList = _requestService.GetRequests(criteria);
            var hssfworkbook = _exportService.ExportWorkingTimeRequest(requestList);
            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult Manpower(int year)
        {
            string fileName = "ManpowerReport_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            var criteria = new GetManpowerCriteria()
            {
                Year = year
            };

            IList<ManpowerModel> manpowers = _reportService.GetManpower(criteria);

            var hssfworkbook = _exportService.ExportManpower(manpowers, year);

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult ManpowerCategory(int year)
        {
            string fileName = "ManpowerCategoryReport_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            var criteria = new GetManpowerCategoryCriteria()
            {
                Year = year
            };

            IList<ManpowerCategoryModel> manpowers = _reportService.GetManpowerCategory(criteria);

            var hssfworkbook = _exportService.ExportManpowerCategory(manpowers, year);

            return ExcelFile(hssfworkbook, fileName);
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

            IList<DisciplineSummaryModel> disciplines = _disciplineService.GetDisciplineSummary(criteria);

            string templatePath = Server.MapPath(DisciplineSummaryTemplatePath);
            _disciplineService.ExportDisciplineSummaryReport(criteria, disciplines, templatePath);

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

            IList<DisciplineDetailModel> disciplines = _disciplineService.GetDisciplineDetail(criteria);

            string templatePath = Server.MapPath(DisciplineDetailTemplatePath);
            _disciplineService.ExportDisciplineDetailReport(criteria, disciplines, templatePath);

            return new EmptyResult();
        }

        private ActionResult ExcelFile(HSSFWorkbook hssfworkbook, string fileName)
        {
            MemoryStream output = new MemoryStream();
            hssfworkbook.Write(output);

            // Return the result to the end user
            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
        }
    }
}