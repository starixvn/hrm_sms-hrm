namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using ExportExcel;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;

    public class ExportExcelOvertimeReportService : IExportExcelOvertimeReportService
    {
        public const int RowStartHeader = 2;
        public const int RowStartBody = 3;
        public const int TotalColumns = 12;
        private const string OvertimeTemplatePath = @"~\ExcelTemplates\OvertimeReportTemplate.xls";

        public void Export(IList<OvertimeDaily> overtimeDetails, DateTime fromDate, DateTime toDate)
        {
            string templatePath = HttpContext.Current.Server.MapPath(OvertimeTemplatePath);
            string fileName = "OvertimeReport_" + DateTime.Now.ToString("yyyyMMdd");
            string reportTitle = "Overtime Report Details" + " from " + fromDate.ToString("dd-MMM-yyyy") + " to " + toDate.ToString("dd-MMM-yyyy");

            var writer = new ExcelTemplateWriter(templatePath, fileName, "Overtime Detail Report");

            /// Fill report title.
            writer.WriteRow(0, new object[] { reportTitle });
            writer.CopyRows(1, 1);

            /// Fill report header.
            writer.CopyRows(RowStartHeader, 1);

            FillContentRowReport(overtimeDetails, ref writer);
            writer.SyncColumnsWidth(TotalColumns);
            writer.CreateFreezePane(0, RowStartBody);

            writer.Render();
        }

        private void FillContentRowReport(IList<OvertimeDaily> overtimeDetails, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (OvertimeDaily overtimeDetail in overtimeDetails)
            {
                FillRowReport(overtimeDetail, ref writer, ref rowIndex);
            }
        }

        private void FillRowReport(OvertimeDaily overtimeDetail, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordReport(overtimeDetail, rowIndex);
            ExportExcelService.WriteRowExcelReport(writer, rowIndex, RowStartBody, record);
        }

        private object CreateRecordReport(OvertimeDaily overtimeDetail, int rowIndex)
        {
            double? nullValue = null;
            CurrentRequestStatus status = (CurrentRequestStatus)overtimeDetail.Status;

            var record = new
            {
                Count = rowIndex,
                Date = overtimeDetail.OvertimeDate.ToDateMonthYearFormatedDate(),
                Code = overtimeDetail.Code,
                Employee = overtimeDetail.FullName + " (" + overtimeDetail.NickName + ")",
                Team = overtimeDetail.Team,
                Schedule = overtimeDetail.Schedule,
                TimeIn = overtimeDetail.TimeIn,
                TimeOut = overtimeDetail.TimeOut,
                TotalHours = overtimeDetail.TotalWorkHours > 0 ? Math.Round(overtimeDetail.TotalWorkHours, 2) : nullValue,
                NormalDay = overtimeDetail.NormalHours > 0 ? (double)overtimeDetail.NormalHours : nullValue,
                OffDay = overtimeDetail.OffHours > 0 ? (double)overtimeDetail.OffHours : nullValue,
                PublicHoliday = overtimeDetail.PublicHours > 0 ? (double)overtimeDetail.PublicHours : nullValue,
                Remark = overtimeDetail.Remark,
                Status = status.ToString()
            };

            return record;
        }
    }
}