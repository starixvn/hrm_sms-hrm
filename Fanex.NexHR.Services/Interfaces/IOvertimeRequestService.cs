namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Request;

    public interface IOvertimeRequestService
    {
        OvertimeRequest GetOvertimeRequestByMonth(GetOvertimeRequestByMonthCriteria criteria);

        OvertimeRequest GetOvertimeDailyReport(GetOvertimeDailyReportCriteria criteria);

        IList<OvertimeDaily> GetOvertimeRequests(GetOvertimeMonthlyByEmployeeIdCriteria criteria);

        IEnumerable<MonthlyOvertimeRequest> GetMonthlyRequestByEmployeeId(int employeeId, int year);

        MonthlyOvertimeRequest GetMonthlyRequestDetailsByEmployeeId(int employeeId, DateTime month);

        IEnumerable<MonthlyOvertimeRequest> GetMemberMonthlyRequest(GetMonthlyOvertimeRequestParam param);

        MonthlyOvertimeRequest GetMonthlyRequestDetailsById(long requestId);

        IList<RequestEntity> GetMonthlyOvertimeReport(GetMemberOvertimeCriteria criteria);

        IList<RequestEntity> GetMonthlyOvertimeReportByNormalCalendar(GetMonthlyOvertimeReportByNormalCalendarCriteria criteria);

        IList<OvertimeDaily> HRGetOvertimeRequest(GetOvertimeMonthlyByHRCriteria criteria);

        IList<OvertimeDaily> HRGetNormalOvertimeRequest(GetOvertimeNormalDailyByHRCriteria criteria);

        void SendMailToReminderClaimOvertime(string subject, string content);

        bool CheckInvalidOvertimes(string requestIds);

        IList<OvertimeDaily> OTExcelReportDetails(GetOvertimeExcelReportDetailCriteria criteria);

        IList<OvertimeExcelReport> OvertimeRecord(GetOvertimeRecordCriteria criteria);

        CommandResult EditMonthlyRequest(int actorId, EditMonthlyOvertimeRequest model);

        CommandResult EditMonthlyRequestStatus(int actorId, string requestIds, string command, byte roleId);
    }
}