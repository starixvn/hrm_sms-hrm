namespace Fanex.NexHR.Services.ExportExcel.WeekendLeave
{
    using System.Collections.Generic;
    using Criteria.Report;
    using Models.Report;
    using System.Web;
    using Resources;
    using Core;
    using Interfaces;

    public class ExportWeekendLeaveSummaryService : IExportWeekendLeaveSummaryService
    {
        private const string WeekendLeaveSummaryTemplatePath = @"~\ExcelTemplates\WeekendLeaveSummaryTemplate.xls";
        private const int HeaderRow = 3;
        private const int BodyRow = 4;
        private const int TotalCol = 7;

        public void Export(GetWeekendLeaveSummaryCriteria criteria, IList<WeekendLeaveSummary> weekendLeaves)
        {
            string templatePath = HttpContext.Current.Server.MapPath(WeekendLeaveSummaryTemplatePath);
            string reportName = Labels.WeekendLeaveSummary;
            string reportTitle;
            string startDate = criteria.FromDate.ToString(Constants.DateFormat);
            string endDate = criteria.ToDate.ToString(Constants.DateFormat);

            if (criteria.EmployeeId != null)
            {
                reportTitle = string.Format(Labels.WeekendLeaveSummary, criteria.EmployeeName, startDate, endDate);
            }
            else if (criteria.GroupId != null)
            {
                reportTitle = string.Format(Labels.WeekendLeaveSummary, criteria.GroupName, startDate, endDate);
            }
            else
            {
                reportTitle = string.Format(Labels.WeekendLeaveSummary, startDate, endDate);
            }

            var writer = new ExcelTemplateWriter(templatePath, reportName, Labels.WeekendLeave);

            FillHeaderRows(ref writer, criteria, reportTitle);

            FillContentRow(weekendLeaves, ref writer);
            writer.SyncColumnsWidth(TotalCol);
            writer.CreateFreezePane(6, 3);

            writer.Render();
        }

        private void FillHeaderRows(ref ExcelTemplateWriter writer, GetWeekendLeaveSummaryCriteria criteria, string reportTitle)
        {
            var dateRange = new
            {
                string.Empty,
                FromDateLabel = Labels.FromDate,
                FromDate = criteria.FromDate.ToDateMonthYearFormatedDate(),
                ToDateLable = Labels.ToDate,
                ToDate = criteria.ToDate.ToDateMonthYearFormatedDate()
            };

            writer.WriteRow(0, new object[] { reportTitle });
            writer.WriteRow(1, dateRange);
            writer.CopyRows(1, 1);

            writer.CopyRows(HeaderRow, 1);
        }

        private void FillContentRow(IList<WeekendLeaveSummary> weekendLeaves, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (WeekendLeaveSummary item in weekendLeaves)
            {
                FillRow(item, ref writer, ref rowIndex);
            }
        }

        private void FillRow(WeekendLeaveSummary weekendLeave, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecord(weekendLeave, ref rowIndex);
            writer.WriteRow(BodyRow, record);
        }

        private object CreateRecord(WeekendLeaveSummary weekendLeave, ref int rowIndex)
        {
            double totalWeekendLeaveDays = weekendLeave.TotalWeekendLeave > 0 ? weekendLeave.TotalWeekendLeave : 0;
            var record = new
            {
                Count = rowIndex,
                Code = weekendLeave.Code,
                FullName = weekendLeave.FullName,
                Nick = weekendLeave.NickName,
                Section = weekendLeave.GroupName,
                Category = weekendLeave.IsLocal ? Labels.Local : Labels.Expat,
                WeekendLeave = totalWeekendLeaveDays
            };
            rowIndex++;

            return record;
        }
    }
}