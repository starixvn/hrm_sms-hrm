namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.ViewModels;

    public class ExportExcelYearlyOvertimeReportService : IExportExcelYearlyOvertimeReportService
    {
        public const int RowStartHeader = 2;
        public const int RowStartBody = 4;
        public const int RowFooter = 6;
        public const int TotalColumns = 17;
        private const string YearlyOvertimeTemplatePath = @"~\ExcelTemplates\YearlyOvertimeReportTemplate.xls";

        public void Export(IList<OvertimeAnnualReportViewModel> overtimeDetails, int year, int calendar)
        {
            string templatePath = HttpContext.Current.Server.MapPath(YearlyOvertimeTemplatePath);
            string fileName = "YearlyOvertimeReport_" + DateTime.Now.ToString("yyyyMMdd");
            string calendarType = calendar == 1 ? "Fiscal Calendar" : "Normal Calendar";

            var writer = new ExcelTemplateWriter(templatePath, fileName, "Yearly Overtime Report");

            /// Fill report title.
            writer.CopyRows(0, 2);

            /// Fill report header.
            writer.CopyRows(RowStartHeader, 2);
            writer.WriteCellRange(2, 1, 4, 12, "Overtime Summary In " + year + " (" + calendarType + ")");

            FillContentRowReport(overtimeDetails, ref writer);
            FillContentRowFooterReport(overtimeDetails, ref writer);
            writer.SyncColumnsWidth(TotalColumns);
            writer.CreateFreezePane(0, RowStartBody);

            writer.Render();
        }

        private void FillContentRowReport(IList<OvertimeAnnualReportViewModel> overtimeDetails, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (OvertimeAnnualReportViewModel overtimeDetail in overtimeDetails)
            {
                FillRowReport(overtimeDetail, ref writer, ref rowIndex);
            }
        }

        private void FillRowReport(OvertimeAnnualReportViewModel overtimeDetail, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordReport(overtimeDetail, rowIndex);
            if (rowIndex % 2 != 0)
            {
                writer.WriteRow(RowStartBody, record);
            }
            else
            {
                writer.WriteRow(RowStartBody + 1, record);
            }

            rowIndex++;
        }

        private object CreateRecordReport(OvertimeAnnualReportViewModel overtimeDetail, int rowIndex)
        {
            double? nullValue = null;
            double totalHours = overtimeDetail.Jan + overtimeDetail.Feb + overtimeDetail.Mar + overtimeDetail.Apr
                    + overtimeDetail.May + overtimeDetail.Jun + overtimeDetail.Jul + overtimeDetail.Aug
                    + overtimeDetail.Sep + overtimeDetail.Oct + overtimeDetail.Nov + overtimeDetail.Dec;

            var record = new
            {
                Count = rowIndex,
                Section = overtimeDetail.Team,
                FullName = overtimeDetail.FullName,
                NickName = overtimeDetail.NickName,
                Jan = !overtimeDetail.Jan.Equals(0) ? (double)overtimeDetail.Jan : nullValue,
                Feb = !overtimeDetail.Feb.Equals(0) ? (double)overtimeDetail.Feb : nullValue,
                Mar = !overtimeDetail.Mar.Equals(0) ? (double)overtimeDetail.Mar : nullValue,
                Apr = !overtimeDetail.Apr.Equals(0) ? (double)overtimeDetail.Apr : nullValue,
                May = !overtimeDetail.May.Equals(0) ? (double)overtimeDetail.May : nullValue,
                Jun = !overtimeDetail.Jun.Equals(0) ? (double)overtimeDetail.Jun : nullValue,
                Jul = !overtimeDetail.Jul.Equals(0) ? (double)overtimeDetail.Jul : nullValue,
                Aug = !overtimeDetail.Aug.Equals(0) ? (double)overtimeDetail.Aug : nullValue,
                Sep = !overtimeDetail.Sep.Equals(0) ? (double)overtimeDetail.Sep : nullValue,
                Oct = !overtimeDetail.Oct.Equals(0) ? (double)overtimeDetail.Oct : nullValue,
                Nov = !overtimeDetail.Nov.Equals(0) ? (double)overtimeDetail.Nov : nullValue,
                Dec = !overtimeDetail.Dec.Equals(0) ? (double)overtimeDetail.Dec : nullValue,
                TotalHours = !totalHours.Equals(0) ? (double)totalHours : nullValue
            };

            return record;
        }

        private void FillContentRowFooterReport(IList<OvertimeAnnualReportViewModel> overtimeDetails, ref ExcelTemplateWriter writer)
        {
            float jan = overtimeDetails.Sum(x => x.Jan);
            float feb = overtimeDetails.Sum(x => x.Feb);
            float mar = overtimeDetails.Sum(x => x.Mar);
            float apr = overtimeDetails.Sum(x => x.Apr);
            float may = overtimeDetails.Sum(x => x.May);
            float jun = overtimeDetails.Sum(x => x.Jun);
            float jul = overtimeDetails.Sum(x => x.Jul);
            float aug = overtimeDetails.Sum(x => x.Aug);
            float sep = overtimeDetails.Sum(x => x.Sep);
            float oct = overtimeDetails.Sum(x => x.Oct);
            float nov = overtimeDetails.Sum(x => x.Nov);
            float dec = overtimeDetails.Sum(x => x.Dec);

            var record = new
            {
                Count = Labels.TotalHours,
                Section = string.Empty,
                FullName = string.Empty,
                NickName = string.Empty,
                Jan = (double)jan,
                Feb = (double)feb,
                Mar = (double)mar,
                Apr = (double)apr,
                May = (double)may,
                Jun = (double)jun,
                Jul = (double)jul,
                Aug = (double)aug,
                Sep = (double)sep,
                Oct = (double)oct,
                Nov = (double)nov,
                Dec = (double)dec,
                TotalHours = (double)(jan + feb + mar + apr + may + jun + jul + aug + sep + oct + nov + dec)
            };

            writer.WriteRow(6, record);
        }
    }
}