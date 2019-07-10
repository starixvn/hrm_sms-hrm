namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models.Report.Overtime;
    using OfficeOpenXml;

    public class OvertimeReportService : IOvertimeReportService
    {
        private readonly IBaseService baseService;

        public OvertimeReportService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IEnumerable<YearlyOvertime> GetYearlyOvertime(GetYearlyOvertimeParam param)
        {
            var criteria = new GetYearlyOvertimeReportCriteria
            {
                Year = param.Year,
                CalendarType = param.Calendar,
                GroupIds = param.SectionIds,
                EmployeeId = param.EmployeeId
            };
            return baseService.GetData<YearlyOvertime>(criteria);
        }

        public IEnumerable<DailyOvertimeDetails> GetMonthlyOvertimeDetails(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetMonthlyOvertimeDetailsReportCriteria
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate
            };
            return baseService.GetData<DailyOvertimeDetails>(criteria);
        }

        public byte[] ExportYearlyOvertime(IEnumerable<YearlyOvertime> overtimes, GetYearlyOvertimeParam param)
        {
            var templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/YearlyOvertime.xlsx");
            const ushort TotalColumns = 17;
            const ushort StartMonthColumn = 5;
            const ushort StartBodyRow = 4;
            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    ExcelWorksheet templateWorksheet = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Yearly Overtime Report");
                    var rowIndex = 1;
                    var colIndex = 1;

                    // title
                    templateWorksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                    rowIndex++;

                    // header
                    templateWorksheet.Cells[rowIndex, colIndex, rowIndex + 1, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex + 1, TotalColumns]);

                    var calendarType = param.Calendar == (byte)CalendarType.Fiscal ? "Fiscal Calendar" : "Normal Calendar";
                    worksheet.Cells[rowIndex, StartMonthColumn].Value = string.Format("Overtime Summary In {0} ({1})", param.Year, calendarType);
                    rowIndex += 2;

                    // data
                    foreach (YearlyOvertime overtime in overtimes)
                    {
                        colIndex = 1;
                        templateWorksheet.Cells[StartBodyRow, 1, StartBodyRow, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

                        worksheet.Cells[rowIndex, colIndex++].Value = rowIndex - 3;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.Section;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.FullName;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.Nickname;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.Jan;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.Feb;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.Mar;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.Apr;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.May;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.Jun;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.Jul;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.Aug;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.Sep;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.Oct;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.Nov;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.Dec;
                        worksheet.Cells[rowIndex, colIndex].Value = overtime.Total;

                        rowIndex++;
                    }

                    // summary
                    templateWorksheet.Cells[StartMonthColumn, 1, StartMonthColumn, TotalColumns].Copy(worksheet.Cells[rowIndex, 1, rowIndex, TotalColumns]);

                    for (var i = 0; i <= 12; i++)
                    {
                        colIndex = StartMonthColumn + i;
                        var excelAddress = $"{worksheet.Cells[StartBodyRow, colIndex].Address}:{worksheet.Cells[rowIndex - 1, colIndex].Address}";
                        worksheet.Cells[rowIndex, colIndex].Formula = string.Format("SUM({0})", excelAddress);
                    }

                    worksheet.Cells.AutoFitColumns();
                    worksheet.View.FreezePanes(StartBodyRow, 1);

                    return xlPackage.GetAsByteArray();
                }
            }
        }
    }
}