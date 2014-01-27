// -----------------------------------------------------------------------
// <copyright file="OvertimeRequestService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class OvertimeRequestService : IOvertimeRequestService
    {
        private readonly IOvertimeRequestRepository _overtimeRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmailService _emailService;

        public OvertimeRequestService()
        {
            this._overtimeRepository = new OvertimeRequestRepository();
            this._employeeRepository = new EmployeeRepository();
            this._emailService = new EmailService();
        }

        public IList<OvertimeRequest> GetOvertimeRequests(int empId, DateTime requestMonth)
        {
            return _overtimeRepository.GetOvertimeRequests(empId, requestMonth);
        }

        /// <summary>
        /// Method Update Overtime Request.
        /// </summary>
        /// <param name="model">MyOvertime ViewModel.</param>
        /// <param name="modelState">MyOvertime ModelState.</param>
        /// <returns>Boolean result.</returns>
        public bool UpdateOvertimeRequest(MyOverTimeViewModel model, ModelStateDictionary modelState)
        {
            int errorCode;
            string message;
            var result = _overtimeRepository.UpdateOvertimeRequest(model, out errorCode, out message);
            if (!result)
            {
                modelState.AddModelError(string.Empty, message);
                return false;
            }

            return result;
        }

        public IList<RequestEntity> GetOvertimeReportInYear(int empId, int year)
        {
            return _overtimeRepository.GetOvertimeReportInYear(empId, year);
        }

        public IList<OvertimeExcelReport> OvertimeRecord(DateTime requestDate, int calendar, int? empId)
        {
            int errorCode;
            string message;

            var result = _overtimeRepository.OvertimeRecord(requestDate, calendar, empId, out errorCode, out message);

            return result;
        }

        /// <summary>
        /// Gets the overtime request.
        /// </summary>
        /// <param name="role">The role identifier.</param>
        /// <param name="fromDate">Get result by From Date.</param>
        /// <param name="toDate">Get result by To Date.</param>
        /// <param name="groupIds">The group ids.</param>
        /// <param name="isDailyRequest">If set to <c>true</c> [is daily request].</param>
        /// <param name="employeeId">The employee identifier.</param>
        /// <param name="status">Get result by Status.</param>
        /// <param name="loginId">The login identifier.</param>
        /// <returns>The list of RequestEntity.</returns>
        public IList<RequestEntity> GetOvertimeRequests(
            string role,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? loginId = null)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            try
            {
                if (fromDate <= toDate)
                {
                    entityList = this._overtimeRepository.GetOvertimeRequests(role, fromDate, toDate, groupIds, isDailyRequest, employeeId, status, loginId);
                }
            }
            catch (Exception ex)
            {
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
        }

        /// <summary>
        /// Function to get Overtime request history.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="requestTypeId">Request type Id.</param>
        /// <returns>RequestEntity List.</returns>
        public IList<RequestEntity> GetHistoricOvertimeRequests(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            try
            {
                entityList = this._overtimeRepository.GetOvertimeRequestHistory(requestId, requestTypeId);
            }
            catch (Exception ex)
            {
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
        }

        public IList<OvertimeAnnualReport> GetYearlyOvertimeReport(int year, string team, int? empId, int supervisorId, string status)
        {
            return _overtimeRepository.GetYearlyOvertimeReport(year, team, empId, supervisorId, status);
        }

        public IList<OvertimeAnnualReport> GetYearlyOvertimeReportByNormalCalendar(int year, string team, int? empId, int supervisorId, string status)
        {
            return _overtimeRepository.GetYearlyOvertimeReportByNormalCalendar(year, team, empId, supervisorId, status);
        }

        public IList<RequestEntity> GetMonthlyOvertimeReport(DateTime date, int? empId, string groupids, string status)
        {
            return _overtimeRepository.GetMonthlyOvertimeReport(date, empId, groupids, status);
        }

        public IList<RequestEntity> GetMonthlyOvertimeReportByNormalCalendar(DateTime fromDate, DateTime toDate, int? empId, string groupids, string status)
        {
            return _overtimeRepository.GetMonthlyOvertimeReportByNormalCalendar(fromDate, toDate, empId, groupids, status);
        }

        public RequestEntity GetOvertimeRequestById(int requestId)
        {
            return _overtimeRepository.GetOvertimeRequestById(requestId);
        }

        public IList<OvertimeRequest> HRGetOvertimeRequest(int supervisorId, int empId, DateTime requestMonth)
        {
            return _overtimeRepository.HRGetOvertimeRequest(supervisorId, empId, requestMonth);
        }

        public IList<OvertimeRequest> HRGetNormalOvertimeRequest(int supervisorId, int empId, DateTime requestMonth)
        {
            return _overtimeRepository.HRGetNormalOvertimeRequest(supervisorId, empId, requestMonth);
        }

        public IList<OvertimeRequest> GetMemberOvertimeDaily(int supervisorId, string roles, int employeeId, DateTime month)
        {
            var memberOvertime = _overtimeRepository.GetMemberOvertimeDaily(supervisorId, roles, employeeId, month);

            return memberOvertime;
        }

        public IList<RequestEntity> GetMemberOvertimeMonthly(int supervisorId, string roles, DateTime month, int? employeeId, string teamIds, string status)
        {
            var memberOvertime = _overtimeRepository.GetMemberOvertimeMonthly(supervisorId, roles, month, employeeId, teamIds, status);

            return memberOvertime;
        }

        public void SendMailToReminderClaimOvertime(string subject, string content)
        {
            var employees = _employeeRepository.GetEmployeeCanClaimOvertime();
            if (employees != null)
            {
                employees = employees.Where(t => !string.IsNullOrWhiteSpace(t.Email)).ToList();
                var mailTo = string.Join(";", employees.Select(x => x.Email));
                this._emailService.SendEmail(ConfigSettings.EmailSetting, mailTo, string.Empty, subject, content);
            }
        }

        public IList<OvertimeRequest> OTExcelReportDetails(DateTime fromDate, DateTime toDate, int? empId, string jobTitle, string groups, byte? isLocal = null)
        {
            return _overtimeRepository.OTExcelReportDetails(fromDate, toDate, empId, jobTitle, groups, isLocal);
        }

        /// <summary>
        /// Function to check invalid overtimes.
        /// </summary>
        /// <param name="requestIds">Request ids.</param>
        /// <returns>Result of check.</returns>
        public bool CheckInvalidOvertimes(string requestIds)
        {
            var isInvalid = false;
            var overtimes = _overtimeRepository.GetOvertimeByRequestIds(requestIds);

            if (overtimes != null && overtimes.Any())
            {
                var index = 0;
                while (!isInvalid && index < overtimes.Count())
                {
                    isInvalid = CheckInvalidOvertimes(overtimes[index]);
                    index++;
                }
            }

            return isInvalid;
        }

        /// <summary>
        /// Function to check invalid overtime.
        /// </summary>
        /// <param name="overtime">Overtime Request.</param>
        /// <returns>Result of check.</returns>
        private bool CheckInvalidOvertimes(OvertimeRequest overtime)
        {
            var isInvalid = false;
            if ((!overtime.IsHoliday && !overtime.IsOffDay) || overtime.IsKeepInView)
            {
                if (overtime.OffHours != 0 || overtime.PublicHours != 0 || overtime.TotalWorkHours < (overtime.TotalShiftHours + overtime.NormalHours))
                {
                    isInvalid = true;
                }
            }
            else if (overtime.IsOffDay)
            {
                if (overtime.NormalHours != 0 || overtime.PublicHours != 0 || overtime.TotalWorkHours < overtime.OffHours)
                {
                    isInvalid = true;
                }
            }
            else if (overtime.IsHoliday)
            {
                if (overtime.NormalHours != 0 || overtime.OffHours != 0 || overtime.TotalWorkHours < overtime.PublicHours)
                {
                    isInvalid = true;
                }
            }

            return isInvalid;
        }
    }
}
