namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Request;

    public interface IOvertimeRequestRepository
    {
        OvertimeRequest GetOvertimeRequestByMonth(ICriteria criteria);

        OvertimeRequest GetOvertimeRequestById(ICriteria criteria);

        OvertimeRequest GetOvertimeDailyReport(ICriteria criteria);

        IList<RequestEntity> GetOvertimeReportInYear(ICriteria criteria);

        IList<RequestEntity> GetMonthlyOvertimeReport(ICriteria criteria);

        IList<RequestEntity> GetMonthlyOvertimeReportByNormalCalendar(ICriteria criteria);

        IList<OvertimeRequest> GetMemberOvertime(ICriteria criteria);

        IList<OvertimeExcelReport> OvertimeRecord(ICriteria criteria);
    }
}
