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
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Report.Attendance;
    using Fanex.NexHR.Criteria.Report.WorkingHour;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.Overtime;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;
    using NPOI.HSSF.UserModel;

    /// <summary>
    /// Export Controller.
    /// </summary>
    public class ExportController : RequestController
    {
        private readonly IReportService _reportService;
        private readonly IExportExcelService _exportService;
        private readonly IRequestService _requestService;
        private readonly ITeamService _teamService;
        private readonly IOvertimeRequestService _overtimeService;
        private readonly IEmployeeService _employeeService;
        private readonly IWorkingCalendarService _workingCalendarService;

        public ExportController(
            IReportService reportService = null,
            IExportExcelService exportService = null,
            IRequestService requestService = null,
            ITeamService teamService = null,
            IOvertimeRequestService overtimeService = null,
            IEmployeeService employeeService = null,
            IWorkingCalendarService workingCalendarService = null)
        {
            this._reportService = reportService ?? new ReportService();
            this._exportService = exportService ?? new ExportExcelService();
            this._requestService = requestService ?? new RequestService();
            this._teamService = teamService ?? new TeamService();
            this._overtimeService = overtimeService ?? new OvertimeRequestService();
            this._employeeService = employeeService ?? new EmployeeService();
            this._workingCalendarService = workingCalendarService ?? new WorkingCalendarService();
        }

        public ExportController()
            : this(null, null, null, null, null, null, null)
        {
        }

        /// <summary>
        /// Export excel Total Working Hours.
        /// </summary>
        /// <param name="model">TotalWorkingHours ViewModel.</param>
        /// <returns>ActionResult Total Working Hours.</returns>
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

        /// <summary>
        /// Export excel Attendance report.
        /// </summary>
        /// <param name="model">EmployeeAttendanceViewModel object.</param>
        /// <returns>ActionResult Attendance report.</returns>
        public ActionResult Attendance(EmployeeAttendanceViewModel model)
        {
            string fileName = "AttendanceReport_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            var criteria = new GetEmployeeAttendanceCriteria()
            {
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

        /// <summary>
        /// ActionResult LeaveSummary.
        /// </summary>
        /// <param name="model">LeaveSummary ViewModel.</param>
        /// <returns>View LeaveSummary.</returns>
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
            var leaveSummaryByTeamList = _reportService.GetAnnualLeaveByTeam(leaveSummary, teams);

            var hssfworkbook = _exportService.ExportLeaveSummary(leaveSummaryByTeamList, model.Year);

            return ExcelFile(hssfworkbook, fileName);
        }

        /// <summary>
        /// Get overtime report.
        /// </summary>
        /// <param name="fromDate">The from date.</param>
        /// <param name="toDate">The to date.</param>
        /// <param name="empId">The employee Id.</param>
        /// <param name="jobTitle">The job title.</param>
        /// <param name="groups">The groups.</param>
        /// <returns>The file excel.</returns>
        public ActionResult OvertimeReport(DateTime fromDate, DateTime toDate, int? empId, string jobTitle, string groups)
        {
            string fileName = "OvertimeReport_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            var criteria = new GetOvertimeExcelReportDetailCriteria
            {
                FromDate = fromDate,
                ToDate = toDate,
                EmployeeID = empId,
                JobTitleId = jobTitle,
                GroupIds  = groups
            };
            IList<OvertimeRequest> entity = _overtimeService.OTExcelReportDetails(criteria).OrderBy(x => x.OvertimeDate).ToList();
            var hssfworkbook = _exportService.ExportOvertimeReport(entity, fromDate, toDate);

            return ExcelFile(hssfworkbook, fileName);
        }

        /// <summary>
        /// Method Export yearly overtime report.
        /// </summary>
        /// <param name="year">Year to export.</param>
        /// <param name="team">Team filter.</param>
        /// <param name="empId">Employee filter.</param>
        /// <param name="calendar">Calendar type.</param>
        /// <returns>Action Result.</returns>
        public ActionResult YearlyOvertimeReport(int year, string team, int? empId, int? calendar)
        {
            string fileName = "YearlyOvertimeReport_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            IList<OvertimeAnnualReport> entity = null;
            if (calendar == 1)
            {
                var getYearlyOvertimeReportCriteria = new GetYearlyOvertimeReportCriteria
                {
                    SupervisorID = this.CurrentUser.Id,
                    Year = year,
                    GroupIds = team,
                    EmployeeId = empId,
                    Status = null
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
                    Status = null
                };
                entity = this._overtimeService.GetYearlyOvertimeReportByNormalCalendar(criteria);
            }

            var hssfworkbook = _exportService.ExportYearlyOvertimeReport(entity, year, calendar);

            return ExcelFile(hssfworkbook, fileName);
        }

        /// <summary>
        /// Export Excel Employee Basic Info.
        /// </summary>
        /// <param name="query">Query to DB.</param>
        /// <param name="groupIds">Query group Id.</param>
        /// <param name="enable">User enable or not.</param>
        /// <param name="isDelete">User is deleted.</param>
        /// <param name="titleId">Job title.</param>
        /// <param name="isLocal">Expat or Local.</param>
        /// <returns>Action Result.</returns>
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

        /// <summary>
        /// Export Excel Employee Detailed Info.
        /// </summary>
        /// <param name="query">Query to DB.</param>
        /// <param name="groupIds">Query group Id.</param>
        /// <param name="enable">User enable or not.</param>
        /// <param name="isDelete">User is deleted.</param>
        /// <param name="titleId">Job title.</param>
        /// <param name="isLocal">Expat or Local.</param>
        /// <returns>Action Result.</returns>
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
            var entity = _workingCalendarService.GetWorkingCalendars(null, fromDate, toDate, group, title);
            var hssfworkbook = _exportService.ExportEmployeeWorkingCalendar(entity, fromDate, toDate);

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult WorkingCalendarSummary(
            DateTime? fromDate,
            DateTime? toDate,
            string group,
            string title,
            int? employeeId,
            byte? isLocal = null)
        {
            string fileName = "WorkingCalendarSummary_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            HSSFWorkbook hssfworkbook = null;

            return ExcelFile(hssfworkbook, fileName);
        }

        /// <summary>
        /// Employee Overtime Record.
        /// </summary>
        /// <param name="year">Requested year.</param>
        /// <param name="month">Requested month.</param>
        /// <param name="calendar">Fiscal or Normal calendar.</param>
        /// <param name="empId">Employee id.</param>
        /// <returns>View EmployeeOvertimeRecord.</returns>
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

        /// <summary>
        /// ActionResult ViewWorkingTimeRequest.
        /// </summary>
        /// <param name="model">Request ViewModel.</param>
        /// <returns>View Working Time Request.</returns>
        public ActionResult ViewWorkingTimeRequest(RequestViewModel model)
        {
            string fileName = "WorkingTimeRequest_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            bool? isLocal = null;
            var criteria = new GetRequestCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupIds = model.TeamIds,
                Status = GetStatus(model.StatusId),
                RequestType = model.RequestType,
                IsQueryByCreatedDate = !model.IsDailyRequest,
                Role = CheckRoleAccordingToRequestType(RequestViewType.HRSpace),
                SupervisorId = null,
                IsLocal = model.IsLocal.HasValue ? (model.IsLocal.Value == 1) : isLocal
            };

            IList<RequestEntity> requestList = _requestService.GetRequests(criteria);
            var hssfworkbook = _exportService.ExportWorkingTimeRequest(requestList);

            return ExcelFile(hssfworkbook, fileName);
        }

        /// <summary>
        /// ActionResult MemberWorkingTimeRequest.
        /// </summary>
        /// <param name="model">Request ViewModel.</param>
        /// <returns>View Member working time request.</returns>
        [Authorize(Roles = "Verifier, Approver")]
        public ActionResult MemberWorkingTimeRequest(RequestViewModel model)
        {
            string fileName = "WorkingTimeRequest_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            model.ToDate = Utilities.AddTimeToDate(model.ToDate, 23, 59, 59);

            bool? isLocal = null;
            var criteria = new GetRequestCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupIds = model.TeamIds,
                Status = GetStatus(model.StatusId),
                RequestType = model.RequestType,
                IsQueryByCreatedDate = !model.IsDailyRequest,
                Role = CheckRoleAccordingToRequestType(RequestViewType.PersonalSpace),
                SupervisorId = null,
                IsLocal = model.IsLocal.HasValue ? (model.IsLocal.Value == 1) : isLocal
            };

            IList<RequestEntity> requestList = _requestService.GetRequests(criteria);

            var hssfworkbook = _exportService.ExportWorkingTimeRequest(requestList);
            return ExcelFile(hssfworkbook, fileName);
        }

        /// <summary>
        ///  ActionResult Human Resource ViewWorkingTimeRequest.
        /// </summary>
        /// <param name="model">Request ViewModel.</param>
        /// <returns>View Working Time Request.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult HRViewWorkingTimeRequest(RequestViewModel model)
        {
            string fileName = "WorkingTimeRequest_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            model.ToDate = Utilities.AddTimeToDate(model.ToDate, 23, 59, 59);

            bool? isLocal = null;
            var criteria = new GetRequestCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupIds = model.TeamIds,
                Status = GetStatus(model.StatusId),
                RequestType = model.RequestType,
                IsQueryByCreatedDate = !model.IsDailyRequest,
                Role = CheckRoleAccordingToRequestType(RequestViewType.HRSpace),
                SupervisorId = null,
                IsLocal = model.IsLocal.HasValue ? (model.IsLocal.Value == 1) : isLocal
            };

            IList<RequestEntity> requestList = _requestService.GetRequests(criteria);
            var hssfworkbook = _exportService.ExportWorkingTimeRequest(requestList);
            return ExcelFile(hssfworkbook, fileName);
        }

        /// <summary>
        /// ActionResult EmployeeViewWorkingTimeRequest.
        /// </summary>
        /// <param name="model">Request ViewModel.</param>
        /// <returns>View Employee Working Time Request.</returns>
        public ActionResult EmployeeViewWorkingTimeRequest(RequestViewModel model)
        {
            string fileName = "WorkingTimeRequest_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            model.ToDate = Utilities.AddTimeToDate(model.ToDate, 23, 59, 59);

            bool? isLocal = null;
            var criteria = new GetRequestCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupIds = model.TeamIds,
                Status = GetStatus(model.StatusId),
                RequestType = model.RequestType,
                IsQueryByCreatedDate = !model.IsDailyRequest,
                Role = CheckRoleAccordingToRequestType(RequestViewType.CompanySpace),
                SupervisorId = null,
                IsLocal = model.IsLocal.HasValue ? (model.IsLocal.Value == 1) : isLocal
            };

            IList<RequestEntity> requestList = _requestService.GetRequests(criteria);
            var hssfworkbook = _exportService.ExportWorkingTimeRequest(requestList);
            return ExcelFile(hssfworkbook, fileName);
        }

        /// <summary>
        /// ActionResult DailyViewWorkingTimeRequest.
        /// </summary>
        /// <param name="model">Request ViewModel.</param>
        /// <returns>View Daily Working Time Request.</returns>
        public ActionResult DailyViewWorkingTimeRequest(RequestViewModel model)
        {
            string fileName = "WorkingTimeRequest_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            model.ToDate = Utilities.AddTimeToDate(model.ToDate, 23, 59, 59);

            bool? isLocal = null;
            var criteria = new GetRequestCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupIds = model.TeamIds,
                Status = GetStatus(model.StatusId),
                RequestType = model.RequestType,
                IsQueryByCreatedDate = !model.IsDailyRequest,
                Role = CheckRoleAccordingToRequestType(RequestViewType.CompanySpace),
                SupervisorId = null,
                IsLocal = model.IsLocal.HasValue ? (model.IsLocal.Value == 1) : isLocal
            };

            IList<RequestEntity> requestList = _requestService.GetRequests(criteria);
            var hssfworkbook = _exportService.ExportWorkingTimeRequest(requestList);
            return ExcelFile(hssfworkbook, fileName);
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
