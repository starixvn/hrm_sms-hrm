using System.Collections.Generic;
using Fanex.NexHR.Criteria.Report;
using Fanex.NexHR.Models.Report;

namespace Fanex.NexHR.Services.ExportExcel.WeekendLeave
{
    public interface IExportWeekendLeaveDetailService
    {
        void Export(GetWeekendLeaveDetailCriteria criteria, IList<WeekendLeaveDetail> weekendLeaveDetails);
    }
}