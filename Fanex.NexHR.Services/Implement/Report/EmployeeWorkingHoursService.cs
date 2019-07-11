namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models.Report.MonthlyWorkingHours;
    using Fanex.NexHR.Repositories;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;

#pragma warning disable S109 // Magic numbers should not be used
    public class EmployeeWorkingHoursService : IEmployeeWorkingHoursService
#pragma warning disable S109 // Magic numbers should not be used
    {
        private readonly IEmployeeWorkingHoursRepository repository;

        public EmployeeWorkingHoursService(IEmployeeWorkingHoursRepository repository)
        {
            this.repository = repository;
        }

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
            const ushort StartDateColumn = 15;
            var totalColumns = StartDateColumn + (int)(toDate - fromDate).TotalDays;

            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    var redColor = ColorTranslator.FromHtml(Constants.RedColor);
                    const string ShortDateFormat = "dd MMM yyyy";
                    var sheetName = toDate.ToString("MMM yyyy");
                    ExcelWorksheet templateWorksheet = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add(sheetName);
                    var rowIndex = 1;
                    var colIndex = 1;

                    // title
                    templateWorksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);
                    worksheet.Cells[rowIndex, colIndex].Value = $"Employee's Monthly Working Hours {fromDate.ToString(ShortDateFormat)} - {toDate.ToString(ShortDateFormat)}";
                    rowIndex++;

                    // header
                    templateWorksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);
                    colIndex = StartDateColumn;
                    for (var day = fromDate.Date; day.Date <= toDate.Date; day = day.AddDays(1))
                    {
                        worksheet.Cells[rowIndex, colIndex].Value = day.ToString("dd MMM, yyyy (ddd)");

                        if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                        {
                            worksheet.Cells[rowIndex, colIndex].Style.Font.Color.SetColor(redColor);
                        }

                        colIndex++;
                    }

                    // data
                    rowIndex++;
                    foreach (MonthlyWorking monthlyWorking in monthlyWorkings)
                    {
                        colIndex = 1;
                        templateWorksheet.Cells[3, 1, 3, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);

                        worksheet.Cells[rowIndex, colIndex++].Value = rowIndex - 2;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Code;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Section.Name;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.FullName;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Nickname;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.WorkingDays;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.ActualWorkingDays;

                        var excelAddress = $"{worksheet.Cells[rowIndex, StartDateColumn].Address}:{worksheet.Cells[rowIndex, totalColumns].Address}";
                        worksheet.Cells[rowIndex, colIndex].Formula = string.Format("COUNTIF({0},\"AL\")+(COUNTIF({0},\"AL½\")/2)", excelAddress);

                        colIndex++;
                        excelAddress = $"{worksheet.Cells[rowIndex, StartDateColumn].Address}:{worksheet.Cells[rowIndex, totalColumns].Address}";
                        worksheet.Cells[rowIndex, colIndex].Formula =
                            string.Format("COUNTIF({0},\"UPL\")+(COUNTIF({0},\"UPL½\")/2)+COUNTIF({0},\"UPLI\")+(COUNTIF({0},\"UPLI½\")/2)+COUNTIF({0},\"PL\")", excelAddress);

                        colIndex++;
                        excelAddress = $"{worksheet.Cells[rowIndex, StartDateColumn].Address}:{worksheet.Cells[rowIndex, totalColumns].Address}";
                        worksheet.Cells[rowIndex, colIndex].Formula = string.Format("COUNTIF({0},\"WL\")", excelAddress);

                        colIndex++;
                        excelAddress = $"{worksheet.Cells[rowIndex, StartDateColumn].Address}:{worksheet.Cells[rowIndex, totalColumns].Address}";
                        worksheet.Cells[rowIndex, colIndex].Formula = string.Format("COUNTIF({0},\"ML\")", excelAddress);

                        colIndex++;
                        excelAddress = $"{worksheet.Cells[rowIndex, StartDateColumn].Address}:{worksheet.Cells[rowIndex, totalColumns].Address}";
                        worksheet.Cells[rowIndex, colIndex].Formula = string.Format("COUNTIF({0},\"BTL\")+(COUNTIF({0},\"BTL½\")/2)", excelAddress);

                        colIndex++;
                        excelAddress = $"{worksheet.Cells[rowIndex, StartDateColumn].Address}:{worksheet.Cells[rowIndex, totalColumns].Address}";
                        worksheet.Cells[rowIndex, colIndex].Formula = string.Format("COUNTIF({0},\"BL3\")", excelAddress);

                        colIndex++;
                        excelAddress = $"{worksheet.Cells[rowIndex, StartDateColumn].Address}:{worksheet.Cells[rowIndex, totalColumns].Address}";
                        worksheet.Cells[rowIndex, colIndex].Formula = string.Format("COUNTIF({0},\"CL\")", excelAddress);

                        colIndex++;
                        foreach (DailyWorking dailyWorking in monthlyWorking.DailyWorkings)
                        {
                            WriteDailyCells(worksheet.Cells[rowIndex, colIndex++], dailyWorking);
                        }

                        rowIndex++;
                    }

                    worksheet.Cells.AutoFitColumns();
                    worksheet.View.FreezePanes(3, 6);

                    return xlPackage.GetAsByteArray();
                }
            }
        }

        public byte[] ExportWeeklyWorkingHours(DateTime fromDate, DateTime toDate, IEnumerable<MonthlyWorking> monthlyWorkings)
        {
            var templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/WeeklyWorkingHours.xlsx");
            const ushort StartDateColumn = 7;
            const ushort TotalColumns = 13;

            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    var redColor = ColorTranslator.FromHtml(Constants.RedColor);
                    const string ShortDateFormat = "dd MMM yyyy";
                    ExcelWorksheet templateWorksheet = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Employee's Weekly Working Hours");
                    var rowIndex = 1;
                    var colIndex = 1;

                    // title
                    templateWorksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                    worksheet.Cells[rowIndex, colIndex].Value = $"Employee's Weekly Working Hours {fromDate.ToString(ShortDateFormat)} - {toDate.ToString(ShortDateFormat)}";
                    rowIndex++;

                    // header
                    templateWorksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                    colIndex = StartDateColumn;
                    for (var day = fromDate.Date; day.Date <= toDate.Date; day = day.AddDays(1))
                    {
                        worksheet.Cells[rowIndex, colIndex].Value = day.ToString("dd MMM, yyyy (ddd)");

                        if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                        {
                            worksheet.Cells[rowIndex, colIndex].Style.Font.Color.SetColor(redColor);
                        }

                        colIndex++;
                    }

                    // data
                    rowIndex++;
                    foreach (MonthlyWorking monthlyWorking in monthlyWorkings)
                    {
                        colIndex = 1;
                        templateWorksheet.Cells[3, 1, 3, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

                        worksheet.Cells[rowIndex, colIndex++].Value = rowIndex - 2;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Code;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Section.Name;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.FullName;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Nickname;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.ActualWorkingDays;
                        foreach (DailyWorking dailyWorking in monthlyWorking.DailyWorkings)
                        {
                            WriteDailyCells(worksheet.Cells[rowIndex, colIndex++], dailyWorking);
                        }

                        rowIndex++;
                    }

                    worksheet.Cells.AutoFitColumns();
                    worksheet.View.FreezePanes(3, 6);

                    return xlPackage.GetAsByteArray();
                }
            }
        }

        public byte[] ExportDailyWorkingHours(DateTime date, IEnumerable<MonthlyWorking> monthlyWorkings)
        {
            var templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/DailyWorkingHours.xlsx");
            const ushort TotalColumns = 13;

            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    var redColor = ColorTranslator.FromHtml(Constants.RedColor);
                    ExcelWorksheet templateWorksheet = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Employee's Daily Working Hours");
                    var rowIndex = 1;
                    var colIndex = 1;

                    // title
                    templateWorksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                    worksheet.Cells[rowIndex, colIndex].Value = $"Employee's Daily Working Hours";
                    rowIndex++;

                    // header
                    templateWorksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                    colIndex = 6;
                    worksheet.Cells[rowIndex, colIndex].Value = date.ToString("dd MMM, yyyy (ddd)");
                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        worksheet.Cells[rowIndex, colIndex].Style.Font.Color.SetColor(redColor);
                    }

                    // data
                    rowIndex++;
                    foreach (MonthlyWorking monthlyWorking in monthlyWorkings)
                    {
                        colIndex = 1;
                        templateWorksheet.Cells[3, 1, 3, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

                        worksheet.Cells[rowIndex, colIndex++].Value = rowIndex - 2;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Code;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Section.Name;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.FullName;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Nickname;
                        WriteDailyCells(worksheet.Cells[rowIndex, colIndex], monthlyWorking.DailyWorkings.First());

                        rowIndex++;
                    }

                    worksheet.Cells.AutoFitColumns();
                    worksheet.View.FreezePanes(3, 1);

                    return xlPackage.GetAsByteArray();
                }
            }
        }

        private static void WriteDailyCells(ExcelRange cell, DailyWorking dailyWorking)
        {
            var backgroundColor = string.Empty;
            var value = string.Empty;
            if (dailyWorking.IsLeaveDay)
            {
                var tupple = GetValueOnLeaveDay(dailyWorking);
                value = tupple.Item1;
                backgroundColor = tupple.Item2;
            }
            else if (dailyWorking.TotalWorkingHours > 0)
            {
                value = dailyWorking.TotalWorkingHoursToDisplay;

                if (dailyWorking.IsHoliday)
                {
                    backgroundColor = Constants.HolidayBackgroundColor;
                }
                else if (dailyWorking.IsOffDay)
                {
                    backgroundColor = Constants.OffDayBackgroundColor;
                }
                else
                {
                    backgroundColor = string.Empty;
                }
            }
            else if (dailyWorking.IsMissingCheckIn)
            {
                value = "Check In";
            }
            else if (dailyWorking.IsMissingCheckOut)
            {
                value = "Check Out";
            }
            else if (dailyWorking.IsAbsent)
            {
                value = "Absent";
            }
            else if (dailyWorking.IsHoliday)
            {
                value = "PH";
                backgroundColor = Constants.HolidayBackgroundColor;
            }
            else if (dailyWorking.IsOffDay)
            {
                value = "OFF";
                backgroundColor = Constants.OffDayBackgroundColor;
            }
            else
            {
                value = string.Empty;
            }

            cell.Value = value;
            if (!string.IsNullOrEmpty(backgroundColor))
            {
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(backgroundColor));
            }
        }

        private static Tuple<string, string> GetValueOnLeaveDay(DailyWorking dailyWorking)
        {
            var backgroundColor = string.Empty;
            var value = string.Empty;

            if (dailyWorking.IsHoliday)
            {
                value = "PH";
                backgroundColor = Constants.HolidayBackgroundColor;
            }
            else if (dailyWorking.IsOffDay)
            {
                value = "OFF";
                backgroundColor = Constants.OffDayBackgroundColor;
            }
            else
            {
                backgroundColor = Utilities.GetBackgroundColorForLeave(dailyWorking.LeaveType.Id);
                value = dailyWorking.LeaveType.Code;
                if (dailyWorking.IsLeaveHalfDay)
                {
                    value = string.Format("{0}½", dailyWorking.LeaveType.Code);
                }
            }

            return new Tuple<string, string>(value, backgroundColor);
        }
    }
}