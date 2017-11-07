namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models.Report;

    public interface IExportWeekendLeaveService
    {
        void ExportWeekendLeaveSummay(GetWeekendLeaveSummaryCriteria criteria, IList<WeekendLeaveSummary> weekendLeaves);

        //void ExportWeekendLeaveDetail(GetWeekendLeaveSummaryCriteria criteria, IList<WeekendLeaveSummary> weekendLeaveDetails);
    }
}