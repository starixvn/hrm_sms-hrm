namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models;

    public interface IExportExcelUnpaidLeaveSummaryReportService
    {
        void Export(IList<LeaveSummaryByTeam> leaveSummaries, int year, string leaveType);
    }

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    public class ExportExcelUnpaidLeaveSummaryReportService : IExportExcelUnpaidLeaveSummaryReportService
    {
        private const int RowStartHeader = 2;
        private const int RowStartBody = 4;
        private const int RowHighlight = 6;
        private const string UnpaidLeaveSummaryTemplatePath = @"~\ExcelTemplates\UnpaidLeaveSummaryTemplate.xls";

        public void Export(IList<LeaveSummaryByTeam> leaveSummaries, int year, string leaveType)
        {
            string templatePath = HttpContext.Current.Server.MapPath(UnpaidLeaveSummaryTemplatePath);
            string fileName = "UnpaidLeaveSummary_" + DateTime.Now.ToString("yyyyMMdd");
            string type = leaveType != null ? leaveType.Replace("25", "Unpaid Leave").Replace("12", "Maternity Leave")
                .Replace("14", "Paternity Leave").Replace("41", "Unpaid Leave (Insurance)") : "";

            var writer = new ExcelTemplateWriter(templatePath, fileName, "Unpaid Leave Summary Report");

            /// Fill report title.
            writer.CopyRows(0, 2);

            /// Fill report header.
            writer.CopyRows(RowStartHeader, 2);
            writer.WriteCellRange(2, 1, 3, 12, $"Take {type} {year}");
            writer.WriteCellRange(2, 1, 15, 1, $"Total Of {type} {year}");

            FillContentRowReport(leaveSummaries, ref writer);

            writer.SyncColumnsWidth(16);
            writer.CreateFreezePane(0, RowStartBody);

            writer.Render();
        }

        private void FillContentRowReport(IList<LeaveSummaryByTeam> leaveSummaries, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (LeaveSummaryByTeam leaveSummary in leaveSummaries.Where(x => x.LeaveSummaryInTeam != null && x.LeaveSummaryInTeam.Any()))
            {
                writer.WriteRow(RowHighlight, new object[] { leaveSummary.Team.ToUpperInvariant() });

                foreach (LeaveSummary item in leaveSummary.LeaveSummaryInTeam)
                {
                    FillRowReport(item, ref writer, ref rowIndex);
                }
            }
        }

        private void FillRowReport(LeaveSummary leaveSummary, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordReport(leaveSummary, rowIndex);
            ExportExcelService.WriteRowExcelReport(writer, rowIndex, RowStartBody, record);
            rowIndex++;
        }

        private object CreateRecordReport(LeaveSummary leaveSummary, int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                leaveSummary.Nickname,
                leaveSummary.FullName,
                Jan = leaveSummary.Jan * 1.00,
                Feb = leaveSummary.Feb * 1.00,
                Mar = leaveSummary.Mar * 1.00,
                Apr = leaveSummary.Apr * 1.00,
                May = leaveSummary.May * 1.00,
                Jun = leaveSummary.Jun * 1.00,
                Jul = leaveSummary.Jul * 1.00,
                Aug = leaveSummary.Aug * 1.00,
                Sep = leaveSummary.Sep * 1.00,
                Oct = leaveSummary.Oct * 1.00,
                Nov = leaveSummary.Nov * 1.00,
                Dec = leaveSummary.Dec * 1.00,
                LeaveBalance = leaveSummary.LeaveBalance * 1.00
            };

            return record;
        }
    }
}