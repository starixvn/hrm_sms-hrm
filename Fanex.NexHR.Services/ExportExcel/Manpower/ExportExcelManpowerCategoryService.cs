namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Resources;

    public interface IExportExcelManpowerCategoryService
    {
        void Export(IEnumerable<ManpowerCategoryModel> manpowers, int year);
    }

    public class ExportExcelManpowerCategoryService : IExportExcelManpowerCategoryService
    {
        private const int RowStartHeader = 2;
        private const int RowStartBody = 4;
        private const int RowFooter = 6;
        private const int TotalColumns = 10;
        private const string ManpowerCategoryTemplatePath = @"~\ExcelTemplates\ManpowerCategoryReportTemplate.xls";

        public void Export(IEnumerable<ManpowerCategoryModel> manpowers, int year)
        {
            string templatePath = HttpContext.Current.Server.MapPath(ManpowerCategoryTemplatePath);
            string fileName = "ManpowerCategoryReport_" + DateTime.Now.ToString("yyyyMMdd");
            string reportTitle = Labels.ManpowerCategoryReportTitle + " " + year;

            var writer = new ExcelTemplateWriter(templatePath, fileName, "Manpower Report");

            /// Fill report title.
            writer.WriteRow(0, new object[] { reportTitle });
            writer.CopyRows(1, 1);

            /// Fill report header.
            const byte TotalCopyRows = 2;
            writer.CopyRows(RowStartHeader, TotalCopyRows);

            FillContentRowReport(manpowers, ref writer);
            FillContentRowFooterReport(manpowers, ref writer);
            writer.SyncColumnsWidth(TotalColumns);
            writer.CreateFreezePane(0, RowStartBody);

            writer.Render();
        }

        private void FillContentRowReport(IEnumerable<ManpowerCategoryModel> manpowers, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (ManpowerCategoryModel manpower in manpowers)
            {
                FillRowReport(manpower, ref writer, ref rowIndex);
            }
        }

        private void FillRowReport(ManpowerCategoryModel manpower, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordReport(manpower, rowIndex);
            ExportExcelService.WriteRowExcelReport(writer, rowIndex, RowStartBody, record);
            rowIndex++;
        }

        private object CreateRecordReport(ManpowerCategoryModel manpower, int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                manpower.GroupCode,
                manpower.TotalExpat,
                manpower.TotalLocal,
                manpower.Total,
                manpower.Management,
                AVGManagement = ReturnValidValue(manpower.Total, ((manpower.Management * 1.00) / manpower.Total)),
                manpower.ExecutiveWithPosition,
                AVGExecutiveWithPosition = ReturnValidValue(manpower.Total, ((manpower.ExecutiveWithPosition * 1.00) / manpower.Total)),
                manpower.Executive,
                AVGExecutive = ReturnValidValue(manpower.Total, ((manpower.Executive * 1.00) / manpower.Total)),
                manpower.Staff,
                AVGStaff = ReturnValidValue(manpower.Total, ((manpower.Staff * 1.00) / manpower.Total)),
                manpower.Worker,
                AVGWorker = ReturnValidValue(manpower.Total, ((manpower.Worker * 1.00) / manpower.Total))
            };

            return record;
        }

        private void FillContentRowFooterReport(IEnumerable<ManpowerCategoryModel> manpowers, ref ExcelTemplateWriter writer)
        {
            int total = manpowers.Sum(t => t.Total);
            int management = manpowers.Sum(t => t.Management);
            int executiveWithPosition = manpowers.Sum(t => t.ExecutiveWithPosition);
            int executive = manpowers.Sum(t => t.Executive);
            int staff = manpowers.Sum(t => t.Staff);
            int worker = manpowers.Sum(t => t.Worker);

            var record = new
            {
                Count = Labels.Total,
                GroupCode = string.Empty,
                TotalExpat = manpowers.Sum(t => t.TotalExpat),
                TotalLocal = manpowers.Sum(t => t.TotalLocal),
                Total = total,
                Management = management,
                AVGManagement = ReturnValidValue(total, ((management * 1.00) / total)),
                ExecutiveWithPosition = executiveWithPosition,
                AVGExecutiveWithPosition = ReturnValidValue(total, ((executiveWithPosition * 1.00) / total)),
                Executive = executive,
                AVGExecutive = ReturnValidValue(total, ((executive * 1.00) / total)),
                Staff = staff,
                AVGStaff = ReturnValidValue(total, ((staff * 1.00) / total)),
                Worker = worker,
                AVGWorker = ReturnValidValue(total, ((worker * 1.00) / total))
            };

            writer.WriteRow(RowFooter, record);
        }

        private static double ReturnValidValue(int condition, double value)
        {
            const double defaultValue = 0.00;
            if (condition != 0)
            {
                return value;
            }

            return defaultValue;
        }
    }
}