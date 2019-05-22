namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models.ScheduleArrange;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;

    public interface IExportExcelScheduleService
    {
        byte[] Export(WeeklyScheduleFilter filter, IEnumerable<WeeklySchedule> weeklySchedules);
    }

    public class ExportExcelScheduleService : IExportExcelScheduleService
    {
        private const int DefaultFontSize = 11;
        private const int DefaultRowHeight = 14;

        public byte[] Export(WeeklyScheduleFilter filter, IEnumerable<WeeklySchedule> weeklySchedules)
        {
            using (var xlPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Sheet1");
                worksheet.DefaultRowHeight = DefaultRowHeight;
                var columns = new List<string>
                {
                    "ID",
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

                    worksheet.Cells[row, col].Value = weeklySchedule.Code;
                    worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, col].Style.Font.Size = DefaultFontSize;
                    col++;

                    worksheet.Cells[row, col].Value = weeklySchedule.Nickname;
                    worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, col].Style.Font.Size = DefaultFontSize;
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

                worksheet.View.FreezePanes(2, 1);

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
            cell.Style.Font.Size = DefaultFontSize;
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
                shift = GetShiftOnLeaveHalfDay(dailySchedule);
            }
            else
            {
                shift = dailySchedule.LeaveType.Code;
            }

            return shift;
        }

        private static string GetShiftOnLeaveHalfDay(DailySchedule dailySchedule)
        {
            var shift = string.Empty;
            if (dailySchedule.Shift != null)
            {
                var checkInTime = GetCheckInTime(dailySchedule.WorkDate, dailySchedule.Shift);
                var checkOutTime = GetCheckOutTime(dailySchedule.WorkDate, dailySchedule.Shift);
                if (dailySchedule.LeaveType.FromTime.Value == checkInTime)
                {
                    shift = $"{GetHalfShiftCode(dailySchedule.LeaveType.ToTime.Value.ToShortTime())} >{GetHalfShiftCode(dailySchedule.Shift.CheckOut)}";
                }
                else if (dailySchedule.LeaveType.ToTime.Value == checkOutTime)
                {
                    shift = $"{GetHalfShiftCode(dailySchedule.Shift.CheckIn)} >{GetHalfShiftCode(dailySchedule.LeaveType.ToTime.Value.ToShortTime())}";
                }
                else
                {
                    shift = string.Empty;
                }
            }

            return shift;
        }

        private static DateTime GetCheckInTime(DateTime workDate, ShiftBase shift)
        {
            var checkInTime = TimeSpan.Parse(shift.CheckIn);
            return workDate.Date.AddDays(shift.IsInAfterDay ? 1 : 0).AddHours(checkInTime.Hours);
        }

        private static DateTime GetCheckOutTime(DateTime workDate, ShiftBase shift)
        {
            var checkOutTime = TimeSpan.Parse(shift.CheckOut);
            return workDate.Date.AddDays(shift.IsOutAfterDay ? 1 : 0).AddHours(checkOutTime.Hours);
        }

        private static string GetHalfShiftCode(string shortTime)
        {
            var time = TimeSpan.Parse(shortTime);

            return time.Minutes > 0 ? time.ToString(@"hh\:mm") : time.ToString("hh");
        }
    }
}