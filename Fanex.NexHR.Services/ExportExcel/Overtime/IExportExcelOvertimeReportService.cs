namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public interface IExportExcelOvertimeReportService
    {
        void Export(IList<OvertimeDaily> overtimeDetails, DateTime fromDate, DateTime toDate);
    }
}