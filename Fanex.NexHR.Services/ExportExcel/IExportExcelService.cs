namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using NPOI.HSSF.UserModel;

    public interface IExportExcelService
    {
        HSSFWorkbook ExportLeaveSummary(IList<LeaveSummary> leaveSummaryList, int year);

        HSSFWorkbook ExportWorkingTimeRequest(IList<RequestEntity> requestList, string title);
    }
}