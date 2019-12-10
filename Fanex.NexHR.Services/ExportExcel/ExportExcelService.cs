namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.ScheduleArrange;
    using Fanex.NexHR.Resources;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    public class ExportExcelService : IExportExcelService
    {
        private const string DefaultSheetName = "sheet1";
        private const int _RowStartBodyLeaveSummary = 4;
        private const int _RowStartBodyWorkingTimeRequest = 3;
        private const int _RowStartBodyWorkingCalendar = 5;

        private const string _LeaveSummaryFilePathExcelTemplate = "~\\ExcelTemplates\\LeaveSummaryTemplate.xls";
        private const string _WorkingTimeRequestFilePathExcelTemplate = "~\\ExcelTemplates\\WorkingTimeRequestTemplate.xls";
        private const string _EmployeeWorkingCalendarFilePathExcelTemplate = "~\\ExcelTemplates\\WorkingCalendarTemplate.xls";

        public HSSFWorkbook ExportLeaveSummary(IList<LeaveSummaryByTeam> leaveSummaryList, int year)
        {
            HSSFWorkbook hssfworkbook;

            var templatePath = HttpContext.Current.Server.MapPath(_LeaveSummaryFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet(DefaultSheetName);
            var sheet = hssfworkbook.CloneSheet(DefaultSheetName, "LeaveSummary");

            //// render hearder
            var rowTakeLeave = sheet.GetRow(2);
            var cellTakeLeave = rowTakeLeave.GetCell(5);
            cellTakeLeave.SetCellValue("Leave in " + year);

            var rowAnnualBalance = sheet.GetRow(2);
            var cellAnnualBalance = rowAnnualBalance.GetCell(17);
            cellAnnualBalance.SetCellValue("Balance Of AL " + year);

            var cellKIVBalance = rowAnnualBalance.GetCell(18);
            cellKIVBalance.SetCellValue("Balance Of KIV " + year);

            //// render body
            var rowIndex = _RowStartBodyLeaveSummary;
            var height = sheet.GetRow(_RowStartBodyLeaveSummary).Height;
            var index = 1;

            foreach (var item in leaveSummaryList)
            {
                if (item.Team != null && item.LeaveSummaryInTeam.Count > 0)
                {
                    //// add row team
                    var rowCopy = _RowStartBodyLeaveSummary + 2;
                    sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                    var rowTeam = sheet.GetRow(rowIndex);
                    var cellTeam = rowTeam.GetCell(0);
                    cellTeam.SetCellValue(item.Team.Name.ToUpperInvariant());
                    sheet.MergeCells(rowIndex, 0, lastRow: rowIndex, lastCol: 18);
                    rowTeam.Height = height;

                    rowIndex++;
                    foreach (var leaveSummary in item.LeaveSummaryInTeam)
                    {
                        rowCopy = _RowStartBodyLeaveSummary + (index % 2);
                        sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                        var row = sheet.GetRow(rowIndex);
                        row.Height = height;

                        row.GetCell(0).SetCellValue(index);
                        row.GetCell(1).SetCellValue(leaveSummary.Employee.NickName);
                        row.GetCell(2).SetCellValue(leaveSummary.Employee.FullName);
                        row.GetCell(3).SetCellValue(leaveSummary.CreditKIV);
                        row.GetCell(4).SetCellValue(leaveSummary.AnnualLeaveCurrentYear);
                        row.GetCell(5).SetCellValue(leaveSummary.Jan);
                        row.GetCell(6).SetCellValue(leaveSummary.Feb);
                        row.GetCell(7).SetCellValue(leaveSummary.Mar);
                        row.GetCell(8).SetCellValue(leaveSummary.Apr);
                        row.GetCell(9).SetCellValue(leaveSummary.May);
                        row.GetCell(10).SetCellValue(leaveSummary.Jun);
                        row.GetCell(11).SetCellValue(leaveSummary.Jul);
                        row.GetCell(12).SetCellValue(leaveSummary.Aug);
                        row.GetCell(13).SetCellValue(leaveSummary.Sep);
                        row.GetCell(14).SetCellValue(leaveSummary.Oct);
                        row.GetCell(15).SetCellValue(leaveSummary.Nov);
                        row.GetCell(16).SetCellValue(leaveSummary.Dec);
                        row.GetCell(17).SetCellValue(leaveSummary.LeaveBalance);
                        row.GetCell(18).SetCellValue(leaveSummary.KIVBalance);

                        index++;
                        rowIndex++;
                    }
                }
            }
            //// end render body

            //// remove sheet & row template
            hssfworkbook.RemoveSheetAt(0);

            return hssfworkbook;
        }

        public HSSFWorkbook ExportWorkingTimeRequest(IList<RequestEntity> requestList, string title)
        {
            HSSFWorkbook hssfworkbook;
            var templatePath = HttpContext.Current.Server.MapPath(_WorkingTimeRequestFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet(DefaultSheetName);
            var sheet = hssfworkbook.CloneSheet(DefaultSheetName, title);
            var rowIndex = _RowStartBodyWorkingTimeRequest;
            var height = sheet.GetRow(_RowStartBodyWorkingTimeRequest).Height;

            var rowTitle = sheet.GetRow(0);
            var cellTitle = rowTitle.GetCell(0);
            cellTitle.SetCellValue(title);
            foreach (var item in requestList)
            {
                var rowCopy = _RowStartBodyWorkingTimeRequest + (rowIndex % 2);
                sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                var row = sheet.GetRow(rowIndex);
                row.Height = height;
                RenderBodyWorkingTimeRequest(rowIndex, item, row);
                rowIndex++;
            }

            hssfworkbook.RemoveSheetAt(0);

            return hssfworkbook;
        }

        public HSSFWorkbook ExportEmployeeWorkingCalendar(IEnumerable<WorkingCalendar> workingCalendars, DateTime fromDate, DateTime toDate)
        {
            HSSFWorkbook hssfworkbook;
            var templatePath = HttpContext.Current.Server.MapPath(_EmployeeWorkingCalendarFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet(DefaultSheetName);
            var sheet = hssfworkbook.CloneSheet(DefaultSheetName, "WorkingCalendar");
            var rowIndex = _RowStartBodyWorkingCalendar;

            var rowDate = sheet.GetRow(1);
            ICell cellFromDate = rowDate.GetCell(2);
            ICell cellToDate = rowDate.GetCell(4);
            cellFromDate.SetCellValue(fromDate);
            cellToDate.SetCellValue(toDate);

            var rowHeaderDate = sheet.GetRow(3);
            rowHeaderDate.GetCell(5).SetCellValue(fromDate);
            rowHeaderDate.GetCell(6).SetCellValue(fromDate.AddDays(1));
            rowHeaderDate.GetCell(7).SetCellValue(fromDate.AddDays(2));
            rowHeaderDate.GetCell(8).SetCellValue(fromDate.AddDays(3));
            rowHeaderDate.GetCell(9).SetCellValue(fromDate.AddDays(4));
            rowHeaderDate.GetCell(10).SetCellValue(fromDate.AddDays(5));
            rowHeaderDate.GetCell(11).SetCellValue(toDate);

            var rowHeaderDay = sheet.GetRow(4);
            rowHeaderDay.GetCell(5).SetCellValue(fromDate);
            rowHeaderDay.GetCell(6).SetCellValue(fromDate.AddDays(1));
            rowHeaderDay.GetCell(7).SetCellValue(fromDate.AddDays(2));
            rowHeaderDay.GetCell(8).SetCellValue(fromDate.AddDays(3));
            rowHeaderDay.GetCell(9).SetCellValue(fromDate.AddDays(4));
            rowHeaderDay.GetCell(10).SetCellValue(fromDate.AddDays(5));
            rowHeaderDay.GetCell(11).SetCellValue(toDate);

            var index = 1;
            foreach (var item in workingCalendars)
            {
                var rowCopy = _RowStartBodyWorkingCalendar + (index % 2);
                sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                var row = sheet.GetRow(rowIndex);
                ICell cellNo = row.GetCell(0);
                ICell cellEmployeeID = row.GetCell(1);
                ICell cellFullName = row.GetCell(2);
                ICell cellNickName = row.GetCell(3);
                ICell cellTeam = row.GetCell(4);
                ICell cellShift1 = row.GetCell(5);
                ICell cellShift2 = row.GetCell(6);
                ICell cellShift3 = row.GetCell(7);
                ICell cellShift4 = row.GetCell(8);
                ICell cellShift5 = row.GetCell(9);
                ICell cellShift6 = row.GetCell(10);
                ICell cellShift7 = row.GetCell(11);

                cellNo.SetCellValue(rowIndex - _RowStartBodyWorkingCalendar + 1);
                cellEmployeeID.SetCellValue(item.Code);
                cellFullName.SetCellValue(item.FullName);
                cellNickName.SetCellValue(item.Nickname);
                cellTeam.SetCellValue(item.Section);
                cellShift1.SetCellValue(GetShift(item.MondaySchedule));
                cellShift2.SetCellValue(GetShift(item.TuesdaySchedule));
                cellShift3.SetCellValue(GetShift(item.WednesdaySchedule));
                cellShift4.SetCellValue(GetShift(item.ThursdaySchedule));
                cellShift5.SetCellValue(GetShift(item.FridaySchedule));
                cellShift6.SetCellValue(GetShift(item.SaturdaySchedule));
                cellShift7.SetCellValue(GetShift(item.SundaySchedule));
                rowIndex++;
                index++;
            }

            sheet.CreateFreezePane(0, 5);
            hssfworkbook.RemoveSheetAt(0);

            return hssfworkbook;
        }

        private static void RenderBodyWorkingTimeRequest(
            int rowIndex,
            RequestEntity item,
            IRow row)
        {
            ICell cellNo = null;
            ICell cellTeam = null;
            ICell cellNick = null;
            ICell cellName = null;
            ICell cellRequest = null;
            ICell cellDetails = null;
            ICell cellRemarks = null;
            ICell cellCreateDate = null;
            ICell cellStatus = null;

            cellNo = row.GetCell(0);
            cellTeam = row.GetCell(1);
            cellNick = row.GetCell(2);
            cellName = row.GetCell(3);
            cellRequest = row.GetCell(4);
            cellDetails = row.GetCell(5);
            cellRemarks = row.GetCell(6);
            cellCreateDate = row.GetCell(7);
            cellStatus = row.GetCell(8);

            cellNo.SetCellValue(rowIndex - _RowStartBodyLeaveSummary + 2);
            cellTeam.SetCellValue(item.Requester.Team.Code);
            cellNick.SetCellValue(item.Requester.NickName);
            cellName.SetCellValue(item.Requester.FullName);
            cellRequest.SetCellValue(item.RequestType.Id == (byte)RequestType.Leave ? item.LeaveType.Name : item.RequestType.Name);

            cellDetails.SetCellValue(RequestService.ToRequestMessage(item, isNoFormat: true));
            cellRemarks.SetCellValue(item.Remark);
            cellCreateDate.SetCellValue(item.CreatedDate);
            cellStatus.SetCellValue(item.Status.Name);
        }

        public static void WriteRowExcelReport(ExcelTemplateWriter writer, int rowIndex, int rowStartBody, object record)
        {
            var rowStart = (rowIndex % 2 != 0) ? rowStartBody : rowStartBody + 1;
            writer.WriteRow(rowStart, record);
        }

        private static string GetShift(DailySchedule schedule)
        {
            var shift = string.Empty;
            if (schedule.IsLeaveOffDay)
            {
                shift = Labels.Off;
            }
            else if (schedule.IsLeaveDay)
            {
                shift = GetShiftOnLeaveDay(schedule);
            }
            else if (schedule.IsHoliday)
            {
                shift = schedule.Shift != null ? schedule.Shift.Code : Labels.PH;
            }
            else if (schedule.IsOffDay)
            {
                shift = schedule.Shift != null ? schedule.Shift.Code : Labels.Off;
            }
            else
            {
                shift = schedule.Shift != null ? schedule.Shift.Code : string.Empty;
            }

            return shift;
        }

        private static string GetShiftOnLeaveDay(DailySchedule schedule)
        {
            var shift = string.Empty;
            if (schedule.IsHoliday)
            {
                shift = Labels.PH;
            }
            else if (schedule.IsOffDay)
            {
                shift = Labels.Off;
            }
            else
            {
                if (schedule.IsLeaveHalfDay)
                {
                    shift = string.Format("{0} ½", schedule.LeaveType.Code);
                }
                else
                {
                    shift = schedule.LeaveType.Code;

                    if (schedule.LeaveType2 != null && schedule.LeaveType.Code != schedule.LeaveType2.Code)
                    {
                        shift = string.Format("{0} / {1}", schedule.LeaveType.Code, schedule.LeaveType2.Code);
                    }
                }
            }

            return shift;
        }
    }
}