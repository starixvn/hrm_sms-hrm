// -----------------------------------------------------------------------
// <copyright file="IOvertimeRequestService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Paul.Nguyen" date="2013.12.03">Created</change>
// </history>

namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
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

        IList<RequestEntity> GetMonthlyOvertimeReport(GetMemberOvertimeMonthlyCriteria criteria);

        IList<RequestEntity> GetMonthlyOvertimeReportByNormalCalendar(GetMonthlyOvertimeReportByNormalCalendarCriteria criteria);

        IList<OvertimeRequest> HRGetOvertimeRequest(GetOvertimeMonthlyByHRCriteria criteria);

        IList<OvertimeRequest> HRGetNormalOvertimeRequest(GetOvertimeNormalDailyByHRCriteria criteria);

        RequestEntity GetOvertimeRequestById(GetOvertimeByRequestIdCriteria criteria);

        IList<RequestEntity> GetMemberOvertimeMonthly(GetMemberOvertimeMonthlyCriteria criteria);

        void SendMailToReminderClaimOvertime(string subject, string content);

        bool CheckInvalidOvertimes(GetDetailOvertimeByRequestIdsCriteria criteria);

        IList<OvertimeRequest> OTExcelReportDetails(GetOvertimeExcelReportDetailCriteria criteria);

        IList<OvertimeExcelReport> OvertimeRecord(GetOvertimeRecordCriteria criteria); 
    }
}
