namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Drawing;
    using Fanex.NexHR.Models.ScheduleArrange;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;

    public interface IExportExcelWorkingShiftService
    {
        byte[] Export(WeeklyScheduleFilter filter, IEnumerable<WeeklySchedule> weeklySchedules);
    }

    public class ExportExcelWorkingShiftService : IExportExcelWorkingShiftService
    {
        private const int defaultFontSize = 11;

        public byte[] Export(WeeklyScheduleFilter filter, IEnumerable<WeeklySchedule> weeklySchedules)
        {
            using (var xlPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Sheet1");
                worksheet.DefaultRowHeight = 14;
                var columns = new List<string>
                {
                    "Nickname"
                };
                for (var day = filter.FromDate.Date; day.Date <= filter.ToDate.Date; day = day.AddDays(1))
                {
                    columns.Add(day.ToString("dd-MMM"));
                }

                int totalColumns = columns.Count;
                var row = 1;
                for (var i = 0; i < totalColumns; i++)
                {
                    var col = i + 1;
                    worksheet.Cells[row, col].Value = columns[i];
                    worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(142, 169, 224));
                }
                worksheet.Row(row).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                row++;

                foreach (WeeklySchedule weeklySchedule in weeklySchedules)
                {
                    int col = 1;

                    worksheet.Cells[row, col].Value = weeklySchedule.Nickname;
                    worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, col].Style.Font.Size = defaultFontSize;
                    col++;

                    WriteDailyCells(worksheet.Cells[row, col++], weeklySchedule.MondaySchedule);
                    WriteDailyCells(worksheet.Cells[row, col++], weeklySchedule.TuesdaySchedule);
                    WriteDailyCells(worksheet.Cells[row, col++], weeklySchedule.WednesdaySchedule);
                    WriteDailyCells(worksheet.Cells[row, col++], weeklySchedule.ThursdaySchedule);
                    WriteDailyCells(worksheet.Cells[row, col++], weeklySchedule.FridaySchedule);
                    WriteDailyCells(worksheet.Cells[row, col++], weeklySchedule.SaturdaySchedule);
                    WriteDailyCells(worksheet.Cells[row, col], weeklySchedule.SundaySchedule);

                    row++;
                }

                return xlPackage.GetAsByteArray();
            }
        }

        private static void WriteDailyCells(ExcelRange cell, DailySchedule dailySchedule)
        {
            var shift = string.Empty;
            if (dailySchedule.IsLeaveOffDay)
            {
                shift = "Off";
            }
            else if (dailySchedule.IsLeaveDay)
            {
                shift = GetShiftOnLeaveDay(dailySchedule);
            }
            else if (dailySchedule.IsHoliday)
            {
                shift = dailySchedule.IsOffDay ? "Off" : dailySchedule.Shift != null ? dailySchedule.Shift.Code : "PH";
            }
            else if (dailySchedule.IsOffDay)
            {
                shift = dailySchedule.Shift != null ? dailySchedule.Shift.Code : "Off";
            }
            else
            {
                shift = dailySchedule.Shift?.Code;
            }

            cell.Value = shift;
            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.Font.Size = defaultFontSize;
        }

        private static string GetShiftOnLeaveDay(DailySchedule dailySchedule)
        {
            var shift = string.Empty;
            if (dailySchedule.IsHoliday)
            {
                shift = "PH";
            }
            else if (dailySchedule.IsOffDay)
            {
                shift = "Off";
            }
            else if (dailySchedule.LeaveDay == 0.5M)
            {
                shift = dailySchedule.Shift != null
                    ? $"{dailySchedule.Shift.Code} ({dailySchedule.LeaveType.Code} ½)" : $"{dailySchedule.LeaveType.Code} ½";
            }
            else
            {
                shift = dailySchedule.LeaveType2 == null
                    ? dailySchedule.LeaveType.Code
                    : $"{dailySchedule.LeaveType.Code} / {dailySchedule.LeaveType2.Code}";
            }

            return shift;
        }
    }
}