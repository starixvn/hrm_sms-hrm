namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Core.Enums;
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
        private const ushort TotalFixedColumns = 13;
        private const ushort TotalUPLColumns = 5;
        private const ushort StartDateColumnIndex = 19;
        private const ushort StartUPLColumnIndex = 14;
        private const ushort EndUPLColumnIndex = 18;
        private const ushort StartHeaderRowIndex = 2;
        private const ushort EndHeaderRowIndex = 3;
        private const ushort StartBodyRowIndex = 4;

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
            IEnumerable<DateTime> months = GetMonthsBetween(fromDate, toDate);
            var totalDays = (int)(toDate - fromDate).TotalDays;
            TotalColumns = TotalFixedColumns + months.Count() * TotalUPLColumns + (totalDays > 0 ? totalDays + 1 : 0);

            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    const string ShortDateFormat = "dd MMM yyyy";
                    ExcelWorksheet template = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Attendance Monthly Details");

                    // title
                    template.Cells[1, 1, 1, TotalFixedColumns].Copy(worksheet.Cells[1, 1, 1, TotalFixedColumns]);
                    worksheet.Cells[1, 1].Value = $"Attendance Monthly Details {fromDate.ToString(ShortDateFormat)} - {toDate.ToString(ShortDateFormat)}";

                    // header
                    WriteHeader(template, worksheet, fromDate, toDate);

                    // data
                    WriteBody(template, worksheet, monthlyWorkings, fromDate, toDate);

                    worksheet.Cells.AutoFitColumns();
                    worksheet.View.FreezePanes(StartBodyRowIndex, 6);

                    return xlPackage.GetAsByteArray();
                }
            }
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
            IList<LeaveType> leaveTypes = leaveTypeService.GetAll();
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
                // payment leaves
                var annualLeaveColumnIndex = colIndex;
                var leaveCode = leaveTypes.First(x => x.Id == (int)LeaveTypes.AnnualLeave).Code;
                var excelAddress = $"{worksheet.Cells[rowIndex, dailyColumnIndex].Address}:{worksheet.Cells[rowIndex, TotalColumns].Address}";
                worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                leaveCode = leaveTypes.First(x => x.Id == (int)LeaveTypes.BusinessTrip).Code;
                excelAddress = $"{worksheet.Cells[rowIndex, dailyColumnIndex].Address}:{worksheet.Cells[rowIndex, TotalColumns].Address}";
                worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                leaveCode = leaveTypes.First(x => x.Id == (int)LeaveTypes.BereavementLeave).Code;
                excelAddress = $"{worksheet.Cells[rowIndex, dailyColumnIndex].Address}:{worksheet.Cells[rowIndex, TotalColumns].Address}";
                worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                leaveCode = leaveTypes.First(x => x.Id == (int)LeaveTypes.CompensationLeave).Code;
                excelAddress = $"{worksheet.Cells[rowIndex, dailyColumnIndex].Address}:{worksheet.Cells[rowIndex, TotalColumns].Address}";
                worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                leaveCode = leaveTypes.First(x => x.Id == (int)LeaveTypes.WeddingLeave).Code;
                excelAddress = $"{worksheet.Cells[rowIndex, dailyColumnIndex].Address}:{worksheet.Cells[rowIndex, TotalColumns].Address}";
                worksheet.Cells[rowIndex, colIndex].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                excelAddress = $"{worksheet.Cells[rowIndex, annualLeaveColumnIndex].Address}:{worksheet.Cells[rowIndex, colIndex++].Address}";
                worksheet.Cells[rowIndex, colIndex++].Formula = string.Format("SUM({0})", excelAddress);

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
                    leaveCode = leaveTypes.First(x => x.Id == (int)LeaveTypes.UnpaidLeave).Code;
                    worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                    leaveCode = leaveTypes.First(x => x.Id == (int)LeaveTypes.UnpaidLeaveInsurance).Code;
                    worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                    leaveCode = leaveTypes.First(x => x.Id == (int)LeaveTypes.PaternityLeave).Code;
                    worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                    leaveCode = leaveTypes.First(x => x.Id == (int)LeaveTypes.MaternityLeave).Code;
                    worksheet.Cells[rowIndex, colIndex].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                    excelAddress = $"{worksheet.Cells[rowIndex, unpaidLeaveColumnIndex].Address}:{worksheet.Cells[rowIndex, colIndex++].Address}";
                    worksheet.Cells[rowIndex, colIndex++].Formula = string.Format("SUM({0})", excelAddress);

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
    }
}