namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using NPOI.HSSF.UserModel;

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    public class ReportExporting
    {
        private const int _RowStartBodyWorkingCalendarSummary = 5;
        private const string _EmployeeWorkingCalendarSummaryFilePathExcelTemplate = "~\\ExcelTemplates\\WorkingCalendarSummaryTemplate.xls";

        public HSSFWorkbook ExportWorkingCalendarSummary(
            List<WorkingCalendarSummary> calendars,
            DateTime fromDate,
            DateTime toDate)
        {
            HSSFWorkbook hssfworkbook;

            var templatePath = HttpContext.Current.Server.MapPath(_EmployeeWorkingCalendarSummaryFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, System.IO.FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "WorkingCalendarSummary");

            var rowDate = sheet.GetRow(1);
            rowDate.GetCell(2).SetCellValue(fromDate.ToString(Constants.DateFormat));
            rowDate.GetCell(4).SetCellValue(toDate.ToString(Constants.DateFormat));

            var rowIndex = _RowStartBodyWorkingCalendarSummary;
            var rowCopy = 0;
            var height = sheet.GetRow(_RowStartBodyWorkingCalendarSummary).Height;
            var index = 1;
            int totalCellInRow = 0;

            var groupWorkingCalendarBySchedule = calendars.GroupBy(
                x => new
                {
                    EmpId = x.EmpId,
                    NickName = x.NickName,
                    FullName = x.FullName,
                    Group = x.Name
                },
            x => new { WorkDate = x.WorkDate, Schedule = x.Schedule })
                .Select(x => new { EmployeeInfo = x.Key, Calendar = x.ToList() })
                .ToList();

            if (groupWorkingCalendarBySchedule.Count > 0)
            {
                WriteRowHeaderDate(calendars, ref totalCellInRow, height, sheet);

                foreach (var item in groupWorkingCalendarBySchedule)
                {
                    rowCopy = _RowStartBodyWorkingCalendarSummary + (index % 2);
                    sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                    var row = sheet.GetRow(rowIndex);
                    row.Height = height;
                    var templeteCellInfo = row.GetCell(5);

                    row.GetCell(0).SetCellValue(index);
                    row.GetCell(1).SetCellValue(item.EmployeeInfo.EmpId);
                    row.GetCell(2).SetCellValue(item.EmployeeInfo.FullName);
                    row.GetCell(3).SetCellValue(item.EmployeeInfo.NickName);
                    row.GetCell(4).SetCellValue(item.EmployeeInfo.Group);

                    for (int i = 5; i < totalCellInRow; i++)
                    {
                        var cell = row.CreateCell(i);
                        cell.CellStyle = templeteCellInfo.CellStyle;
                        row.GetCell(i).SetCellValue(item.Calendar[i - 5].Schedule);
                    }

                    rowIndex++;
                    index++;
                }
            }

            hssfworkbook.RemoveSheetAt(0);

            return hssfworkbook;
        }

        private static void WriteRowHeaderDate(List<WorkingCalendarSummary> calendars, ref int totalCellInRow, short height, NPOI.SS.UserModel.ISheet sheet)
        {
            var groupWorkingCalendarBySchedule = calendars.GroupBy(
                x => new
                {
                    EmpId = x.EmpId,
                    NickName = x.NickName,
                    FullName = x.FullName,
                    Group = x.Name
                },
            x => new { WorkDate = x.WorkDate, Schedule = x.Schedule })
                .Select(x => new { EmployeeInfo = x.Key, Calendar = x.ToList() })
                .ToList();

            int countItem = 0;
            int startColumn = 5;

            var firstItemInGroup = groupWorkingCalendarBySchedule[0];
            if (firstItemInGroup.Calendar.Any())
            {
                countItem = firstItemInGroup.Calendar.Count;
                totalCellInRow = countItem + startColumn;

                var rowHeaderDate = sheet.GetRow(3);
                var rowHeaderDay = sheet.GetRow(4);
                rowHeaderDate.Height = height;
                rowHeaderDay.Height = height;

                var templeteCellHeader = rowHeaderDate.GetCell(5);
                const int customCellWidth = 3071;

                for (int i = 5; i < totalCellInRow; i++)
                {
                    var cellHeaderDate = rowHeaderDate.CreateCell(i);
                    var cellHeaderDay = rowHeaderDay.CreateCell(i);

                    cellHeaderDate.CellStyle = templeteCellHeader.CellStyle;
                    cellHeaderDay.CellStyle = templeteCellHeader.CellStyle;

                    cellHeaderDate.Sheet.SetColumnWidth(i, customCellWidth);
                    cellHeaderDay.Sheet.SetColumnWidth(i, customCellWidth);
                }

                // Row Header Date
                rowHeaderDate.GetCell(0).SetCellValue(Labels.SymbolCount);
                rowHeaderDate.GetCell(1).SetCellValue(Labels.Code);
                rowHeaderDate.GetCell(2).SetCellValue(Labels.FullName);
                rowHeaderDate.GetCell(3).SetCellValue(Labels.NickName);
                rowHeaderDate.GetCell(4).SetCellValue(Labels.Section);

                foreach (var item in firstItemInGroup.Calendar)
                {
                    if (startColumn < totalCellInRow)
                    {
                        rowHeaderDate.GetCell(startColumn).SetCellValue(item.WorkDate.ToString(Constants.DateFormat));
                        rowHeaderDay.GetCell(startColumn).SetCellValue(item.WorkDate.ToString("ddd"));
                    }

                    startColumn++;
                }
            }
        }
    }
}