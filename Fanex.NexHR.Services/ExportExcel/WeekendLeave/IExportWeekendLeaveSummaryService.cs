namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models.Report;

    public interface IExportWeekendLeaveSummaryService
    {
        void Export(GetWeekendLeaveSummaryCriteria criteria, IList<WeekendLeaveSummary> weekendLeaves);
    }
}