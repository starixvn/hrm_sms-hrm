namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.ViewModels;

    public interface IExportExcelAttendanceReportService
    {
        void Export(IList<EmployeeAttendanceModel> attendances, DateTime fromDate, DateTime toDate);
    }
}