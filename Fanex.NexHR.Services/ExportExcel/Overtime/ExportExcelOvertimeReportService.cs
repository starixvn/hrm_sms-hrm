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
            ExportExcelService.WriteRowExcelReport(writer, ref rowIndex, RowStartBody, record);
        }

        private object CreateRecordReport(OvertimeDaily overtimeDetail, int rowIndex)
        {
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
                TotalHours = ValidateHours(overtimeDetail.TotalWorkHours > 0, Math.Round(overtimeDetail.TotalWorkHours, 2)),
                NormalDay = ValidateHours(overtimeDetail.NormalHours > 0, overtimeDetail.NormalHours),
                OffDay = ValidateHours(overtimeDetail.OffHours > 0, overtimeDetail.OffHours),
                PublicHoliday = ValidateHours(overtimeDetail.PublicHours > 0, overtimeDetail.PublicHours),
                Remark = overtimeDetail.Remark,
                Status = status.ToString()
            };

            return record;
        }

        private static double? ValidateHours(bool hourCondition, double value)
        {
            double? nullValue = null;
            return hourCondition ? value : nullValue;
        }
    }
}