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
        void Export(IList<LeaveSummary> leaveSummaries, int year, string leaveType);
    }

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    public class ExportExcelUnpaidLeaveSummaryReportService : IExportExcelUnpaidLeaveSummaryReportService
    {
        private readonly ICommonDataService commonDataService;

        private const int RowStartHeader = 2;
        private const int RowStartBody = 4;
        private const int RowHighlight = 6;
        private const string UnpaidLeaveSummaryTemplatePath = @"~\ExcelTemplates\UnpaidLeaveSummaryTemplate.xls";

        public ExportExcelUnpaidLeaveSummaryReportService(ICommonDataService commonDataService)
        {
            this.commonDataService = commonDataService;
        }

        public void Export(IList<LeaveSummary> leaveSummaries, int year, string leaveType)
        {
            string templatePath = HttpContext.Current.Server.MapPath(UnpaidLeaveSummaryTemplatePath);
            string fileName = "UnpaidLeaveSummary_" + DateTime.Now.ToString("yyyyMMdd");

            string[] leaveTypeIds = !string.IsNullOrWhiteSpace(leaveType) ? leaveType.Split(',') : new string[] { };
            IList<LeaveType> leaveTypes = commonDataService.GetLeaveType();
            string type = string.Join(", ", leaveTypes.Where(x => leaveTypeIds.Any(y => x.Id.ToString() == y)).Select(x => x.Name));

            var writer = new ExcelTemplateWriter(templatePath, fileName, "Unpaid Leave Summary Report");

            /// Fill report title.
            writer.CopyRows(0, 2);

            /// Fill report header.
            writer.CopyRows(RowStartHeader, 2);
            writer.WriteCellRange(2, 1, 6, 12, $"Take {type} {year}");
            writer.WriteCellRange(2, 1, 18, 1, $"Total Of {type} {year}");

            FillContentRowReport(leaveSummaries, ref writer);

            writer.SyncColumnsWidth(19);
            writer.CreateFreezePane(0, RowStartBody);

            writer.Render();
        }

        private void FillContentRowReport(IList<LeaveSummary> leaveSummaries, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 0;
            foreach (LeaveSummary leaveSummary in leaveSummaries)
            {
                FillRowReport(leaveSummary, ref writer, ref rowIndex);
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
                Count = rowIndex + 1,
                leaveSummary.Department,
                leaveSummary.Section,
                leaveSummary.Team,
                leaveSummary.FullName,
                leaveSummary.Nickname,
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