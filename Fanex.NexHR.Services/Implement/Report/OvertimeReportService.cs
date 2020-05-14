namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using Fanex.Common.Extensions;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models.Report.Overtime;
    using Fanex.NexHR.Repositories;
    using OfficeOpenXml;

    public class OvertimeReportService : IOvertimeReportService
    {
        private readonly IBaseService baseService;
        private readonly IOvertimeReportRepository overtimeReportRepository;
        private const string DateFormat = "dd-MMM-yyyy";
        private const byte MinimumWidth = 5;
        private const byte MaximumWidth = 50;

        public OvertimeReportService(
                IBaseService baseService,
                IOvertimeReportRepository overtimeReportRepository)
        {
            this.baseService = baseService;
            this.overtimeReportRepository = overtimeReportRepository;
        }

        public IEnumerable<YearlyOvertime> GetYearlyOvertime(GetYearlyOvertimeParam param)
        {
            var monthxml = new StringBuilder(Constants.BeginRootTag);
            for (var month = 1; month <= Constants.December; month++)
            {
                var fromDate = new DateTime(param.Year, month, 1);
                var toDate = fromDate.LastDayOfMonth();

                if (param.Calendar == (byte)CalendarType.Fiscal)
                {
                    var daterange = GetDateRange(fromDate);
                    fromDate = daterange.Item1;
                    toDate = daterange.Item2;
                }

                monthxml.AppendFormat("<Row Month='{0}' FromDate='{1}' ToDate='{2}' />", month, fromDate, toDate);
            }

            monthxml.Append(Constants.EndRootTag);

            var criteria = new GetYearlyOvertimeReportCriteria
            {
                ActorId = param.ActorId,
                GroupIds = param.SectionIds,
                Employee = param.Employee,
                EmployeeStatusId = param.EmployeeStatusId,
                MonthXml = monthxml.ToString()
            };
            return baseService.GetData<YearlyOvertime>(criteria);
        }

        public IEnumerable<DailyOvertimeDetails> GetMonthlyOvertimeDetails(GetMonthlyOvertimeDetailParam param)
        {
            var fromDate = new DateTime(param.Year, param.Month, 1);
            var toDate = fromDate.LastDayOfMonth();

            if (param.Calendar == CalendarType.Fiscal)
            {
                var daterange = GetDateRange(fromDate);
                fromDate = daterange.Item1;
                toDate = daterange.Item2;
            }

            var criteria = new GetMonthlyOvertimeDetailsReportCriteria
            {
                EmployeeId = param.EmployeeId,
                FromDate = fromDate,
                ToDate = toDate
            };
            return baseService.GetData<DailyOvertimeDetails>(criteria);
        }

        public Tuple<IEnumerable<DateRange>, IEnumerable<DailyOvertimeDetails>> GetMonthlyOvertime(GetMonthlyOvertimeParam param)
        {
            var criteria = new GetMonthlyOvertimeReportCriteria
            {
                ActorId = param.ActorId,
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                Employee = param.Employee,
                EmployeeStatusId = param.EmployeeStatusId
            };
            return overtimeReportRepository.GetMonthlyOvertime(criteria);
        }

        public IEnumerable<DailyOvertimeDetails> GetDailyOvertimeDetails(GetDailyOvertimeDetailsParam param)
        {
            var criteria = new GetDailyOvertimeReportCriteria
            {
                ActorId = param.ActorId,
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                GroupIds = param.SectionIds,
                Employee = param.Employee,
                IsLocal = param.IsLocal,
                EmployeeStatusId = param.EmployeeStatusId
            };
            return baseService.GetData<DailyOvertimeDetails>(criteria);
        }

        public byte[] ExportYearlyOvertime(IEnumerable<YearlyOvertime> overtimes, GetYearlyOvertimeParam param)
        {
            var templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/YearlyOvertime.xlsx");
            const byte TotalColumns = 17;
            const byte TotalHeaderRows = 2;
            const byte StartMonthColumn = 5;
            const byte StartBodyRow = 4;
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
                    rowIndex += TotalHeaderRows;

                    // data
                    var rowNum = 1;
                    foreach (YearlyOvertime overtime in overtimes)
                    {
                        colIndex = 1;
                        templateWorksheet.Cells[StartBodyRow, 1, StartBodyRow, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

                        worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
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
                    const byte TotalMonths = 12;
                    for (var i = 0; i <= TotalMonths; i++)
                    {
                        colIndex = StartMonthColumn + i;
                        var excelAddress = $"{worksheet.Cells[StartBodyRow, colIndex].Address}:{worksheet.Cells[rowIndex - 1, colIndex].Address}";
                        worksheet.Cells[rowIndex, colIndex].Formula = string.Format(Constants.SummaryFormula, excelAddress);
                    }

                    worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
                    worksheet.View.FreezePanes(StartBodyRow, 1);

                    return xlPackage.GetAsByteArray();
                }
            }
        }

        public byte[] ExportMonthlyOvertime(IEnumerable<DateRange> dateRanges, IEnumerable<DailyOvertimeDetails> dailyOvertimes)
        {
            var templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/MonthlyOvertime.xlsx");
            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    ExcelWorksheet template = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Raw Data");
                    WriteRawData(template, worksheet, dateRanges, dailyOvertimes);

                    const byte OvertimeSheetIndex = 2;
                    template = templatePackage.Workbook.Worksheets[OvertimeSheetIndex];
                    worksheet = xlPackage.Workbook.Worksheets.Add("Overtime Record");
                    WriteOvertimeRecord(template, worksheet, dateRanges, dailyOvertimes);

                    const byte SummarySheetIndex = 3;
                    template = templatePackage.Workbook.Worksheets[SummarySheetIndex];
                    worksheet = xlPackage.Workbook.Worksheets.Add("Summary OT");
                    WriteSummaryOvertime(template, worksheet, dateRanges, dailyOvertimes);

                    return xlPackage.GetAsByteArray();
                }
            }
        }

        public byte[] ExportDailyOvertimeDetails(IEnumerable<DailyOvertimeDetails> overtimes, GetDailyOvertimeDetailsParam param)
        {
            var templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/DailyOvertimeDetails.xlsx");
            const byte TotalColumns = 16;
            const byte StartBodyRow = 3;
            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    ExcelWorksheet templateWorksheet = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Overtime Details Report");
                    var rowIndex = 1;
                    var colIndex = 1;

                    // title
                    templateWorksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                    worksheet.Cells[rowIndex, colIndex].Value = string.Format(
                        "Overtime Details Report from {0} to {1}",
                        param.FromDate.ToString(DateFormat),
                        param.ToDate.ToString(DateFormat));
                    rowIndex++;

                    // header
                    templateWorksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                    rowIndex++;

                    // data
                    var rowNum = 1;
                    foreach (DailyOvertimeDetails overtime in overtimes)
                    {
                        var status = (CurrentRequestStatus)overtime.StatusId;
                        colIndex = 1;
                        templateWorksheet.Cells[StartBodyRow, 1, StartBodyRow, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

                        worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.WorkDate;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.Code;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.FullName;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.Nickname;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.Section;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.ShiftCode;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.TimeIn;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.TimeOut;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.ActualWorkingHours;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.NormalHours;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.OffHours;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.PublicHours;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.SpecialHours;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime.Remark;
                        worksheet.Cells[rowIndex, colIndex].Value = status.ToString();

                        rowIndex++;
                    }

                    worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
                    worksheet.View.FreezePanes(StartBodyRow, 1);

                    return xlPackage.GetAsByteArray();
                }
            }
        }

        private static void WriteRawData(
            ExcelWorksheet template,
            ExcelWorksheet worksheet,
            IEnumerable<DateRange> dateRanges,
            IEnumerable<DailyOvertimeDetails> dailyOvertimes)
        {
            const byte TotalColumns = 16;
            var templateIndex = new
            {
                bodyRow = 3,
                summaryRow = 4,
                startSummaryColumn = 11
            };
            var fromDate = dateRanges.Min(x => x.Date);
            var toDate = dateRanges.Max(x => x.Date);
            var rowIndex = 1;
            var colIndex = 1;

            // title
            template.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
            worksheet.Cells[rowIndex, colIndex].Value = string.Format(
                worksheet.Cells[rowIndex, colIndex].Value.ToString(),
                fromDate.ToString(DateFormat),
                toDate.ToString(DateFormat));
            rowIndex++;

            // header
            template.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
            rowIndex++;

            // data
            var rowNum = 1;
            foreach (DailyOvertimeDetails overtime in dailyOvertimes)
            {
                var status = (CurrentRequestStatus)overtime.StatusId;
                colIndex = 1;
                template.Cells[templateIndex.bodyRow, 1, templateIndex.bodyRow, TotalColumns]
                    .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

                worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
                worksheet.Cells[rowIndex, colIndex++].Value = overtime.WorkDate;
                worksheet.Cells[rowIndex, colIndex++].Value = overtime.Code;
                worksheet.Cells[rowIndex, colIndex++].Value = overtime.FullName;
                worksheet.Cells[rowIndex, colIndex++].Value = overtime.Nickname;
                worksheet.Cells[rowIndex, colIndex++].Value = overtime.Section;
                worksheet.Cells[rowIndex, colIndex++].Value = overtime.ShiftCode;
                worksheet.Cells[rowIndex, colIndex++].Value = overtime.TimeIn;
                worksheet.Cells[rowIndex, colIndex++].Value = overtime.TimeOut;
                worksheet.Cells[rowIndex, colIndex++].Value = overtime.ActualWorkingHours;
                worksheet.Cells[rowIndex, colIndex++].Value = overtime.NormalHours;
                worksheet.Cells[rowIndex, colIndex++].Value = overtime.OffHours;
                worksheet.Cells[rowIndex, colIndex++].Value = overtime.PublicHours;
                worksheet.Cells[rowIndex, colIndex++].Value = overtime.SpecialHours;
                worksheet.Cells[rowIndex, colIndex++].Value = overtime.Remark;
                worksheet.Cells[rowIndex, colIndex].Value = status.ToString();

                rowIndex++;
            }

            // summary
            if (dailyOvertimes.Any())
            {
                template.Cells[templateIndex.summaryRow, 1, templateIndex.summaryRow, TotalColumns].Copy(worksheet.Cells[rowIndex, 1, rowIndex, TotalColumns]);
                const byte TotalSummaryColumns = 4;
                for (var i = 0; i < TotalSummaryColumns; i++)
                {
                    colIndex = templateIndex.startSummaryColumn + i;
                    var excelAddress = $"{worksheet.Cells[templateIndex.bodyRow, colIndex].Address}:{worksheet.Cells[rowIndex - 1, colIndex].Address}";
                    worksheet.Cells[rowIndex, colIndex].Formula = string.Format(Constants.SummaryFormula, excelAddress);
                }
            }

            worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
            worksheet.View.FreezePanes(templateIndex.bodyRow, 1);
        }

        private static void WriteOvertimeRecord(
            ExcelWorksheet template,
            ExcelWorksheet worksheet,
            IEnumerable<DateRange> dateRanges,
            IEnumerable<DailyOvertimeDetails> dailyOvertimes)
        {
            const byte TotalFixedColumns = 9;
            var templateIndex = new
            {
                headerRow = 2,
                bodyRow = 3,
                summaryRow = 4,
                startSummaryColumn = 6,
                startNormalDayColumn = 10,
                endNormalDayColumn = 12,
                holidayColumn = 13
            };
            var rowIndex = 3;
            var colIndex = 1;

            // title & header
            WriteHeaderOvertimeRecord(template, worksheet, dateRanges, templateIndex);

            // data
            var rowNum = 1;
            IEnumerable<DailyOvertimeDetails> employees = dailyOvertimes.GroupBy(x => x.EmpId)
                .Select(x => x.First()).OrderByDescending(x => x.Section).ThenBy(x => x.Code);
            foreach (DailyOvertimeDetails employee in employees)
            {
                IEnumerable<DailyOvertimeDetails> overtimes = dailyOvertimes.Where(x => x.EmpId == employee.EmpId);
                colIndex = 1;
                template.Cells[templateIndex.bodyRow, 1, templateIndex.bodyRow, TotalFixedColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalFixedColumns]);

                worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
                worksheet.Cells[rowIndex, colIndex++].Value = employee.Code;
                worksheet.Cells[rowIndex, colIndex++].Value = employee.FullName;
                worksheet.Cells[rowIndex, colIndex++].Value = employee.Nickname;
                worksheet.Cells[rowIndex, colIndex++].Value = employee.Section;
                worksheet.Cells[rowIndex, colIndex++].Value = overtimes.Sum(x => x.NormalHours);
                worksheet.Cells[rowIndex, colIndex++].Value = overtimes.Sum(x => x.OffHours);
                worksheet.Cells[rowIndex, colIndex++].Value = overtimes.Sum(x => x.PublicHours);
                worksheet.Cells[rowIndex, colIndex++].Value = overtimes.Sum(x => x.SpecialHours);

                foreach (DateRange date in dateRanges)
                {
                    DailyOvertimeDetails overtime = overtimes.FirstOrDefault(x => x.WorkDate.Date == date.Date.Date);
                    if (date.IsHoliday)
                    {
                        template.Cells[templateIndex.bodyRow, templateIndex.startNormalDayColumn].Copy(worksheet.Cells[rowIndex, colIndex]);
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime?.PublicHours;
                    }
                    else
                    {
                        var toColumn = colIndex + 3;
                        template.Cells[templateIndex.bodyRow, templateIndex.startNormalDayColumn, templateIndex.bodyRow, templateIndex.endNormalDayColumn]
                            .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, toColumn]);
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime?.NormalHours;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime?.OffHours;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime?.SpecialHours;
                    }
                }

                rowIndex++;
            }

            // summary
            if (employees.Any())
            {
                const byte TotalSummaryColumns = 4;
                template.Cells[templateIndex.summaryRow, 1, templateIndex.summaryRow, TotalFixedColumns]
                    .Copy(worksheet.Cells[rowIndex, 1, rowIndex, TotalFixedColumns]);
                for (var i = 0; i < TotalSummaryColumns; i++)
                {
                    colIndex = templateIndex.startSummaryColumn + i;
                    var excelAddress = $"{worksheet.Cells[templateIndex.bodyRow, colIndex].Address}:{worksheet.Cells[rowIndex - 1, colIndex].Address}";
                    worksheet.Cells[rowIndex, colIndex].Formula = string.Format(Constants.SummaryFormula, excelAddress);
                }
            }

            worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
            worksheet.View.FreezePanes(templateIndex.bodyRow, templateIndex.startNormalDayColumn);
        }

        private static void WriteHeaderOvertimeRecord(
            ExcelWorksheet template,
            ExcelWorksheet worksheet,
            IEnumerable<DateRange> dateRanges,
            dynamic templateIndex)
        {
            const byte TotalFixedColumns = 9;
            const byte TotalNormalDayColumns = 3;
            var colIndex = 1;
            var fromDate = dateRanges.Min(x => x.Date);
            var toDate = dateRanges.Max(x => x.Date);
            template.Cells[1, colIndex, templateIndex.headerRow, TotalFixedColumns].Copy(worksheet.Cells[1, colIndex, templateIndex.headerRow, TotalFixedColumns]);
            worksheet.Cells[1, colIndex].Value = string.Format(
                worksheet.Cells[1, colIndex].Value.ToString(),
                fromDate.ToString(DateFormat),
                toDate.ToString(DateFormat));
            colIndex = templateIndex.startNormalDayColumn;
            foreach (DateRange date in dateRanges)
            {
                if (date.IsHoliday)
                {
                    template.Cells[1, templateIndex.holidayColumn].Copy(worksheet.Cells[1, colIndex]);
                    worksheet.Cells[1, colIndex].Value = date.Date.ToString("dd-MMM");
                    template.Cells[templateIndex.headerRow, templateIndex.holidayColumn].Copy(worksheet.Cells[templateIndex.headerRow, colIndex]);
                }
                else
                {
                    var toColumn = colIndex + TotalNormalDayColumns;
                    template.Cells[1, templateIndex.startNormalDayColumn, 1, templateIndex.endNormalDayColumn]
                        .Copy(worksheet.Cells[1, colIndex, 1, toColumn]);
                    worksheet.Cells[1, colIndex].Value = date.Date.ToString("dd-MMM");
                    template.Cells[templateIndex.headerRow, templateIndex.startNormalDayColumn, templateIndex.headerRow, templateIndex.endNormalDayColumn]
                        .Copy(worksheet.Cells[templateIndex.headerRow, colIndex, templateIndex.headerRow, toColumn]);
                }

                colIndex += date.IsHoliday ? 1 : TotalNormalDayColumns;
            }
        }

        private static void WriteSummaryOvertime(
            ExcelWorksheet template,
            ExcelWorksheet worksheet,
            IEnumerable<DateRange> dateRanges,
            IEnumerable<DailyOvertimeDetails> dailyOvertimes)
        {
            const byte TotalColumns = 5;
            var templateIndex = new
            {
                bodyRow = 3,
                summaryRow = 4,
                startSummaryColumn = 2
            };
            var fromDate = dateRanges.Min(x => x.Date);
            var toDate = dateRanges.Max(x => x.Date);
            var rowIndex = 1;
            var colIndex = 1;

            // title
            template.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
            worksheet.Cells[rowIndex, colIndex].Value = string.Format(
                worksheet.Cells[rowIndex, colIndex].Value.ToString(),
                fromDate.ToString(DateFormat),
                toDate.ToString(DateFormat));
            rowIndex++;

            // header
            template.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
            rowIndex++;

            // data
            IEnumerable<string> sections = dailyOvertimes.Select(x => x.Section).Distinct().OrderBy(x => x);
            foreach (var section in sections)
            {
                IEnumerable<DailyOvertimeDetails> overtimes = dailyOvertimes.Where(x => x.Section == section);
                colIndex = 1;
                template.Cells[templateIndex.bodyRow, 1, templateIndex.bodyRow, TotalColumns]
                    .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                worksheet.Cells[rowIndex, colIndex++].Value = section;
                worksheet.Cells[rowIndex, colIndex++].Value = overtimes.Sum(x => x.NormalHours);
                worksheet.Cells[rowIndex, colIndex++].Value = overtimes.Sum(x => x.OffHours);
                worksheet.Cells[rowIndex, colIndex++].Value = overtimes.Sum(x => x.PublicHours);
                worksheet.Cells[rowIndex, colIndex].Value = overtimes.Sum(x => x.SpecialHours);

                rowIndex++;
            }

            // summary
            if (sections.Any())
            {
                template.Cells[templateIndex.summaryRow, 1, templateIndex.summaryRow, TotalColumns].Copy(worksheet.Cells[rowIndex, 1, rowIndex, TotalColumns]);
                const byte TotalSummaryColumns = 4;
                for (var i = 0; i < TotalSummaryColumns; i++)
                {
                    colIndex = templateIndex.startSummaryColumn + i;
                    var excelAddress = $"{worksheet.Cells[templateIndex.bodyRow, colIndex].Address}:{worksheet.Cells[rowIndex - 1, colIndex].Address}";
                    worksheet.Cells[rowIndex, colIndex].Formula = string.Format(Constants.SummaryFormula, excelAddress);
                }
            }

            worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
            worksheet.View.FreezePanes(templateIndex.bodyRow, 1);
        }

        private static Tuple<DateTime, DateTime> GetDateRange(DateTime month)
        {
            const byte FromDay = 21;
            const byte ToDay = 20;
            var newRuleApplyFromMonth = new DateTime(2020, 5, 1);
            var lastMonth = month.AddMonths(-1);
            var fromDate = new DateTime(lastMonth.Year, lastMonth.Month, 10);
            var toDate = new DateTime(month.Year, month.Month, 9);

            if (month == newRuleApplyFromMonth)
            {
                toDate = new DateTime(month.Year, month.Month, ToDay);
            }

            if (month > newRuleApplyFromMonth)
            {
                fromDate = new DateTime(lastMonth.Year, lastMonth.Month, FromDay);
                toDate = new DateTime(month.Year, month.Month, ToDay);
            }

            return new Tuple<DateTime, DateTime>(fromDate, toDate);
        }
    }
}