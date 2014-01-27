// -----------------------------------------------------------------------
// <copyright file="IOvertimeRequestService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Paul.Nguyen" date="2013.12.03">Created</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IOvertimeRequestService
    {
        IList<OvertimeRequest> GetOvertimeRequests(int empId, DateTime requestMonth);

        bool UpdateOvertimeRequest(MyOverTimeViewModel model, ModelStateDictionary modelState);

        IList<RequestEntity> GetOvertimeReportInYear(int empId, int year);

        IList<RequestEntity> GetOvertimeRequests(
            string role,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? logInId = null);

        IList<RequestEntity> GetHistoricOvertimeRequests(long requestId, int requestTypeId);

        IList<OvertimeAnnualReport> GetYearlyOvertimeReport(int year, string team, int? empId, int supervisorId, string status);

        IList<OvertimeAnnualReport> GetYearlyOvertimeReportByNormalCalendar(int year, string team, int? empId, int supervisorId, string status);

        IList<RequestEntity> GetMonthlyOvertimeReport(DateTime date, int? empId, string groupids, string status);

        IList<RequestEntity> GetMonthlyOvertimeReportByNormalCalendar(DateTime fromDate, DateTime toDate, int? empId, string groupids, string status);

        IList<OvertimeRequest> HRGetOvertimeRequest(int supervisorId, int empId, DateTime requestMonth);

        IList<OvertimeRequest> HRGetNormalOvertimeRequest(int supervisorId, int empId, DateTime requestMonth);

        RequestEntity GetOvertimeRequestById(int requestId);

        IList<OvertimeRequest> GetMemberOvertimeDaily(int supervisorId, string roles, int employeeId, DateTime month);

        IList<RequestEntity> GetMemberOvertimeMonthly(int supervisorId, string roles, DateTime month, int? employeeId, string teamIds, string status);

        void SendMailToReminderClaimOvertime(string subject, string content);

        bool CheckInvalidOvertimes(string requestIds);

        IList<OvertimeRequest> OTExcelReportDetails(DateTime fromDate, DateTime toDate, int? empId, string jobTitle, string groups, byte? isLocal = null);

        IList<OvertimeExcelReport> OvertimeRecord(DateTime requestDate, int calendar, int? empId); 
    }
}
