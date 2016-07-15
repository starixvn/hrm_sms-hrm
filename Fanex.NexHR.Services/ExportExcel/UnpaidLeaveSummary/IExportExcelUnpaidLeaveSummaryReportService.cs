namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public interface IExportExcelUnpaidLeaveSummaryReportService
    {
        void Export(IList<LeaveSummaryByTeam> leaveSummaries, int year);
    }
}