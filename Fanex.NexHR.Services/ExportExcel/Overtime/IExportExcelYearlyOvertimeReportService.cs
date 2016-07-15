namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.ViewModels;

    public interface IExportExcelYearlyOvertimeReportService
    {
        void Export(IList<OvertimeAnnualReportViewModel> overtimeDetails, int year, int calendar);
    }
}