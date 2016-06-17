namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Overtime;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels.Overtime;

    public interface IOvertimeRequestService
    {
        OvertimeRequest GetOvertimeRequestByMonth(GetOvertimeRequestByMonthCriteria criteria);

        OvertimeRequest GetOvertimeRequestById(GetOvertimeRequestByIdCriteria criteria);

        OvertimeRequest GetOvertimeDailyReport(GetOvertimeDailyReportCriteria criteria);

        IList<OvertimeDaily> GetOvertimeRequests(GetOvertimeMonthlyByEmployeeIdCriteria criteria);

        IList<RequestEntity> GetOvertimeReportInYear(GetOvertimeReportInYearCriteria criteria);

        IList<OvertimeAnnualReportViewModel> GetYearlyOvertimeReport(GetYearlyOvertimeReportCriteria criteria);

        IList<OvertimeAnnualReportViewModel> GetYearlyOvertimeReportByNormalCalendar(GetYearlyOvertimeReportByNormalCalendarCriteria criteria);

        IList<RequestEntity> GetMonthlyOvertimeReport(GetMemberOvertimeCriteria criteria);

        IList<RequestEntity> GetMonthlyOvertimeReportByNormalCalendar(GetMonthlyOvertimeReportByNormalCalendarCriteria criteria);

        IList<OvertimeDaily> HRGetOvertimeRequest(GetOvertimeMonthlyByHRCriteria criteria);

        IList<OvertimeDaily> HRGetNormalOvertimeRequest(GetOvertimeNormalDailyByHRCriteria criteria);

        IList<OvertimeRequest> GetMemberOvertime(GetMemberOvertimeCriteria criteria);

        void SendMailToReminderClaimOvertime(string subject, string content);

        bool CheckInvalidOvertimes(GetDetailOvertimeByRequestIdsCriteria criteria);

        IList<OvertimeDaily> OTExcelReportDetails(GetOvertimeExcelReportDetailCriteria criteria);

        IList<OvertimeExcelReport> OvertimeRecord(GetOvertimeRecordCriteria criteria); 
    }
}
