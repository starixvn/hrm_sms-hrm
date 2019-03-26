namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models.Report;

    public interface IExportWeekendLeaveDetailService
    {
        void Export(GetWeekendLeaveDetailCriteria criteria, IList<WeekendLeaveDetail> weekendLeaveDetails);
    }
}