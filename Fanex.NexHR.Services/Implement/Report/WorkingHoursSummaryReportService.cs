namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models.Report;
    using OfficeOpenXml;

    public interface IWorkingHoursSummaryReportService
    {
        IEnumerable<WorkingHoursSummary> GetWorkingHoursSummary(GetWorkingHoursSummaryParam param);

        byte[] Export(GetWorkingHoursSummaryParam param, IEnumerable<WorkingHoursSummary> workingHours);
    }

    public class WorkingHoursSummaryReportService : IWorkingHoursSummaryReportService
    {
        private const byte MinimumWidth = 5;
        private const byte MaximumWidth = 50;
        private readonly IBaseService baseService;

        public WorkingHoursSummaryReportService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IEnumerable<WorkingHoursSummary> GetWorkingHoursSummary(GetWorkingHoursSummaryParam param)
        {
            var criteria = new GetWorkingHoursSummaryReportCriteria
            {
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                IsLocal = param.IsLocal,
                GroupIds = param.SectionIds,
                EmployeeId = param.EmployeeId
            };
            return baseService.GetData<WorkingHoursSummary>(criteria);
        }

        public byte[] Export(GetWorkingHoursSummaryParam param, IEnumerable<WorkingHoursSummary> workingHours)
        {
            var templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/WorkingHoursSummary.xlsx");
            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    const byte TotalColumns = 26;
                    const byte TotalHeaderRows = 2;
                    const byte StartBodyRow = 4;
                    var rowIndex = 1;
                    var colIndex = 1;
                    ExcelWorksheet template = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Total Working Hours");

                    // title
                    template.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                    worksheet.Cells[rowIndex, colIndex].Value = string.Format(
                        worksheet.Cells[rowIndex, colIndex].Value.ToString(),
                        param.FromDate.ToDateMonthYearFormatedDate(),
                        param.ToDate.ToDateMonthYearFormatedDate());
                    rowIndex++;

                    // header
                    template.Cells[rowIndex, colIndex, rowIndex + 1, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex + 1, TotalColumns]);
                    rowIndex += TotalHeaderRows;

                    // data
                    var rowNum = 1;
                    foreach (WorkingHoursSummary item in workingHours)
                    {
                        colIndex = 1;
                        template.Cells[StartBodyRow, 1, StartBodyRow, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

                        worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
                        worksheet.Cells[rowIndex, colIndex++].Value = item.Code;
                        worksheet.Cells[rowIndex, colIndex++].Value = item.FullName;
                        worksheet.Cells[rowIndex, colIndex++].Value = item.Nickname;
                        worksheet.Cells[rowIndex, colIndex++].Value = item.Section;
                        worksheet.Cells[rowIndex, colIndex++].Value = item.WorkingDays;
                        worksheet.Cells[rowIndex, colIndex++].Value = item.WorkingHours;
                        worksheet.Cells[rowIndex, colIndex++].Value = item.TotalWorkingHours;
                        worksheet.Cells[rowIndex, colIndex++].Value = item.ActualWorkingDays;

                        worksheet.Cells[rowIndex, colIndex++].Value = item.PaymentWorkingDays;
                        worksheet.Cells[rowIndex, colIndex++].Value = item.AnnualLeave;
                        worksheet.Cells[rowIndex, colIndex++].Value = item.BusinessTrip;
                        worksheet.Cells[rowIndex, colIndex++].Value = item.BereavementLeave;
                        worksheet.Cells[rowIndex, colIndex++].Value = item.CompensationLeave;
                        worksheet.Cells[rowIndex, colIndex++].Value = item.WeddingLeave;
                        worksheet.Cells[rowIndex, colIndex++].Value = item.TotalPayments;

                        worksheet.Cells[rowIndex, colIndex++].Value = item.UnpaidLeave;
                        worksheet.Cells[rowIndex, colIndex++].Value = item.UnpaidLeaveInsurance;
                        worksheet.Cells[rowIndex, colIndex++].Value = item.PaternityLeave;
                        worksheet.Cells[rowIndex, colIndex++].Value = item.MaternityLeave;
                        worksheet.Cells[rowIndex, colIndex++].Value = item.TotalUnpaidLeaves;

                        worksheet.Cells[rowIndex, colIndex++].Value = item.NormalOvertimeHours;
                        worksheet.Cells[rowIndex, colIndex++].Value = item.OffOvertimeHours;
                        worksheet.Cells[rowIndex, colIndex++].Value = item.PublicOvertimeHours;
                        worksheet.Cells[rowIndex, colIndex++].Value = 0;
                        worksheet.Cells[rowIndex, colIndex].Value = item.TotalOvertimes;

                        rowIndex++;
                    }

                    const byte FreezeColumnIndex = 6;
                    worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
                    worksheet.View.FreezePanes(StartBodyRow, FreezeColumnIndex);
                    return xlPackage.GetAsByteArray();
                }
            }
        }
    }
}