namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Report.MonthlyWorkingHours;
    using OfficeOpenXml;

    public partial interface IEmployeeWorkingHoursService
    {
        IEnumerable<MonthlyWorking> GetMonthlyWorkingHours(GetMonthlyWorkingHoursParam param);

        byte[] ExportMonthlyWorkingHours(DateTime fromDate, DateTime toDate, IEnumerable<MonthlyWorking> monthlyWorkings);
    }

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    public partial class EmployeeWorkingHoursService : IEmployeeWorkingHoursService
    {
        private const ushort TotalFixedColumns = 14;
        private const ushort TotalUPLColumns = 4;
        private const ushort StartDateColumnIndex = 19;
        private const ushort StartUPLColumnIndex = 15;
        private const ushort EndUPLColumnIndex = 18;
        private const ushort StartHeaderRowIndex = 2;
        private const ushort EndHeaderRowIndex = 3;
        private const ushort StartBodyRowIndex = 4;

        private IEnumerable<LeaveType> LeaveTypes { get; set; }

        public IEnumerable<MonthlyWorking> GetMonthlyWorkingHours(GetMonthlyWorkingHoursParam param)
        {
            var criteria = new GetMonthlyWorkingHoursCriteria
            {
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                GroupIds = param.SectionIds,
                EmployeeId = param.EmployeeId,
                IsLocal = param.IsLocal
            };

            return repository.GetMonthlyWorkingHours(criteria);
        }

        public byte[] ExportMonthlyWorkingHours(DateTime fromDate, DateTime toDate, IEnumerable<MonthlyWorking> monthlyWorkings)
        {
            var templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/MonthlyWorkingHours.xlsx");
            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    LeaveTypes = leaveTypeService.GetAll();

                    ExcelWorksheet template = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Attendance Monthly Detail");
                    WriteDetailsSheet(template, worksheet, monthlyWorkings, fromDate, toDate);

                    const byte SummarySheetIndex = 2;
                    template = templatePackage.Workbook.Worksheets[SummarySheetIndex];
                    worksheet = xlPackage.Workbook.Worksheets.Add("Summary");
                    WriteSummarySheet(template, worksheet, monthlyWorkings, fromDate, toDate);

                    return xlPackage.GetAsByteArray();
                }
            }
        }

        private void WriteDetailsSheet(
            ExcelWorksheet template,
            ExcelWorksheet worksheet,
            IEnumerable<MonthlyWorking> monthlyWorkings,
            DateTime fromDate,
            DateTime toDate)
        {
            IEnumerable<DateTime> months = GetMonthsBetween(fromDate, toDate);
            var totalDays = (int)(toDate - fromDate).TotalDays;
            TotalColumns = TotalFixedColumns + months.Count() * TotalUPLColumns + (totalDays > 0 ? totalDays + 1 : 0);

            // title
            template.Cells[1, 1, 1, TotalFixedColumns].Copy(worksheet.Cells[1, 1, 1, TotalFixedColumns]);
            worksheet.Cells[1, 1].Value = string.Format(
                worksheet.Cells[1, 1].Value.ToString(),
                fromDate.ToString(ShortDateFormat),
                toDate.ToString(ShortDateFormat));

            // header
            WriteHeader(template, worksheet, fromDate, toDate);

            // data
            WriteBody(template, worksheet, monthlyWorkings, fromDate, toDate);

            worksheet.Cells.AutoFitColumns();
            worksheet.View.FreezePanes(StartBodyRowIndex, 6);
        }

        private void WriteHeader(ExcelWorksheet template, ExcelWorksheet worksheet, DateTime fromDate, DateTime toDate)
        {
            template.Cells[StartHeaderRowIndex, 1, EndHeaderRowIndex, TotalFixedColumns]
                .Copy(worksheet.Cells[StartHeaderRowIndex, 1, EndHeaderRowIndex, TotalFixedColumns]);

            IEnumerable<DateTime> months = GetMonthsBetween(fromDate, toDate);
            var colIndex = (int)StartUPLColumnIndex;
            foreach (DateTime month in months)
            {
                template.Cells[StartHeaderRowIndex, StartUPLColumnIndex, EndHeaderRowIndex, EndUPLColumnIndex]
                    .Copy(worksheet.Cells[StartHeaderRowIndex, colIndex, EndHeaderRowIndex, colIndex + TotalUPLColumns - 1]);
                worksheet.Cells[StartHeaderRowIndex, colIndex].Value = $"Unpaid Leaves {month.ToString("MMM-yyyy")}";

                colIndex = colIndex + TotalUPLColumns;
            }

            for (var day = fromDate.Date; day.Date <= toDate.Date; day = day.AddDays(1))
            {
                template.Cells[StartHeaderRowIndex, StartDateColumnIndex, EndHeaderRowIndex, StartDateColumnIndex]
                    .Copy(worksheet.Cells[StartHeaderRowIndex, colIndex, EndHeaderRowIndex, colIndex]);
                worksheet.Cells[StartHeaderRowIndex, colIndex].Value = day.ToString("dd MMM, yyyy (ddd)");

                if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                {
                    worksheet.Cells[StartHeaderRowIndex, colIndex].Style.Font.Color.SetColor(RedColor);
                }

                colIndex++;
            }
        }

        private void WriteBody(
            ExcelWorksheet template,
            ExcelWorksheet worksheet,
            IEnumerable<MonthlyWorking> monthlyWorkings,
            DateTime fromDate,
            DateTime toDate)
        {
            var rowIndex = StartBodyRowIndex;
            var rowNum = 1;
            foreach (MonthlyWorking monthlyWorking in monthlyWorkings)
            {
                var colIndex = 1;
                template.Cells[StartBodyRowIndex, 1, StartBodyRowIndex, TotalFixedColumns]
                    .Copy(worksheet.Cells[rowIndex, 1, rowIndex, TotalFixedColumns]);

                worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
                worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Code;
                worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Section.Name;
                worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.FullName;
                worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Nickname;
                worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.WorkingDays;
                worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.ActualWorkingDays;

                IEnumerable<DateTime> months = GetMonthsBetween(fromDate, toDate);
                var dailyColumnIndex = TotalFixedColumns + months.Count() * TotalUPLColumns + 1;
                const string CountLeaveFormula = "COUNTIF({0},\"{1}\")+(COUNTIF({0},\"*{1}½*\")/2)";

                var leaveCode = LeaveTypes.First(x => x.Id == (int)Core.Enums.LeaveTypes.MaternityLeave).Code;
                var excelAddress = $"{worksheet.Cells[rowIndex, dailyColumnIndex].Address}:{worksheet.Cells[rowIndex, TotalColumns].Address}";
                worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                // payment leaves
                var annualLeaveColumnIndex = colIndex;
                leaveCode = LeaveTypes.First(x => x.Id == (int)Core.Enums.LeaveTypes.AnnualLeave).Code;
                excelAddress = $"{worksheet.Cells[rowIndex, dailyColumnIndex].Address}:{worksheet.Cells[rowIndex, TotalColumns].Address}";
                worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                leaveCode = LeaveTypes.First(x => x.Id == (int)Core.Enums.LeaveTypes.BusinessTrip).Code;
                excelAddress = $"{worksheet.Cells[rowIndex, dailyColumnIndex].Address}:{worksheet.Cells[rowIndex, TotalColumns].Address}";
                worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                leaveCode = LeaveTypes.First(x => x.Id == (int)Core.Enums.LeaveTypes.BereavementLeave).Code;
                excelAddress = $"{worksheet.Cells[rowIndex, dailyColumnIndex].Address}:{worksheet.Cells[rowIndex, TotalColumns].Address}";
                worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                leaveCode = LeaveTypes.First(x => x.Id == (int)Core.Enums.LeaveTypes.CompensationLeave).Code;
                excelAddress = $"{worksheet.Cells[rowIndex, dailyColumnIndex].Address}:{worksheet.Cells[rowIndex, TotalColumns].Address}";
                worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                leaveCode = LeaveTypes.First(x => x.Id == (int)Core.Enums.LeaveTypes.WeddingLeave).Code;
                excelAddress = $"{worksheet.Cells[rowIndex, dailyColumnIndex].Address}:{worksheet.Cells[rowIndex, TotalColumns].Address}";
                worksheet.Cells[rowIndex, colIndex].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                excelAddress = $"{worksheet.Cells[rowIndex, annualLeaveColumnIndex].Address}:{worksheet.Cells[rowIndex, colIndex++].Address}";
                worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(Constants.SummaryFormula, excelAddress);

                // unpaid leaves
                var startColumnIndex = dailyColumnIndex;
                foreach (DateTime month in months)
                {
                    template.Cells[StartBodyRowIndex, StartUPLColumnIndex, StartBodyRowIndex, EndUPLColumnIndex]
                        .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + TotalUPLColumns - 1]);
                    var endColumnIndex = startColumnIndex +
                        monthlyWorking.DailyWorkings.Count(x => x.WorkDate.Year == month.Year && x.WorkDate.Month == month.Month) - 1;
                    excelAddress = $"{worksheet.Cells[rowIndex, startColumnIndex].Address}:{worksheet.Cells[rowIndex, endColumnIndex].Address}";

                    var unpaidLeaveColumnIndex = colIndex;
                    leaveCode = LeaveTypes.First(x => x.Id == (int)Core.Enums.LeaveTypes.UnpaidLeave).Code;
                    worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                    leaveCode = LeaveTypes.First(x => x.Id == (int)Core.Enums.LeaveTypes.UnpaidLeaveInsurance).Code;
                    worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                    leaveCode = LeaveTypes.First(x => x.Id == (int)Core.Enums.LeaveTypes.PaternityLeave).Code;
                    worksheet.Cells[rowIndex, colIndex].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                    excelAddress = $"{worksheet.Cells[rowIndex, unpaidLeaveColumnIndex].Address}:{worksheet.Cells[rowIndex, colIndex++].Address}";
                    worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(Constants.SummaryFormula, excelAddress);

                    startColumnIndex = endColumnIndex + 1;
                }

                foreach (DailyWorking dailyWorking in monthlyWorking.DailyWorkings)
                {
                    template.Cells[StartBodyRowIndex, StartDateColumnIndex].Copy(worksheet.Cells[rowIndex, colIndex]);
                    WriteDailyCells(worksheet.Cells[rowIndex, colIndex++], dailyWorking);
                }

                rowIndex++;
            }
        }

        private void WriteSummarySheet(
            ExcelWorksheet template,
            ExcelWorksheet worksheet,
            IEnumerable<MonthlyWorking> monthlyWorkings,
            DateTime fromDate,
            DateTime toDate)
        {
            const byte DefaultColumns = 8;
            var templateIndex = new
            {
                headerRow = 2,
                bodyRow = 3,
                summaryRow = 4,
                annualLeaveColumn = 3,
                unpaidLeaveColumn = 4,
                weddingLeaveColumn = 5,
                compensationLeaveColumn = 9
            };
            IEnumerable<DateTime> months = GetMonthsBetween(fromDate, toDate);
            TotalColumns = DefaultColumns + months.Count();
            var rowIndex = 1;
            var colIndex = 1;

            // title
            template.Cells[1, 1, 1, DefaultColumns].Copy(worksheet.Cells[1, 1, 1, DefaultColumns]);
            worksheet.Cells[1, 1].Value = string.Format(
                worksheet.Cells[1, 1].Value.ToString(),
                fromDate.ToString(ShortDateFormat),
                toDate.ToString(ShortDateFormat));

            // header
            WriteHeaderSummary(template, worksheet, months, templateIndex);

            // data
            WriteBodySummary(template, worksheet, monthlyWorkings, months, ref rowIndex, templateIndex);

            // summary
            colIndex = 1;
            template.Cells[templateIndex.summaryRow, colIndex].Copy(worksheet.Cells[rowIndex, colIndex++]);
            for (var i = 2; i <= TotalColumns; i++)
            {
                template.Cells[templateIndex.summaryRow, templateIndex.annualLeaveColumn].Copy(worksheet.Cells[rowIndex, colIndex]);
                var excelAddress = $"{worksheet.Cells[templateIndex.bodyRow, colIndex].Address}:{worksheet.Cells[rowIndex - 1, colIndex].Address}";
                worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(Constants.SummaryFormula, excelAddress);
            }

            worksheet.Cells.AutoFitColumns();
            worksheet.View.FreezePanes(templateIndex.bodyRow, 1);
        }

        private void WriteHeaderSummary(
            ExcelWorksheet template,
            ExcelWorksheet worksheet,
            IEnumerable<DateTime> months,
            dynamic templateIndex)
        {
            var rowIndex = templateIndex.headerRow;
            var colIndex = templateIndex.annualLeaveColumn;
            template.Cells[rowIndex, 1, rowIndex, colIndex].Copy(worksheet.Cells[rowIndex, 1, rowIndex, colIndex]);
            colIndex++;
            foreach (DateTime month in months)
            {
                template.Cells[rowIndex, templateIndex.unpaidLeaveColumn].Copy(worksheet.Cells[rowIndex, colIndex]);
                worksheet.Cells[rowIndex, colIndex].Value = string.Format(worksheet.Cells[rowIndex, colIndex].Value.ToString(), month.ToString("MMM-yyyy"));
                colIndex++;
            }

            template.Cells[rowIndex, templateIndex.weddingLeaveColumn, rowIndex, templateIndex.compensationLeaveColumn]
                .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
        }

        private void WriteBodySummary(
            ExcelWorksheet template,
            ExcelWorksheet worksheet,
            IEnumerable<MonthlyWorking> monthlyWorkings,
            IEnumerable<DateTime> months,
            ref int rowIndex,
            dynamic templateIndex)
        {
            rowIndex = templateIndex.bodyRow;
            var sections = monthlyWorkings.GroupBy(x => x.Section.Name)
                .Select(x => new { Section = x.Key, Members = x.ToList() })
                .OrderBy(x => x.Section);
            foreach (var section in sections)
            {
                var colIndex = 1;
                template.Cells[templateIndex.bodyRow, 1, templateIndex.bodyRow, templateIndex.annualLeaveColumn]
                    .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, templateIndex.annualLeaveColumn]);

                worksheet.Cells[rowIndex, colIndex++].Value = section.Section;
                worksheet.Cells[rowIndex, colIndex++].Value = section.Members.Sum(x => x.ActualWorkingDays);
                worksheet.Cells[rowIndex, colIndex++].Value =
                    section.Members.Sum(x => SummaryLeaveDay(x.DailyWorkings, Core.Enums.LeaveTypes.AnnualLeave));

                foreach (DateTime month in months)
                {
                    template.Cells[templateIndex.bodyRow, templateIndex.unpaidLeaveColumn].Copy(worksheet.Cells[rowIndex, colIndex]);
                    worksheet.Cells[rowIndex, colIndex++].Value =
                        section.Members.Sum(x => SummaryLeaveDay(
                            x.DailyWorkings.Where(daily => daily.WorkDate.Month == month.Month && daily.WorkDate.Year == month.Year),
                            Core.Enums.LeaveTypes.UnpaidLeave))
                        + section.Members.Sum(x => SummaryLeaveDay(
                            x.DailyWorkings.Where(daily => daily.WorkDate.Month == month.Month && daily.WorkDate.Year == month.Year),
                            Core.Enums.LeaveTypes.UnpaidLeaveInsurance))
                        + section.Members.Sum(x => SummaryLeaveDay(
                            x.DailyWorkings.Where(daily => daily.WorkDate.Month == month.Month && daily.WorkDate.Year == month.Year),
                            Core.Enums.LeaveTypes.PaternityLeave));
                }

                template.Cells[templateIndex.bodyRow, templateIndex.weddingLeaveColumn, templateIndex.bodyRow, templateIndex.compensationLeaveColumn]
                    .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                worksheet.Cells[rowIndex, colIndex++].Value =
                    section.Members.Sum(x => SummaryLeaveDay(x.DailyWorkings, Core.Enums.LeaveTypes.WeddingLeave));
                worksheet.Cells[rowIndex, colIndex++].Value =
                    section.Members.Sum(x => SummaryLeaveDay(x.DailyWorkings, Core.Enums.LeaveTypes.MaternityLeave));
                worksheet.Cells[rowIndex, colIndex++].Value =
                    section.Members.Sum(x => SummaryLeaveDay(x.DailyWorkings, Core.Enums.LeaveTypes.BusinessTrip));
                worksheet.Cells[rowIndex, colIndex++].Value =
                    section.Members.Sum(x => SummaryLeaveDay(x.DailyWorkings, Core.Enums.LeaveTypes.BereavementLeave));
                worksheet.Cells[rowIndex, colIndex].Value =
                    section.Members.Sum(x => SummaryLeaveDay(x.DailyWorkings, Core.Enums.LeaveTypes.CompensationLeave));

                rowIndex++;
            }
        }

        private static IEnumerable<DateTime> GetMonthsBetween(DateTime fromDate, DateTime toDate)
        {
            DateTime iterator;
            DateTime limit;
            IList<DateTime> months = new List<DateTime>();

            if (toDate >= fromDate)
            {
                iterator = new DateTime(fromDate.Year, fromDate.Month, 1);
                limit = toDate;

                while (iterator <= limit)
                {
                    months.Add(new DateTime(iterator.Year, iterator.Month, 1));

                    iterator = iterator.AddMonths(1);
                }
            }

            return months;
        }

        private static decimal SummaryLeaveDay(IEnumerable<DailyWorking> dailyWorkings, Core.Enums.LeaveTypes leaveType)
        {
            decimal totals = 0;
            foreach (DailyWorking dailyWorking in dailyWorkings)
            {
                if (dailyWorking.IsLeaveDay
                    && (dailyWorking.LeaveType.Id == (int)leaveType
                        || (dailyWorking.LeaveType2 != null && dailyWorking.LeaveType2.Id == (int)leaveType)))
                {
                    if (dailyWorking.IsLeaveHalfDay
                        || (dailyWorking.LeaveType2 != null && dailyWorking.LeaveType.Id != dailyWorking.LeaveType2.Id))
                    {
                        totals += Constants.HalfDayOffValue;
                    }
                    else
                    {
                        totals += 1;
                    }
                }
            }

            return totals;
        }
    }
}