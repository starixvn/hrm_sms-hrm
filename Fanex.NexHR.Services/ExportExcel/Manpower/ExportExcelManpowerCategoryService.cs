namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;

    public class ExportExcelManpowerCategoryService : IExportExcelManpowerCategoryService
    {
        private const int RowStartHeader = 2;
        private const int RowStartBody = 4;
        private const int RowFooter = 6;
        private const int TotalColumns = 10;
        private const string ManpowerCategoryTemplatePath = @"~\ExcelTemplates\ManpowerCategoryReportTemplate.xls";

        public void Export(IList<ManpowerCategoryModel> manpowers, int year)
        {
            string templatePath = HttpContext.Current.Server.MapPath(ManpowerCategoryTemplatePath);
            string fileName = "ManpowerCategoryReport_" + DateTime.Now.ToString("yyyyMMdd");
            string reportTitle = Labels.ManpowerCategoryReportTitle + " " + year;

            var writer = new ExcelTemplateWriter(templatePath, fileName, "Manpower Report");

            /// Fill report title.
            writer.WriteRow(0, new object[] { reportTitle });
            writer.CopyRows(1, 1);

            /// Fill report header.
            writer.CopyRows(RowStartHeader, 2);

            FillContentRowReport(manpowers, ref writer);
            FillContentRowFooterReport(manpowers, ref writer);
            writer.SyncColumnsWidth(TotalColumns);
            writer.CreateFreezePane(0, RowStartBody);

            writer.Render();
        }

        private void FillContentRowReport(IList<ManpowerCategoryModel> manpowers, ref ExcelTemplateWriter writer)
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

        private object CreateRecordReport(ManpowerCategoryModel manpower, int rowIndex)
        {
            double defaultValue = 0.00;
            var record = new
            {
                Count = rowIndex,
                GroupCode = manpower.GroupCode,
                TotalExpat = manpower.TotalExpat,
                TotalLocal = manpower.TotalLocal,
                Total = manpower.Total,
                Management = manpower.Management,
                AVGManagement = manpower.Total != 0 ? ((manpower.Management * 1.00) / manpower.Total) : defaultValue,
                ExecutiveWithPosition = manpower.ExecutiveWithPosition,
                AVGExecutiveWithPosition = manpower.Total != 0 ? ((manpower.ExecutiveWithPosition * 1.00) / manpower.Total) : defaultValue,
                Executive = manpower.Executive,
                AVGExecutive = manpower.Total != 0 ? ((manpower.Executive * 1.00) / manpower.Total) : defaultValue,
                Staff = manpower.Staff,
                AVGStaff = manpower.Total != 0 ? ((manpower.Staff * 1.00) / manpower.Total) : defaultValue,
                Worker = manpower.Worker,
                AVGWorker = manpower.Total != 0 ? ((manpower.Worker * 1.00) / manpower.Total) : defaultValue
            };

            return record;
        }

        private void FillContentRowFooterReport(IList<ManpowerCategoryModel> manpowers, ref ExcelTemplateWriter writer)
        {
            double defaultValue = 0.00;
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
                AVGManagement = total != 0 ? ((management * 1.00) / total) : defaultValue,
                ExecutiveWithPosition = executiveWithPosition,
                AVGExecutiveWithPosition = total != 0 ? ((executiveWithPosition * 1.00) / total) : defaultValue,
                Executive = executive,
                AVGExecutive = total != 0 ? ((executive * 1.00) / total) : defaultValue,
                Staff = staff,
                AVGStaff = total != 0 ? ((staff * 1.00) / total) : defaultValue,
                Worker = worker,
                AVGWorker = total != 0 ? ((worker * 1.00) / total) : defaultValue
            };

            writer.WriteRow(RowFooter, record);
        }
    }
}