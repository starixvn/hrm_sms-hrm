namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;
    using NPOI.HSSF.UserModel;

    public interface IExportExcelService
    {
        HSSFWorkbook ExportLeaveSummary(IList<LeaveSummaryByTeam> leaveSummaryList, int year);

        HSSFWorkbook ExportWorkingTimeRequest(IList<RequestEntity> requestList, string title);

        HSSFWorkbook ExportEmployeeWorkingCalendar(IList<WorkingCalendarsViewModel> workingCalendars, DateTime fromDate, DateTime toDate);

        HSSFWorkbook ExportWorkingCalendarSummary(List<WorkingCalendarSummary> calendars, DateTime fromDate, DateTime toDate);
    }
}