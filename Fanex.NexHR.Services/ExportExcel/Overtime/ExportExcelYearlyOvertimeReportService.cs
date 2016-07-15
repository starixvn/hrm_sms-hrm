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
            string templatePath = HttpContext.Current.Server.MapPath(YearlyOvertimeTemplatePath),
                fileName = "YearlyOvertimeReport_" + DateTime.Now.ToString("yyyyMMdd"),
                calendarType = calendar == 1 ? "Fiscal Calendar" : "Normal Calendar";

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
            if (rowIndex % 2 == 1)
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
            double? nullValue = null,
                totalHours = overtimeDetail.Jan + overtimeDetail.Feb + overtimeDetail.Mar + overtimeDetail.Apr
                    + overtimeDetail.May + overtimeDetail.Jun + overtimeDetail.Jul + overtimeDetail.Aug
                    + overtimeDetail.Sep + overtimeDetail.Oct + overtimeDetail.Nov + overtimeDetail.Dec;

            var record = new
            {
                Count = rowIndex,
                Section = overtimeDetail.Team,
                FullName = overtimeDetail.FullName,
                NickName = overtimeDetail.NickName,
                Jan = overtimeDetail.Jan != 0 ? (double)overtimeDetail.Jan : nullValue,
                Feb = overtimeDetail.Feb != 0 ? (double)overtimeDetail.Feb : nullValue,
                Mar = overtimeDetail.Mar != 0 ? (double)overtimeDetail.Mar : nullValue,
                Apr = overtimeDetail.Apr != 0 ? (double)overtimeDetail.Apr : nullValue,
                May = overtimeDetail.May != 0 ? (double)overtimeDetail.May : nullValue,
                Jun = overtimeDetail.Jun != 0 ? (double)overtimeDetail.Jun : nullValue,
                Jul = overtimeDetail.Jul != 0 ? (double)overtimeDetail.Jul : nullValue,
                Aug = overtimeDetail.Aug != 0 ? (double)overtimeDetail.Aug : nullValue,
                Sep = overtimeDetail.Sep != 0 ? (double)overtimeDetail.Sep : nullValue,
                Oct = overtimeDetail.Oct != 0 ? (double)overtimeDetail.Oct : nullValue,
                Nov = overtimeDetail.Nov != 0 ? (double)overtimeDetail.Nov : nullValue,
                Dec = overtimeDetail.Dec != 0 ? (double)overtimeDetail.Dec : nullValue,
                TotalHours = totalHours != 0 ? (double)totalHours : nullValue
            };

            return record;
        }

        private void FillContentRowFooterReport(IList<OvertimeAnnualReportViewModel> overtimeDetails, ref ExcelTemplateWriter writer)
        {
            float jan = overtimeDetails.Sum(x => x.Jan),
                feb = overtimeDetails.Sum(x => x.Feb),
                mar = overtimeDetails.Sum(x => x.Mar),
                apr = overtimeDetails.Sum(x => x.Apr),
                may = overtimeDetails.Sum(x => x.May),
                jun = overtimeDetails.Sum(x => x.Jun),
                jul = overtimeDetails.Sum(x => x.Jul),
                aug = overtimeDetails.Sum(x => x.Aug),
                sep = overtimeDetails.Sum(x => x.Sep),
                oct = overtimeDetails.Sum(x => x.Oct),
                nov = overtimeDetails.Sum(x => x.Nov),
                dec = overtimeDetails.Sum(x => x.Dec);

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