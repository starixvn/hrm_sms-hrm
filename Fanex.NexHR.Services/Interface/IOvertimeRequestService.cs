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
    using Fanex.NexHR.Criteria.Request.Overtime;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Interface Overtime Request Service.
    /// </summary>
    public interface IOvertimeRequestService
    {
        IList<OvertimeRequest> GetOvertimeRequests(GetOvertimeMonthlyByEmployeeIdCriteria criteria);

        CommandResult UpdateOvertimeRequest(UpdateOvertimeRequestCriteria criteria);

        IList<RequestEntity> GetOvertimeReportInYear(GetOvertimeReportInYearCriteria criteria);

        IList<OvertimeAnnualReport> GetYearlyOvertimeReport(GetYearlyOvertimeReportCriteria criteria);

        IList<OvertimeAnnualReport> GetYearlyOvertimeReportByNormalCalendar(GetYearlyOvertimeReportByNormalCalendarCriteria criteria);

        IList<RequestEntity> GetMonthlyOvertimeReport(GetMonthlyOvertimeReportCriteria criteria);

        IList<RequestEntity> GetMonthlyOvertimeReportByNormalCalendar(GetMonthlyOvertimeReportByNormalCalendarCriteria criteria);

        IList<OvertimeRequest> HRGetOvertimeRequest(GetOvertimeMonthlyByHRCriteria criteria);

        IList<OvertimeRequest> HRGetNormalOvertimeRequest(GetOvertimeNormalDailyByHRCriteria criteria);

        RequestEntity GetOvertimeRequestById(GetOvertimeByRequestIdCriteria criteria);

        IList<OvertimeRequest> GetMemberOvertimeDaily(int supervisorId, string roles, int employeeId, DateTime month);

        IList<RequestEntity> GetMemberOvertimeMonthly(int supervisorId, string roles, DateTime month, int? employeeId, string teamIds, string status);

        void SendMailToReminderClaimOvertime(string subject, string content);

        bool CheckInvalidOvertimes(string requestIds);

        IList<OvertimeRequest> OTExcelReportDetails(DateTime fromDate, DateTime toDate, int? empId, string jobTitle, string groups, byte? isLocal = null);

        IList<OvertimeExcelReport> OvertimeRecord(DateTime requestDate, int calendar, int? empId); 
    }
}
