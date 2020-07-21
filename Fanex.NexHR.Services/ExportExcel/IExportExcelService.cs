namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Models.Request;
    using NPOI.HSSF.UserModel;

    public interface IExportExcelService
    {
        HSSFWorkbook ExportLeaveSummary(IList<LeaveSummary> leaveSummaryList, int year);

        HSSFWorkbook ExportWorkingTimeRequest(IList<RequestEntity> requestList, string title);

        HSSFWorkbook ExportEmployeeWorkingCalendar(IEnumerable<WorkingCalendar> workingCalendars, DateTime fromDate, DateTime toDate);
    }
}