namespace Fanex.NexHR.Services.ExportExcel.JobTitle
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models;

    public class JobTitleExportService : IJobTitleExportService
    {
        private const string JobTitleTemplatePath = @"~\ExcelTemplates\JobTitleSummary.xls";
        private const int RowStartHead = 2;
        private const int RowStartBody = 3;
        private const int TotalColumns = 6;

        public void ExportJobTitle(IEnumerable<LookupData> jobtitles)
        {
            string templatePath = HttpContext.Current.Server.MapPath(JobTitleTemplatePath);
            string fileName = "Jobtitle_" + DateTime.Now.ToString("yyyyMMdd");
            const string sheetName = "Job title";
            var writer = new ExcelTemplateWriter(templatePath, fileName, sheetName);

            /// Fill report title.
            writer.CopyRows(0, 2);
            /// Fill report header.
            writer.CopyRows(RowStartHead, 1);

            FillContent(jobtitles, ref writer);
            writer.SyncColumnsWidth(TotalColumns);
            writer.CreateFreezePane(0, RowStartBody);

            writer.Render();
        }

        private void FillContent(IEnumerable<LookupData> jobtitles, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (LookupData jobtitle in jobtitles)
            {
                FillRow(jobtitle, ref writer, ref rowIndex);
            }
        }

        private void FillRow(LookupData jobtitle, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = new
            {
                Count = rowIndex,
                jobtitle.Name,
                jobtitle.CategoryName,
                jobtitle.TotalExpat,
                jobtitle.TotalLocal,
                jobtitle.Value
            };

            var writeRowNumber = (rowIndex % 2 == 0) ? 4 : 3;

            writer.WriteRow(writeRowNumber, record);

            rowIndex++;
        }
    }
}