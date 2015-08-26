// --------------------------------------------------------------------------
// <copyright file="ExportExcelService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.9.20">Created</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.ExcelExporting;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Calendar;
    using Fanex.NexHR.ViewModels.Overtime;
    using Fanex.NexHR.ViewModels.Report;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// Export Excel Service.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
    public class ExportExcelService : IExportExcelService
    {
        private const int _RowStartBodyAttendance = 3;
        private const int _RowStartBodyLeaveSummary = 4;
        private const int _RowStartBodyWorkingTimeRequest = 3;
        private const int _RowStartBodyOvertimeReport = 4;
        private const int _RowStartBodyOvertimeRecord = 3;
        private const int _RowStartBodyWorkingCalendar = 5;
        private const int _RowStartBodyYearlyOvertimeReport = 5;
        
        private const string _AttendanceFilePathExcelTemplate = "~\\ExcelTemplates\\AttendanceReportTemplate.xls";
        private const string _LeaveSummaryFilePathExcelTemplate = "~\\ExcelTemplates\\LeaveSummaryTemplate.xls";
        private const string _WorkingTimeRequestFilePathExcelTemplate = "~\\ExcelTemplates\\WorkingTimeRequestTemplate.xls";
        private const string _OvertimeReportFilePathExcelTemplate = "~\\ExcelTemplates\\OvertimeReportTemplate.xls";
        private const string _YearlyOvertimeReportFilePathExcelTemplate = "~\\ExcelTemplates\\YearlyOvertimeReportTemplate.xls";
        private const string _EmployeeBasicInfoFilePathExcelTemplate = "~\\ExcelTemplates\\EmployeeBasicInfoTemplate.xls";
        private const string _EmployeeDetailsInfoFilePathExcelTemplate = "~\\ExcelTemplates\\EmployeeDetailsInfoTemplate.xls";
        private const string _EmployeeOvertimeRecordFilePathExcelTemplate = "~\\ExcelTemplates\\EmployeeOvertimeRecordTemplate.xls";
        private const string _EmployeeWorkingCalendarFilePathExcelTemplate = "~\\ExcelTemplates\\WorkingCalendarTemplate.xls";

        private const string _MsgLeave = "{1} ({0} days)";
        private const string _MsgLeaveHalfDay = "{0} ({1}-{2}) 0.5 day";
        private const string _MsgLeaveOneDay = "{0} (1 day)";
        private const string _MsgLeaveDuringWorking = "{0} ({1}-{2}) {3} minutes";
        private const string _MsgChangeShift = "{0} --> {1} ({2})";
        private const string _MsgCheckInOut = "{0} {1} ({2})";
        private const string _MsgLateIn = "+{0}' {1} ({2})";
        private const string _MsgEarlyOut = "-{0}' {1} ({2})";
        private const string _MsgOvertimeHours = "{0} ({1}-{2}) {3}' - {4}";
        private const string _MsgOvertimeShift = "{0} ({1}) {2}";
        private const string _MsgKeepInView = "{0} ({1})";

        /// <summary>
        /// Export excel total working hours report.
        /// </summary>
        /// <param name="listTotalWorkingHours">List total working hours object.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">The to date.</param>
        /// <returns>HSSF Workbook.</returns>
        public HSSFWorkbook ExportTotalWorkingHours(IList<TotalWorkingHours> listTotalWorkingHours, DateTime fromDate, DateTime toDate)
        {
            // Create a new workbook and a sheet named "User Accounts"
            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("Total Working Hours");
            var rowTitle = sheet.CreateRow(1);
            var titleString = "Total Working Hours: " + fromDate.ToString("dd MMM, yyyy") + " - " + toDate.ToString("dd MMM, yyyy");

            var font = workbook.CreateFont();
            font.FontHeightInPoints = 18;
            font.Boldweight = (short)FontBoldWeight.BOLD;

            var cell = rowTitle.CreateCell(1);
            cell.SetCellValue(titleString);
            cell.CellStyle = workbook.CreateCellStyle();
            cell.CellStyle.SetFont(font);
            sheet.AddMergedRegion(new CellRangeAddress(1, 1, 1, 12));

            // header
            var rowIndex = 3;
            var row = sheet.CreateRow(rowIndex);
            var font1 = workbook.CreateFont();
            font1.FontHeightInPoints = 11;
            font1.Color = HSSFColor.GREY_80_PERCENT.WHITE.index;
            font1.Boldweight = (short)FontBoldWeight.BOLD;

            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.FillForegroundColor = HSSFColor.GREY_80_PERCENT.index;
            cellStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
            cellStyle.FillBackgroundColor = HSSFColor.GREY_80_PERCENT.index;
            cellStyle.Alignment = HorizontalAlignment.CENTER;
            cellStyle.VerticalAlignment = VerticalAlignment.CENTER;
            cellStyle.SetFont(font1);

            var cell1 = row.CreateCell(0);
            cell1.CellStyle = cellStyle;
            cell1.SetCellValue("No.");

            var cell2 = row.CreateCell(1);
            cell2.CellStyle = cellStyle;
            cell2.SetCellValue(Labels.Code);

            var cell3 = row.CreateCell(2);
            cell3.CellStyle = cellStyle;
            cell3.SetCellValue(Labels.Name);

            var cell4 = row.CreateCell(3);
            cell4.CellStyle = cellStyle;
            cell4.SetCellValue(Labels.Nick);

            var cell5 = row.CreateCell(4);
            cell5.CellStyle = cellStyle;
            cell5.SetCellValue(Labels.Team);

            var cell6 = row.CreateCell(5);
            cell6.CellStyle = cellStyle;
            cell6.SetCellValue(Labels.WorkigDay);

            var cell7 = row.CreateCell(6);
            cell7.CellStyle = cellStyle;
            cell7.SetCellValue(Labels.ActualWorkingDay);

            var cell8 = row.CreateCell(7);
            cell8.CellStyle = cellStyle;
            cell8.SetCellValue("Leave Taken");

            var cell9 = row.CreateCell(8);
            cell9.CellStyle = cellStyle;
            cell9.SetCellValue("Unpaid Leave");

            var cell10 = row.CreateCell(9);
            cell10.CellStyle = cellStyle;
            cell10.SetCellValue(Labels.LateIn);

            var cell11 = row.CreateCell(10);
            cell11.CellStyle = cellStyle;
            cell11.SetCellValue(Labels.EarlyOut);

            var cell12 = row.CreateCell(11);
            cell12.CellStyle = cellStyle;
            cell12.SetCellValue(Labels.TotalHours);

            var cell13 = row.CreateCell(12);
            cell13.CellStyle = cellStyle;
            cell13.SetCellValue("Average Hours");
            //// end header

            // body
            rowIndex++;

            ICellStyle bodyStyle = workbook.CreateCellStyle();

            ICellStyle bodyStyle1 = workbook.CreateCellStyle();
            bodyStyle1.Alignment = HorizontalAlignment.CENTER;
            bodyStyle1.VerticalAlignment = VerticalAlignment.CENTER;

            ICellStyle bodyStyle2 = workbook.CreateCellStyle();
            bodyStyle2.Alignment = HorizontalAlignment.CENTER;
            bodyStyle2.VerticalAlignment = VerticalAlignment.CENTER;
            bodyStyle2.FillForegroundColor = HSSFColor.LIGHT_CORNFLOWER_BLUE.index;
            bodyStyle2.FillPattern = FillPatternType.SOLID_FOREGROUND;
            bodyStyle2.FillBackgroundColor = HSSFColor.LIGHT_CORNFLOWER_BLUE.index;

            ICellStyle bodyStyle3 = workbook.CreateCellStyle();
            bodyStyle3.Alignment = HorizontalAlignment.LEFT;

            ICellStyle bodyStyle4 = workbook.CreateCellStyle();
            bodyStyle4.Alignment = HorizontalAlignment.LEFT;
            bodyStyle4.FillForegroundColor = HSSFColor.LIGHT_CORNFLOWER_BLUE.index;
            bodyStyle4.FillPattern = FillPatternType.SOLID_FOREGROUND;
            bodyStyle4.FillBackgroundColor = HSSFColor.LIGHT_CORNFLOWER_BLUE.index;
            ICell cellBody;
            //// Add data rows
            foreach (var totalWorkingHours in listTotalWorkingHours)
            {
                if (rowIndex % 2 == 0)
                {
                    bodyStyle = bodyStyle2;
                }
                else
                {
                    bodyStyle = bodyStyle1;
                }

                row = sheet.CreateRow(rowIndex);
                cellBody = row.CreateCell(0);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(rowIndex - 3);

                cellBody = row.CreateCell(1);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.Employee.Code.Trim());

                bodyStyle3.Alignment = HorizontalAlignment.LEFT;
                cellBody = row.CreateCell(2);
                if (rowIndex % 2 == 0)
                {
                    cellBody.CellStyle = bodyStyle4;
                }
                else
                {
                    cellBody.CellStyle = bodyStyle3;
                }

                cellBody.SetCellValue(totalWorkingHours.Employee.FullName);

                // bodyStyle.Alignment = HorizontalAlignment.CENTER;
                cellBody = row.CreateCell(3);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.Employee.NickName);

                cellBody = row.CreateCell(4);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.Employee.Team.Name);

                cellBody = row.CreateCell(5);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.WorkingDay);

                cellBody = row.CreateCell(6);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.ActualWorkingDay);

                cellBody = row.CreateCell(7);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.LeaveTaken);

                cellBody = row.CreateCell(8);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.UnpaidLeave);

                cellBody = row.CreateCell(9);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.LateInMins);

                cellBody = row.CreateCell(10);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.EarlyOutMins);

                cellBody = row.CreateCell(11);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.TotalHours.ToString("#.##"));

                cellBody = row.CreateCell(12);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.AverageHours.ToString("#.##"));

                rowIndex++;
            }
            ////end body

            sheet.CreateFreezePane(0, 4);
            //// Auto-size each column
            for (var i = 0; i < sheet.GetRow(3).LastCellNum; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            return workbook;
        }

        /// <summary>
        /// Exports the total working hour monthly.
        /// </summary>
        /// <param name="totalWorkingHourMonthly">All items of the total working hour monthly.</param>
        /// <param name="firstRecord">The first record contains the header titles.</param>
        /// <returns>The instance of HSSFWorkbook supports for exporting excel file.</returns>
        public HSSFWorkbook ExportTotalWorkingHourMonthly(IList<TotalWorkingHourMonthly> totalWorkingHourMonthly, TotalWorkingHourMonthly firstRecord)
        {
            var totalWorkingHourMonthlyExporting = new TotalWorkingHourMonthlyExporting(totalWorkingHourMonthly, firstRecord);
            return totalWorkingHourMonthlyExporting.ExportTotalWorkingHourMonthly();
        }

        /// <summary>
        /// Method Export Total Working Calendar Summary.
        /// </summary>
        /// <param name="calendars">List of Working Calendar Entity.</param>
        /// <param name="fromDate">Export From Date.</param>
        /// <param name="toDate">Export To Date.</param>
        /// <returns>The instance of HSSFWorkbook supports for exporting excel file.</returns>
        public HSSFWorkbook ExportWorkingCalendarSummary(List<WorkingCalendarSummary> calendars, DateTime fromDate, DateTime toDate)
        {
            HSSFWorkbook hssfworkbook;
            var exportExcel = new ReportExporting();
            hssfworkbook = exportExcel.ExportWorkingCalendarSummary(calendars, fromDate, toDate);

            return hssfworkbook;
        }

        /// <summary>
        /// Method Export HR Request Statistic.
        /// </summary>
        /// <param name="requests">List of Request Monitoring Entity.</param>
        /// <param name="fromDate">Export From Date.</param>
        /// <param name="toDate">Export To Date.</param>
        /// <returns>The instance of HSSFWorkbook supports for exporting excel file.</returns>
        public HSSFWorkbook ExportHRRequestStatistic(List<RequestsMonitoring> requests, DateTime fromDate, DateTime toDate)
        {
            HSSFWorkbook hssfworkbook;
            var exportExcel = new ReportExporting();
            hssfworkbook = exportExcel.ExportHRRequestStatistic(requests, fromDate, toDate);

            return hssfworkbook;
        }

        public HSSFWorkbook ExportEmployeeSupervisor(List<SupervisorReportGroupModel> supervisors)
        {
            HSSFWorkbook hssfworkbook;
            var exportExcel = new ReportExporting();
            hssfworkbook = exportExcel.ExportEmployeeSupervisor(supervisors);

            return hssfworkbook;
        }

        public HSSFWorkbook ExportTeamSupervisor(List<TeamSupervisorReportModel> supervisors)
        {
            HSSFWorkbook hssfworkbook;
            var exportExcel = new ReportExporting();
            hssfworkbook = exportExcel.ExportTeamSupervisor(supervisors);

            return hssfworkbook;
        }

        /// <summary>
        /// Function to Export attendance report.
        /// </summary>
        /// <param name="attendanceList">List EmployeeAttendanceModel need export.</param>
        /// <param name="fromDate">From Date which list object is filter from it.</param>
        /// <param name="toDate">To Date which list object is filter from it.</param>
        /// <returns>HSSF Workbook.</returns>
        public HSSFWorkbook ExportAttendance(IList<EmployeeAttendanceModel> attendanceList, DateTime fromDate, DateTime toDate)
        {
            HSSFWorkbook hssfworkbook;
            var templatePath = HttpContext.Current.Server.MapPath(_AttendanceFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, System.IO.FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "AttendanceReport");

            //// render body
            var rowIndex = _RowStartBodyAttendance;
            var rowCopy = _RowStartBodyAttendance;
            var index = 1;
            var height = sheet.GetRow(_RowStartBodyAttendance).Height;

            foreach (var item in attendanceList)
            {
                var requests = string.Empty;
                rowCopy = _RowStartBodyAttendance + (index % 2);
                if (item.LateIn > 30 || item.EarlyOut > 30)
                {
                    rowCopy = _RowStartBodyAttendance + 2;
                }
                else
                {
                    if (item.IsWeekend)
                    {
                        rowCopy = 6 + (index % 2);
                    }
                    else
                    {
                        rowCopy = _RowStartBodyAttendance + (index % 2);
                    }
                }
                
                sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                var row = sheet.GetRow(rowIndex);
                row.Height = height;

                row.GetCell(0).SetCellValue(index);
                row.GetCell(1).SetCellValue(item.AttendanceDate.Value.ToDayOfWeekFormatedDate());
                row.GetCell(2).SetCellValue(item.Code);
                row.GetCell(3).SetCellValue(item.NickName);
                row.GetCell(4).SetCellValue(item.FullName);
                row.GetCell(5).SetCellValue(item.TeamCode);
                row.GetCell(6).SetCellValue(string.IsNullOrEmpty(item.ShiftCode) ?
                    string.Empty : item.ShiftCode);
                row.GetCell(7).SetCellValue(item.InFixed.HasValue ? item.InFixed.Value.ToShortTime() : string.Empty);
                row.GetCell(8).SetCellValue(item.OutFixed.HasValue ? item.OutFixed.Value.ToShortTime() : string.Empty);
                row.GetCell(9).SetCellValue(item.TotalWorkHours != 0 ? Utilities.TotalHoursToTime((float)item.TotalWorkHours) : string.Empty);
                row.GetCell(10).SetCellValue(Utilities.MinuteToTime(item.LateIn));
                row.GetCell(11).SetCellValue(Utilities.MinuteToTime(item.EarlyOut));
                row.GetCell(14).SetCellValue(item.Notes);

                if (item.RequestList.Count > 0)
                {
                    row.Height = (short)(height * item.RequestList.Count);

                    foreach (var request in item.RequestList)
                    {
                        requests += "- " + request.RequestType.Name + ": " + this.GetRequestDetail(request)
                            + (string.IsNullOrEmpty(request.Remark) ? string.Empty : " --> " + request.Remark) + "\r\n";
                    }

                    requests = requests.Substring(0, requests.Length - 2);
                    row.GetCell(12).SetCellValue(requests);
                }
                else
                {
                    row.Height = height;
                }

                if (item.IsOvertime)
                {
                    row.GetCell(13).SetCellValue(item.FormattedOvertimeStatus + " (" + item.OvertimeHours.ToString() + ")");
                }

                rowIndex++;
                index++;
            }

            //// remove sheet & row template
            hssfworkbook.RemoveSheetAt(0);

            return hssfworkbook;
        }

        /// <summary>
        /// Export Leave summary report.
        /// </summary>
        /// <param name="leaveSummaryList">Leave Summary List.</param>
        /// <param name="year">Year which query report.</param>
        /// <returns>HSSF Workbook.</returns>
        public HSSFWorkbook ExportLeaveSummary(IList<LeaveSummaryByTeam> leaveSummaryList, int year)
        {
            HSSFWorkbook hssfworkbook;

            var templatePath = HttpContext.Current.Server.MapPath(_LeaveSummaryFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, System.IO.FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "LeaveSummary");

            //// render hearder
            var rowTakeLeave = sheet.GetRow(2);
            var cellTakeLeave = rowTakeLeave.GetCell(5);
            cellTakeLeave.SetCellValue("Leave's Report " + year);

            var rowAnnualBalance = sheet.GetRow(2);
            var cellAnnualBalance = rowAnnualBalance.GetCell(17);
            cellAnnualBalance.SetCellValue("Balance Of AL " + year);

            var cellKIVBalance = rowAnnualBalance.GetCell(18);
            cellKIVBalance.SetCellValue("Balance Of KIV " + year);

            //// render body
            var rowIndex = _RowStartBodyLeaveSummary;
            var rowCopy = _RowStartBodyLeaveSummary;
            var height = sheet.GetRow(_RowStartBodyLeaveSummary).Height;
            var index = 1;

            foreach (var item in leaveSummaryList)
            {
                if (item.Team != null && item.LeaveSummaryInTeam.Count > 0)
                {
                    //// add row team
                    rowCopy = _RowStartBodyLeaveSummary + 2;
                    sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                    var rowTeam = sheet.GetRow(rowIndex);
                    var cellTeam = rowTeam.GetCell(0);
                    cellTeam.SetCellValue(item.Team.Name.ToUpper());
                    sheet.MergeCells(rowIndex, 0, rowIndex, 18);
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

        /// <summary>
        /// Export Employee Overtime Record.
        /// </summary>
        /// <param name="overtimes">List OvertimeExcelReport entity.</param>
        /// <param name="calendar">Fiscal or Normal calendar.</param>
        /// <param name="fromDate">Request from Date.</param>
        /// <param name="toDate">Request To Date.</param>
        /// <returns>HSSFWork book.</returns>
        public HSSFWorkbook ExportEmployeeOvertimeRecord(List<OvertimeExcelReport> overtimes, int? calendar, DateTime fromDate, DateTime toDate)
        {
            int totalDay = Convert.ToInt32((toDate - fromDate).TotalDays) + 1;
            HSSFWorkbook hssfworkbook;

            var templatePath = HttpContext.Current.Server.MapPath(_EmployeeOvertimeRecordFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, System.IO.FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "OvertimeRecord");

            //// render Header
            var rowHeaderTitle = sheet.GetRow(0);
            var cellHeader = rowHeaderTitle.GetCell(0);
            cellHeader.SetCellValue("OVERTIME RECORD - SMS STAFF Local From "
                + fromDate.ToString("dd MMM yyyy") + " - " + toDate.ToString("dd MMM yyyy"));

            var rowHeaderDate = sheet.GetRow(1);
            int totalHeaderDateCell = totalDay * 2;
            int rowHeaderCount = 0;
            for (int i = 1; i <= totalHeaderDateCell; i++)
            {
                if (i % 2 == 1)
                {
                    var displayDate = fromDate.Date.AddDays(rowHeaderCount);
                    rowHeaderDate.GetCell(i + 10).SetCellValue(displayDate.ToString("dd-MMM"));
                    rowHeaderCount++;
                }
            }

            if (overtimes != null && overtimes.Count() > 0)
            {
                //// render Overtime type Header.
                var rowHeaderOvertimeType = sheet.GetRow(2);
                var headerRowEntity = overtimes.FirstOrDefault();
                var overtimeTypeIndex = 0;
                for (int i = 1; i <= totalHeaderDateCell; i++)
                {
                    var data = headerRowEntity.OvertimeDailyReport[overtimeTypeIndex];
                    if (i % 2 == 0)
                    {
                        rowHeaderOvertimeType.GetCell(i + 10).SetCellValue(data.TotalOvertimeType2);
                        overtimeTypeIndex++;
                    }

                    if (i % 2 == 1)
                    {
                        rowHeaderOvertimeType.GetCell(i + 10).SetCellValue(data.TotalOvertimeType1);
                    }
                }

                //// render Body.
                overtimes.RemoveAll(i => i == headerRowEntity);
                var model = (from item in overtimes
                             group item by item.Team into g
                             select new OvertimeExcelReportViewModel() { Name = g.Key, ListOvertimeEntity = g.ToList() }).ToList();

                var index = 1;
                var rowIndex = _RowStartBodyOvertimeRecord;
                var rowCopy = _RowStartBodyOvertimeRecord;
                var rowTeamCopy = _RowStartBodyOvertimeRecord + 2;
                var rowTeamHeight = sheet.GetRow(rowTeamCopy).Height;
                var rowHeight = sheet.GetRow(_RowStartBodyOvertimeRecord).Height;
                var numberCount = 1;
                var numberStart = 0;
                foreach (var overtime in model)
                {
                    if (overtime.Name != null && overtime.ListOvertimeEntity.Count > 0)
                    {
                        if (rowIndex <= _RowStartBodyOvertimeRecord)
                        {
                            sheet.CopyRow(templateSheet, rowTeamCopy, rowIndex);
                        }

                        var rowTeam = sheet.GetRow(rowIndex);
                        rowTeam.Height = rowTeamHeight;
                        var cellTeam = rowTeam.GetCell(0);
                        cellTeam.SetCellValue(overtime.Name.ToUpper());
                        sheet.MergeCells(rowIndex, 0, rowIndex, 2);
                        rowIndex++;
                        numberStart = rowIndex + 1;
                        foreach (var item in overtime.ListOvertimeEntity)
                        {
                            rowCopy = _RowStartBodyOvertimeRecord + (index % 2);
                            sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                            var row = sheet.GetRow(rowIndex);
                            row.Height = rowHeight;
                            row.GetCell(0).SetCellValue(numberCount);
                            row.GetCell(1).SetCellValue(item.EmpCode);
                            row.GetCell(2).SetCellValue(item.Fullname);
                            row.GetCell(3).SetCellValue(item.Nickname);
                            row.GetCell(4).SetCellValue(item.Team);
                            row.GetCell(5).SetCellValue(item.TotalWorkDays);
                            row.GetCell(6).SetCellValue(item.UnpaidLeave);

                            row.GetCell(7).SetCellValue(item.Normal);
                            row.GetCell(8).SetCellValue(item.Offday);
                            row.GetCell(9).SetCellValue(item.Holiday);
                            row.GetCell(10).SetCellValue(item.Tax);

                            var indexCount = 0;
                            double defaultCellValue = 0.0;
                            for (int i = 1; i <= totalHeaderDateCell; i++)
                            {
                                var data = item.OvertimeDailyReport[indexCount];
                                if (i % 2 == 0)
                                {
                                    var cellValue = data.TotalOvertimeType2;
                                    if (string.IsNullOrEmpty(cellValue))
                                    {
                                        row.GetCell(i + 10).SetCellValue(defaultCellValue);
                                    }
                                    else
                                    {
                                        row.GetCell(i + 10).SetCellValue(Convert.ToDouble(cellValue));
                                    }

                                    indexCount++;
                                }

                                if (i % 2 == 1)
                                {
                                    var cellValue = data.TotalOvertimeType1;
                                    if (string.IsNullOrEmpty(cellValue))
                                    {
                                        row.GetCell(i + 10).SetCellValue(defaultCellValue);
                                    }
                                    else
                                    {
                                        row.GetCell(i + 10).SetCellValue(Convert.ToDouble(cellValue));
                                    }
                                }
                            }

                            rowIndex++;
                            index++;
                            numberCount++;
                        }

                        var cellCountMemberInTeam = rowTeam.GetCell(3);
                        cellCountMemberInTeam.SetCellValue(numberCount - 1);

                        sheet.CopyRow(templateSheet, rowTeamCopy, rowIndex);
                        var rowCalculateTotalOTOfTeam = sheet.GetRow(rowIndex);

                        var cellSumNormal = rowCalculateTotalOTOfTeam.GetCell(7);
                        var cellSumOff = rowCalculateTotalOTOfTeam.GetCell(8);
                        var cellSumPH = rowCalculateTotalOTOfTeam.GetCell(9);
                        var cellSumTax = rowCalculateTotalOTOfTeam.GetCell(10);
                        string formulaNormal = "SUM(H{0}:H{1})";
                        string formulaOff = "SUM(I{0}:I{1})";
                        string formulaPH = "SUM(J{0}:J{1})";
                        string formulaTax = "SUM(K{0}:K{1})";

                        string data1 = Convert.ToString(numberStart);
                        string data2 = Convert.ToString(rowIndex);
                        cellSumNormal.CellFormula = string.Format(formulaNormal, data1, data2);
                        cellSumOff.CellFormula = string.Format(formulaOff, data1, data2);
                        cellSumPH.CellFormula = string.Format(formulaPH, data1, data2);
                        cellSumTax.CellFormula = string.Format(formulaTax, data1, data2);
                        numberCount = 1;
                    }
                }
            }

            hssfworkbook.RemoveSheetAt(0);

            return hssfworkbook;
        }

        /// <summary>
        /// Export WorkingTime Request.
        /// </summary>
        /// <param name="requestList">Request List.</param>
        /// <returns>HSSF Workbook.</returns>
        public HSSFWorkbook ExportWorkingTimeRequest(IList<RequestEntity> requestList)
        {
            HSSFWorkbook hssfworkbook;
            var templatePath = HttpContext.Current.Server.MapPath(_WorkingTimeRequestFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, System.IO.FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "WorkingTimeRequest");
            var rowIndex = _RowStartBodyWorkingTimeRequest;
            var rowCopy = _RowStartBodyWorkingTimeRequest;
            var height = sheet.GetRow(_RowStartBodyWorkingTimeRequest).Height;
            ICell cellNo = null;
            ICell cellTeam = null;
            ICell cellNick = null;
            ICell cellName = null;
            ICell cellRequest = null;
            ICell cellDetails = null;
            ICell cellRemarks = null;
            ICell cellCreateDate = null;
            ICell cellStatus = null;

            foreach (var item in requestList)
            {
                rowCopy = _RowStartBodyWorkingTimeRequest + (rowIndex % 2);
                sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                var row = sheet.GetRow(rowIndex);
                row.Height = height;
                RenderBodyWorkingTimeRequest(
                    rowIndex,
                    ref cellNo,
                    ref cellTeam,
                    ref cellNick,
                    ref cellName,
                    ref cellRequest,
                    ref cellDetails,
                    ref cellRemarks,
                    ref cellCreateDate,
                    ref cellStatus,
                    item,
                    row);
                rowIndex++;
            }

            hssfworkbook.RemoveSheetAt(0);

            return hssfworkbook;
        }

        /// <summary>
        /// Export Overtime Report.
        /// </summary>
        /// <param name="requestList">OvertimeRequest List.</param>
        /// <param name="fromDate">Report FromDate.</param>
        /// <param name="toDate">Report ToDate.</param>
        /// <returns>HSSF Workbook.</returns>
        public HSSFWorkbook ExportOvertimeReport(IList<OvertimeRequest> requestList, DateTime fromDate, DateTime toDate)
        {
            HSSFWorkbook hssfworkbook;
            var templatePath = HttpContext.Current.Server.MapPath(_OvertimeReportFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, System.IO.FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "OvertimeReport");
            var rowIndex = _RowStartBodyOvertimeReport;
            var rowCopy = _RowStartBodyOvertimeReport;
            var rowCopyDate = _RowStartBodyOvertimeReport - 3;

            var height = sheet.GetRow(_RowStartBodyOvertimeReport).Height;
            ICell cellFromDate = null;
            ICell cellToDate = null;
            ICell cellNo = null;
            ICell cellDate = null;
            ICell cellEmployee = null;
            ICell cellTeam = null;
            ICell cellShift = null;
            ICell cellTimeIn = null;
            ICell cellTimeOut = null;
            ICell cellTotalHours = null;
            ICell cellNormalDay = null;
            ICell cellOffDay = null;
            ICell cellPublicHoliday = null;
            ICell cellRemark = null;
            ICell cellStatus = null;

            sheet.CopyRow(templateSheet, rowCopyDate, rowCopyDate);
            var rowDate = sheet.GetRow(rowCopyDate);
            rowDate.Height = height;
            cellFromDate = rowDate.GetCell(2);
            cellToDate = rowDate.GetCell(4);
            cellFromDate.SetCellValue(fromDate.ToString("dd-MMM-yyyy"));
            cellToDate.SetCellValue(toDate.ToString("dd-MMM-yyyy"));
            var index = 1;

            foreach (var item in requestList)
            {
                rowCopy = _RowStartBodyOvertimeReport + (index % 2);
                sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                var row = sheet.GetRow(rowIndex);
                row.Height = height;
                RenderBodyOvertimeReport(
                    rowIndex - 1,
                    ref cellNo,
                    ref cellDate,
                    ref cellEmployee,
                    ref cellTeam,
                    ref cellShift,
                    ref cellTimeIn,
                    ref cellTimeOut,
                    ref cellTotalHours,
                    ref cellNormalDay,
                    ref cellOffDay,
                    ref cellPublicHoliday,
                    ref cellRemark,
                    ref cellStatus,
                    item,
                    row);
                rowIndex++;
                index++;
            }

            hssfworkbook.RemoveSheetAt(0);

            return hssfworkbook;
        }

        /// <summary>
        /// Method Export Yearly Overtime Report.
        /// </summary>
        /// <param name="requestList">OvertimeAnnualReport Entities.</param>
        /// <param name="year">Year to export.</param>
        /// <param name="calendar">Calendar type.</param>
        /// <returns>HSSF Workbook.</returns>
        public HSSFWorkbook ExportYearlyOvertimeReport(IList<OvertimeAnnualReportViewModel> requestList, int year, int? calendar)
        {
            HSSFWorkbook hssfworkbook;
            var templatePath = HttpContext.Current.Server.MapPath(_YearlyOvertimeReportFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, System.IO.FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "YearlyOvertimeReport");
            var rowIndex = _RowStartBodyYearlyOvertimeReport;
            var rowCopy = _RowStartBodyYearlyOvertimeReport;

            string calendarType = string.Empty;
            int index = 1;
            float totalYearlyOTHours = 0;
            var height = sheet.GetRow(_RowStartBodyOvertimeReport).Height;

            if (calendar == 1)
            {
                calendarType = "Fiscal Calendar";
            }
            else
            {
                calendarType = "Normal Calendar";
            }

            // Header
            var rowHeader = sheet.GetRow(3);
            rowHeader.Height = height;
            var cellYear = rowHeader.GetCell(4);
            cellYear.SetCellValue("Overtime Summary In " + year + " (" + calendarType + ")");

            foreach (var item in requestList)
            {
                rowCopy = _RowStartBodyYearlyOvertimeReport + (index % 2);
                sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                var row = sheet.GetRow(rowIndex);
                row.Height = height;
                float totalHours = 0;

                row.GetCell(0).SetCellValue(index);
                row.GetCell(1).SetCellValue(item.Team);
                row.GetCell(2).SetCellValue(item.FullName);
                row.GetCell(3).SetCellValue(item.NickName);

                row.GetCell(4).SetCellValue(item.Jan == 0 ? string.Empty : item.Jan.ToString());
                row.GetCell(5).SetCellValue(item.Feb == 0 ? string.Empty : item.Feb.ToString());
                row.GetCell(6).SetCellValue(item.Mar == 0 ? string.Empty : item.Mar.ToString());
                row.GetCell(7).SetCellValue(item.Apr == 0 ? string.Empty : item.Apr.ToString());
                row.GetCell(8).SetCellValue(item.May == 0 ? string.Empty : item.May.ToString());
                row.GetCell(9).SetCellValue(item.Jun == 0 ? string.Empty : item.Jun.ToString());
                row.GetCell(10).SetCellValue(item.Jul == 0 ? string.Empty : item.Jul.ToString());
                row.GetCell(11).SetCellValue(item.Aug == 0 ? string.Empty : item.Aug.ToString());
                row.GetCell(12).SetCellValue(item.Sep == 0 ? string.Empty : item.Sep.ToString());
                row.GetCell(13).SetCellValue(item.Oct == 0 ? string.Empty : item.Oct.ToString());
                row.GetCell(14).SetCellValue(item.Nov == 0 ? string.Empty : item.Nov.ToString());
                row.GetCell(15).SetCellValue(item.Dec == 0 ? string.Empty : item.Dec.ToString());

                totalHours += item.Jan + item.Feb + item.Mar + item.Apr + item.May + item.Jun + item.Jun + item.Aug + item.Sep
                    + item.Oct + item.Oct + item.Nov + item.Dec;

                row.GetCell(16).SetCellValue(totalHours);
                totalYearlyOTHours += totalHours;
                index++;
                rowIndex++;
            }

            //// Footer
            sheet.CopyRow(templateSheet, _RowStartBodyYearlyOvertimeReport + 2, rowIndex);
            var rowFooter = sheet.GetRow(rowIndex);
            rowFooter.Height = height;

            RenderFooterForOvertimeReport(requestList, totalYearlyOTHours, rowFooter);

            hssfworkbook.RemoveSheetAt(0);

            return hssfworkbook;
        }

        /// <summary>
        /// Method ExportEmployeeBasicInfo.
        /// </summary>
        /// <param name="employees">List of Employee entity.</param>
        /// <returns>HSSF Workbook.</returns>
        public HSSFWorkbook ExportEmployeeBasicInfo(IList<Employee> employees)
        {
            HSSFWorkbook hssfworkbook;
            var templatePath = HttpContext.Current.Server.MapPath(_EmployeeBasicInfoFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, System.IO.FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "EmployeeBasicInfo");
            var rowIndex = _RowStartBodyOvertimeReport;
            var rowCopy = _RowStartBodyOvertimeReport;

            var height = sheet.GetRow(_RowStartBodyOvertimeReport).Height;
            ICell cellNo = null;
            ICell cellEmployeeID = null;
            ICell cellFullName = null;
            ICell cellNickName = null;
            ICell cellTeam = null;
            ICell cellGroupDate = null;
            ICell cellCompanyDate = null;
            ICell cellConfirmedDate = null;
            ICell cellResignedDate = null;
            ICell cellDOB = null;
            ICell cellGender = null;
            ICell cellCellPhone = null;
            ICell cellEmail = null;
            ICell cellCurrentAddress = null;
            ICell cellUnit = null;
            ICell cellCategory = null;
            var index = 1;

            foreach (var item in employees)
            {
                rowCopy = _RowStartBodyOvertimeReport + (index % 2);
                sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                var row = sheet.GetRow(rowIndex);
                row.Height = height;
                RenderBodyEmployeeBasicInfo(
                    rowIndex - 1,
                    ref cellNo,
                    ref cellEmployeeID,
                    ref cellFullName,
                    ref cellNickName,
                    ref cellTeam,
                    ref cellGroupDate,
                    ref cellCompanyDate,
                    ref cellConfirmedDate,
                    ref cellResignedDate,
                    ref cellDOB,
                    ref cellGender,
                    ref cellCellPhone,
                    ref cellEmail,
                    ref cellCurrentAddress,
                    ref cellUnit,
                    ref cellCategory,
                    item,
                    row);
                rowIndex++;
                index++;
            }

            hssfworkbook.RemoveSheetAt(0);

            return hssfworkbook;
        }

        /// <summary>
        /// Method Export EmployeeDetailsInfo.
        /// </summary>
        /// <param name="employees">List of Employee info.</param>
        /// <returns>HSSF Workbook.</returns>
        public HSSFWorkbook ExportEmployeeDetailsInfo(IList<Employee> employees)
        {
            HSSFWorkbook hssfworkbook;
            var templatePath = HttpContext.Current.Server.MapPath(_EmployeeDetailsInfoFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, System.IO.FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "EmployeeDetailsInfo");
            var rowIndex = _RowStartBodyOvertimeReport;
            var rowCopy = _RowStartBodyOvertimeReport;
            var height = sheet.GetRow(_RowStartBodyOvertimeReport).Height;

            ICell cellNo = null;
            ICell cellEmployeeID = null;
            ICell cellDomainUserName = null;
            ICell cellFullName = null;
            ICell cellNickName = null;
            ICell cellTeam = null;
            ICell cellDOB = null;
            ICell cellPlaceOfBirth = null;
            ICell cellEthnic = null;
            ICell cellNative = null;

            ICell cellReligion = null;
            ICell cellGender = null;
            ICell cellMarriage = null;
            ICell cellNationality = null;
            ICell cellIDCardNo = null;
            ICell cellIDCardIssueDate = null;
            ICell cellIDCardPlaceIssue = null;
            ICell cellPassportNo = null;
            ICell cellPassportExp = null;
            ICell cellQualification = null;

            ICell cellMajor = null;
            ICell cellCellPhone = null;
            ICell cellHomePhone = null;
            ICell cellEmail = null;
            ICell cellPermanentAddress = null;
            ICell cellCurrentAddress = null;
            ICell cellSkype = null;
            ICell cellUnit = null;
            ICell cellJobTitle = null;
            ICell cellGroupDate = null;

            ICell cellCompanyDate = null;
            ICell cellConfirmedDate = null;
            ICell cellResignedDate = null;
            ICell cellWorkingDays = null;
            ICell cellWorkingHours = null;
            ICell cellNumberOffdays = null;
            ICell cellLeaveBalance = null;
            ICell cellCategory = null;
            ICell cellBankAccount = null;
            ICell cellTaxCode = null;
            ICell cellSocialInsuranceNo = null;
            ICell cellRegisteredHospital = null;

            var index = 1;
            foreach (var item in employees)
            {
                rowCopy = _RowStartBodyOvertimeReport + (index % 2);
                sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                var row = sheet.GetRow(rowIndex);
                row.Height = height;
                RenderBodyEmployeeDetailsInfo(
                    rowIndex - 1,
                    ref cellNo,
                    ref cellEmployeeID,
                    ref cellDomainUserName,
                    ref cellFullName,
                    ref cellNickName,
                    ref cellTeam,
                    ref cellDOB,
                    ref cellPlaceOfBirth,
                    ref cellEthnic,
                    ref cellNative,
                    ref cellReligion,
                    ref cellGender,
                    ref cellMarriage,
                    ref cellNationality,
                    ref cellIDCardNo,
                    ref cellIDCardIssueDate,
                    ref cellIDCardPlaceIssue,
                    ref cellPassportNo,
                    ref cellPassportExp,
                    ref cellQualification,
                    ref cellMajor,
                    ref cellCellPhone,
                    ref cellHomePhone,
                    ref cellEmail,
                    ref cellPermanentAddress,
                    ref cellCurrentAddress,
                    ref cellSkype,
                    ref cellUnit,
                    ref cellJobTitle,
                    ref cellGroupDate,
                    ref cellCompanyDate,
                    ref cellConfirmedDate,
                    ref cellResignedDate,
                    ref cellWorkingDays,
                    ref cellWorkingHours,
                    ref cellNumberOffdays,
                    ref cellLeaveBalance,
                    ref cellCategory,
                    ref cellBankAccount,
                    ref cellTaxCode,
                    ref cellSocialInsuranceNo,
                    ref cellRegisteredHospital,
                    item,
                    row);
                rowIndex++;
                index++;
            }

            hssfworkbook.RemoveSheetAt(0);

            return hssfworkbook;
        }

        /// <summary>
        /// Method Export Employee's Working Calendar.
        /// </summary>
        /// <param name="workingCalendars">List WorkingCalendarsViewModel.</param>
        /// <param name="fromDate">Export From Date.</param>
        /// <param name="toDate">Export To Date.</param>
        /// <returns>HSSF Workbook.</returns>
        public HSSFWorkbook ExportEmployeeWorkingCalendar(IList<WorkingCalendarsViewModel> workingCalendars, DateTime fromDate, DateTime toDate)
        {
            HSSFWorkbook hssfworkbook;
            var templatePath = HttpContext.Current.Server.MapPath(_EmployeeWorkingCalendarFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, System.IO.FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "WorkingCalendar");
            var rowIndex = _RowStartBodyWorkingCalendar;
            var rowCopy = _RowStartBodyWorkingCalendar;

            ICell cellFromDate = null;
            ICell cellToDate = null;
            ICell cellNo = null;
            ICell cellEmployeeID = null;
            ICell cellFullName = null;
            ICell cellNickName = null;
            ICell cellTeam = null;
            ICell cellShift1 = null;
            ICell cellShift2 = null;
            ICell cellShift3 = null;
            ICell cellShift4 = null;
            ICell cellShift5 = null;
            ICell cellShift6 = null;
            ICell cellShift7 = null;

            var rowDate = sheet.GetRow(1);
            cellFromDate = rowDate.GetCell(2);
            cellToDate = rowDate.GetCell(4);
            cellFromDate.SetCellValue(fromDate.ToString("dd MMM, yyyy"));
            cellToDate.SetCellValue(toDate.ToString("dd MMM, yyyy"));

            var rowHeaderDate = sheet.GetRow(3);
            rowHeaderDate.GetCell(5).SetCellValue(fromDate.ToString("dd MMM, yyyy"));
            rowHeaderDate.GetCell(6).SetCellValue(fromDate.AddDays(1).ToString("dd MMM, yyyy"));
            rowHeaderDate.GetCell(7).SetCellValue(fromDate.AddDays(2).ToString("dd MMM, yyyy"));
            rowHeaderDate.GetCell(8).SetCellValue(fromDate.AddDays(3).ToString("dd MMM, yyyy"));
            rowHeaderDate.GetCell(9).SetCellValue(fromDate.AddDays(4).ToString("dd MMM, yyyy"));
            rowHeaderDate.GetCell(10).SetCellValue(fromDate.AddDays(5).ToString("dd MMM, yyyy"));
            rowHeaderDate.GetCell(11).SetCellValue(toDate.ToString("dd MMM, yyyy"));

            var rowHeaderDay = sheet.GetRow(4);
            rowHeaderDay.GetCell(5).SetCellValue(fromDate.ToString("ddd"));
            rowHeaderDay.GetCell(6).SetCellValue(fromDate.AddDays(1).ToString("ddd"));
            rowHeaderDay.GetCell(7).SetCellValue(fromDate.AddDays(2).ToString("ddd"));
            rowHeaderDay.GetCell(8).SetCellValue(fromDate.AddDays(3).ToString("ddd"));
            rowHeaderDay.GetCell(9).SetCellValue(fromDate.AddDays(4).ToString("ddd"));
            rowHeaderDay.GetCell(10).SetCellValue(fromDate.AddDays(5).ToString("ddd"));
            rowHeaderDay.GetCell(11).SetCellValue(toDate.ToString("ddd"));

            var index = 1;
            foreach (var item in workingCalendars)
            {
                rowCopy = _RowStartBodyWorkingCalendar + (index % 2);
                sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                var row = sheet.GetRow(rowIndex);
                cellNo = row.GetCell(0);
                cellEmployeeID = row.GetCell(1);
                cellFullName = row.GetCell(2);
                cellNickName = row.GetCell(3);
                cellTeam = row.GetCell(4);
                cellShift1 = row.GetCell(5);
                cellShift2 = row.GetCell(6);
                cellShift3 = row.GetCell(7);
                cellShift4 = row.GetCell(8);
                cellShift5 = row.GetCell(9);
                cellShift6 = row.GetCell(10);
                cellShift7 = row.GetCell(11);

                cellNo.SetCellValue(rowIndex - _RowStartBodyWorkingCalendar + 1);
                cellEmployeeID.SetCellValue(item.EmpCode);
                cellFullName.SetCellValue(item.FullName);
                cellNickName.SetCellValue(item.NickName);
                cellTeam.SetCellValue(item.Team);
                cellShift1.SetCellValue(item.WorkingSchedule[0].Title);
                cellShift2.SetCellValue(item.WorkingSchedule[1].Title);
                cellShift3.SetCellValue(item.WorkingSchedule[2].Title);
                cellShift4.SetCellValue(item.WorkingSchedule[3].Title);
                cellShift5.SetCellValue(item.WorkingSchedule[4].Title);
                cellShift6.SetCellValue(item.WorkingSchedule[5].Title);
                cellShift7.SetCellValue(item.WorkingSchedule[6].Title);
                rowIndex++;
                index++;
            }

            hssfworkbook.RemoveSheetAt(0);

            return hssfworkbook;
        }

        /// <summary>
        /// Function render footer for overtime report.
        /// </summary>
        /// <param name="requestList">Request List.</param>
        /// <param name="totalYearlyOTHours">Total Yearly OTHours.</param>
        /// <param name="rowFooter">Row Footer.</param>
        private static void RenderFooterForOvertimeReport(IList<OvertimeAnnualReportViewModel> requestList, float totalYearlyOTHours, IRow rowFooter)
        {
            rowFooter.GetCell(4).SetCellValue(requestList.Sum(x => x.Jan));
            rowFooter.GetCell(5).SetCellValue(requestList.Sum(x => x.Feb));
            rowFooter.GetCell(6).SetCellValue(requestList.Sum(x => x.Mar));
            rowFooter.GetCell(7).SetCellValue(requestList.Sum(x => x.Apr));
            rowFooter.GetCell(8).SetCellValue(requestList.Sum(x => x.May));
            rowFooter.GetCell(9).SetCellValue(requestList.Sum(x => x.Jun));
            rowFooter.GetCell(10).SetCellValue(requestList.Sum(x => x.Jul));
            rowFooter.GetCell(11).SetCellValue(requestList.Sum(x => x.Aug));
            rowFooter.GetCell(12).SetCellValue(requestList.Sum(x => x.Sep));
            rowFooter.GetCell(13).SetCellValue(requestList.Sum(x => x.Oct));
            rowFooter.GetCell(14).SetCellValue(requestList.Sum(x => x.Nov));
            rowFooter.GetCell(15).SetCellValue(requestList.Sum(x => x.Dec));
            rowFooter.GetCell(16).SetCellValue(totalYearlyOTHours);
        }

        /// <summary>
        /// Method RenderBodyEmployeeDetailsInfo.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="cellNo">Cell Number.</param>
        /// <param name="cellEmployeeID">Cell Employee Id.</param>
        /// <param name="cellDomainUserName">Cell Domain user name.</param>
        /// <param name="cellFullName">Cell Full Name.</param>
        /// <param name="cellNickName">Cell Nick Name.</param>
        /// <param name="cellTeam">Cell Team.</param>
        /// <param name="cellDOB">Cell Day of Birth.</param>
        /// <param name="cellPlaceOfBirth">Cell Place of birth.</param>
        /// <param name="cellEthnic">Cell Ethnic.</param>
        /// <param name="cellNative">Cell Native.</param>
        /// <param name="cellReligion">Cell Religion.</param>
        /// <param name="cellGender">Cell Gender.</param>
        /// <param name="cellMarriage">Cell Marriage.</param>
        /// <param name="cellNationality">Cell Nationality.</param>
        /// <param name="cellIDCardNo">Cell Id Card No.</param>
        /// <param name="cellIDCardIssueDate">Cell ID Card Issued Date.</param>
        /// <param name="cellIDCardPlaceIssue">Cell ID Card Place Of Issue.</param>
        /// <param name="cellPassportNo">Cell Passport No.</param>
        /// <param name="cellPassportExp">Cell Passport Expired.</param>
        /// <param name="cellQualification">Cell Qualification.</param>
        /// <param name="cellMajor">Cell major.</param>
        /// <param name="cellCellPhone">Cell CellPhone.</param>
        /// <param name="cellHomePhone">Cell Home Phone.</param>
        /// <param name="cellEmail">Cell Email.</param>
        /// <param name="cellPermanentAddress">Cell permanent address.</param>
        /// <param name="cellCurrentAddress">Cell current address.</param>
        /// <param name="cellSkype">Cell skype.</param>
        /// <param name="cellUnit">Cell unit.</param>
        /// <param name="cellJobTitle">Cell job title.</param>
        /// <param name="cellGroupDate">Cell group date.</param>
        /// <param name="cellCompanyDate">Cell company date.</param>
        /// <param name="cellConfirmedDate">Cell confirmed Date.</param>
        /// <param name="cellResignedDate">Cell Resigned Date.</param>
        /// <param name="cellWorkingDays">Cell Working days per month.</param>
        /// <param name="cellWorkingHours">Cell working hours per day.</param>
        /// <param name="cellNumberOffdays">Cell Number of Off day.</param>
        /// <param name="cellLeaveBalance">Cell Leave Balance.</param>
        /// <param name="cellCategory">Cell Category.</param>
        /// <param name="cellBankAccount">Cell Bank Account.</param>
        /// <param name="cellTaxCode">Cell Tax Code.</param>
        /// <param name="cellSocialInsuranceNo">Cell Social Insurance No.</param>
        /// <param name="cellRegisteredHospital">Cell Registered Hospital.</param>
        /// <param name="item">Employee entity.</param>
        /// <param name="row">Row to render data.</param>
        private void RenderBodyEmployeeDetailsInfo(
            int rowIndex,
            ref ICell cellNo,
            ref ICell cellEmployeeID,
            ref ICell cellDomainUserName,
            ref ICell cellFullName,
            ref ICell cellNickName,
            ref ICell cellTeam,
            ref ICell cellDOB,
            ref ICell cellPlaceOfBirth,
            ref ICell cellEthnic,
            ref ICell cellNative,
            ref ICell cellReligion,
            ref ICell cellGender,
            ref ICell cellMarriage,
            ref ICell cellNationality,
            ref ICell cellIDCardNo,
            ref ICell cellIDCardIssueDate,
            ref ICell cellIDCardPlaceIssue,
            ref ICell cellPassportNo,
            ref ICell cellPassportExp,
            ref ICell cellQualification,
            ref ICell cellMajor,
            ref ICell cellCellPhone,
            ref ICell cellHomePhone,
            ref ICell cellEmail,
            ref ICell cellPermanentAddress,
            ref ICell cellCurrentAddress,
            ref ICell cellSkype,
            ref ICell cellUnit,
            ref ICell cellJobTitle,
            ref ICell cellGroupDate,
            ref ICell cellCompanyDate,
            ref ICell cellConfirmedDate,
            ref ICell cellResignedDate,
            ref ICell cellWorkingDays,
            ref ICell cellWorkingHours,
            ref ICell cellNumberOffdays,
            ref ICell cellLeaveBalance,
            ref ICell cellCategory,
            ref ICell cellBankAccount,
            ref ICell cellTaxCode,
            ref ICell cellSocialInsuranceNo,
            ref ICell cellRegisteredHospital,
            Employee item,
            IRow row)
        {
            cellNo = row.GetCell(0);
            cellEmployeeID = row.GetCell(1);
            cellDomainUserName = row.GetCell(2);
            cellFullName = row.GetCell(3);
            cellNickName = row.GetCell(4);
            cellTeam = row.GetCell(5);
            cellDOB = row.GetCell(6);
            cellPlaceOfBirth = row.GetCell(7);
            cellEthnic = row.GetCell(8);
            cellNative = row.GetCell(9);
            cellReligion = row.GetCell(10);

            cellGender = row.GetCell(11);
            cellMarriage = row.GetCell(12);
            cellNationality = row.GetCell(13);
            cellIDCardNo = row.GetCell(14);
            cellIDCardIssueDate = row.GetCell(15);
            cellIDCardPlaceIssue = row.GetCell(16);
            cellPassportNo = row.GetCell(17);
            cellPassportExp = row.GetCell(18);
            cellQualification = row.GetCell(19);
            cellMajor = row.GetCell(20);

            cellCellPhone = row.GetCell(21);
            cellHomePhone = row.GetCell(22);
            cellEmail = row.GetCell(23);
            cellPermanentAddress = row.GetCell(24);
            cellCurrentAddress = row.GetCell(25);
            cellSkype = row.GetCell(26);
            cellUnit = row.GetCell(27);
            cellJobTitle = row.GetCell(28);
            cellGroupDate = row.GetCell(29);
            cellCompanyDate = row.GetCell(30);

            cellConfirmedDate = row.GetCell(31);
            cellResignedDate = row.GetCell(32);
            cellWorkingDays = row.GetCell(33);
            cellWorkingHours = row.GetCell(34);
            cellNumberOffdays = row.GetCell(35);
            cellLeaveBalance = row.GetCell(36);
            cellCategory = row.GetCell(37);
            cellBankAccount = row.GetCell(38);
            cellTaxCode = row.GetCell(39);
            cellSocialInsuranceNo = row.GetCell(40);
            cellRegisteredHospital = row.GetCell(41);

            string gender = string.Empty;
            string category = string.Empty;

            if (item.Gender == 0)
            {
                gender = "F";
            }
            else
            {
                gender = "M";
            }

            if (item.IsLocal)
            {
                category = "Local";
            }
            else
            {
                category = "Expat";
            }

            cellNo.SetCellValue(rowIndex - _RowStartBodyLeaveSummary + 2);
            cellEmployeeID.SetCellValue(item.Code);
            cellDomainUserName.SetCellValue(item.DomainUsername);
            cellFullName.SetCellValue(item.FullName);
            cellNickName.SetCellValue(item.NickName);
            cellTeam.SetCellValue(item.Team.Name);
            cellDOB.SetCellValue(item.DateOfBirth.ToString("dd MMM, yyyy"));
            cellPlaceOfBirth.SetCellValue(item.PlaceOfBirth);
            cellEthnic.SetCellValue(item.Ethnic);
            cellNative.SetCellValue(item.Native);
            cellReligion.SetCellValue(item.Religion);

            cellGender.SetCellValue(gender);
            cellMarriage.SetCellValue(item.MarriageName);
            cellNationality.SetCellValue(item.CountryName);
            cellIDCardNo.SetCellValue(item.CardNo);
            cellIDCardIssueDate.SetCellValue(item.DateOfIdCard != null ? item.DateOfIdCard.Value.ToString("dd MMM, yyyy") : string.Empty);
            cellIDCardPlaceIssue.SetCellValue(item.PlaceOfIdCard);
            cellPassportNo.SetCellValue(item.PassportNo);
            cellPassportExp.SetCellValue(item.PassportExp != null ? item.PassportExp.Value.ToString("dd MMM, yyyy") : string.Empty);
            cellQualification.SetCellValue(item.Qualification);
            cellMajor.SetCellValue(item.Major);

            cellCellPhone.SetCellValue(item.Phone);
            cellHomePhone.SetCellValue(item.HomePhone);
            cellEmail.SetCellValue(item.Email);
            cellPermanentAddress.SetCellValue(item.Address);
            cellCurrentAddress.SetCellValue(item.CurrentAddress);
            cellSkype.SetCellValue(item.Skype);
            cellUnit.SetCellValue(item.Unit);
            cellJobTitle.SetCellValue(item.JobTitle);
            cellGroupDate.SetCellValue(item.JoinedDate != null ? item.JoinedDate.Value.ToString("dd MMM, yyyy") : string.Empty);
            cellCompanyDate.SetCellValue(item.CompanyDate != null ? item.CompanyDate.Value.ToString("dd MMM, yyyy") : string.Empty);
            cellConfirmedDate.SetCellValue(item.ConfirmedDate != null ? item.ConfirmedDate.Value.ToString("dd MMM, yyyy") : string.Empty);
            cellResignedDate.SetCellValue(item.ResignedDate != null ? item.ResignedDate.Value.ToString("dd MMM, yyyy") : string.Empty);
            cellWorkingDays.SetCellValue(item.WorkingDays);

            cellWorkingHours.SetCellValue(item.WorkingHours);
            cellNumberOffdays.SetCellValue(item.OffDaysbyWeek);
            cellLeaveBalance.SetCellValue(item.AnnualLeaveBalance);
            cellCategory.SetCellValue(category);
            cellBankAccount.SetCellValue(item.BankAccount);
            cellTaxCode.SetCellValue(item.TaxCode);
            cellSocialInsuranceNo.SetCellValue(item.SocialInsuranceNo);
            cellRegisteredHospital.SetCellValue(item.RegisteredHospital);
        }

        /// <summary>
        /// Method RenderBodyEmployeeBasicInfo.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="cellNo">Cell number.</param>
        /// <param name="cellEmployeeID">Cell Employee ID.</param>
        /// <param name="cellFullName">Cell Full name.</param>
        /// <param name="cellNickName">Cell Nick name.</param>
        /// <param name="cellTeam">Cell Team / Department.</param>
        /// <param name="cellGroupDate">Cell GroupDate.</param>
        /// <param name="cellCompanyDate">Cell CompanyDate.</param>
        /// <param name="cellConfirmedDate">Cell ConfirmedDate.</param>
        /// <param name="cellResignedDate">Cell ResignedDate.</param>
        /// <param name="cellDOB">Cell Day of birth.</param>
        /// <param name="cellGender">Cell Gender.</param>
        /// <param name="cellCellPhone">Cell CellPhone.</param>
        /// <param name="cellEmail">Cell Email.</param>
        /// <param name="cellCurrentAddress">Cell CurrentAddress.</param>
        /// <param name="cellUnit">Cell Unit.</param>
        /// <param name="cellCategory">Cell Category.</param>
        /// <param name="item">Employee entity.</param>
        /// <param name="row">Row to render data.</param>
        private void RenderBodyEmployeeBasicInfo(
            int rowIndex,
            ref ICell cellNo,
            ref ICell cellEmployeeID,
            ref ICell cellFullName,
            ref ICell cellNickName,
            ref ICell cellTeam,
            ref ICell cellGroupDate,
            ref ICell cellCompanyDate,
            ref ICell cellConfirmedDate,
            ref ICell cellResignedDate,
            ref ICell cellDOB,
            ref ICell cellGender,
            ref ICell cellCellPhone,
            ref ICell cellEmail,
            ref ICell cellCurrentAddress,
            ref ICell cellUnit,
            ref ICell cellCategory,
            Employee item,
            IRow row)
        {
            cellNo = row.GetCell(0);
            cellEmployeeID = row.GetCell(1);
            cellFullName = row.GetCell(2);
            cellNickName = row.GetCell(3);
            cellTeam = row.GetCell(4);
            cellGroupDate = row.GetCell(5);
            cellCompanyDate = row.GetCell(6);
            cellConfirmedDate = row.GetCell(7);
            cellResignedDate = row.GetCell(8);
            cellDOB = row.GetCell(9);
            cellGender = row.GetCell(10);
            cellCellPhone = row.GetCell(11);
            cellEmail = row.GetCell(12);
            cellCurrentAddress = row.GetCell(13);
            cellUnit = row.GetCell(14);
            cellCategory = row.GetCell(15);

            string gender = string.Empty;
            string category = string.Empty;

            if (item.Gender == 0)
            {
                gender = "F";
            }
            else
            {
                gender = "M";
            }

            if (item.IsLocal)
            {
                category = "Local";
            }
            else
            {
                category = "Expat";
            }

            cellNo.SetCellValue(rowIndex - _RowStartBodyLeaveSummary + 2);
            cellEmployeeID.SetCellValue(item.Code);
            cellFullName.SetCellValue(item.FullName);
            cellNickName.SetCellValue(item.NickName);
            cellTeam.SetCellValue(item.Team.Name);
            cellGroupDate.SetCellValue(item.JoinedDate != null ? item.JoinedDate.Value.ToString("dd MMM, yyyy") : string.Empty);
            cellCompanyDate.SetCellValue(item.CompanyDate != null ? item.CompanyDate.Value.ToString("dd MMM, yyyy") : string.Empty);
            cellConfirmedDate.SetCellValue(item.ConfirmedDate != null ? item.ConfirmedDate.Value.ToString("dd MMM, yyyy") : string.Empty);
            cellResignedDate.SetCellValue(item.ResignedDate != null ? item.ResignedDate.Value.ToString("dd MMM, yyyy") : string.Empty);
            cellDOB.SetCellValue(item.DateOfBirth.ToString("dd MMM, yyyy"));
            cellGender.SetCellValue(gender);
            cellCellPhone.SetCellValue(item.Phone);
            cellEmail.SetCellValue(item.Email);
            cellCurrentAddress.SetCellValue(item.CurrentAddress);
            cellUnit.SetCellValue(item.Unit);
            cellCategory.SetCellValue(category);
        }

        /// <summary>
        /// Method RenderBodyOvertimeReport.
        /// </summary>
        /// <param name="rowIndex">The row Index.</param>
        /// <param name="cellNo">The cell number.</param>
        /// <param name="cellDate">The cellDate.</param>
        /// <param name="cellEmployee">The cellEmployee.</param>
        /// <param name="cellTeam">The cellTeam.</param>
        /// <param name="cellShift">The cellShift.</param>
        /// <param name="cellTimeIn">The cellTimeIn.</param>
        /// <param name="cellTimeOut">The cellTimeOut.</param>
        /// <param name="cellTotalHours">The cellTotalHours.</param>
        /// <param name="cellNormalDay">The cellNormalDay.</param>
        /// <param name="cellOffDay">The cellOffDay.</param>
        /// <param name="cellPublicHoliday">The cellPublicHoliday.</param>
        /// <param name="cellRemark">The cellRemark.</param>
        /// <param name="cellStatus">The cellStatus.</param>
        /// <param name="item">The OvertimeRequest entity.</param>
        /// <param name="row">Row to render data.</param>
        private void RenderBodyOvertimeReport(
           int rowIndex,
           ref ICell cellNo,
           ref ICell cellDate,
           ref ICell cellEmployee,
           ref ICell cellTeam,
           ref ICell cellShift,
           ref ICell cellTimeIn,
           ref ICell cellTimeOut,
           ref ICell cellTotalHours,
           ref ICell cellNormalDay,
           ref ICell cellOffDay,
           ref ICell cellPublicHoliday,
           ref ICell cellRemark,
           ref ICell cellStatus,
           OvertimeRequest item,
           IRow row)
        {
            cellNo = row.GetCell(0);
            cellDate = row.GetCell(1);
            cellEmployee = row.GetCell(2);
            cellTeam = row.GetCell(3);
            cellShift = row.GetCell(4);
            cellTimeIn = row.GetCell(5);
            cellTimeOut = row.GetCell(6);
            cellTotalHours = row.GetCell(7);
            cellNormalDay = row.GetCell(8);
            cellOffDay = row.GetCell(9);
            cellPublicHoliday = row.GetCell(10);
            cellRemark = row.GetCell(11);
            cellStatus = row.GetCell(12);

            CurrentRequestStatus status = (CurrentRequestStatus)item.Status;

            cellNo.SetCellValue(rowIndex - _RowStartBodyLeaveSummary + 2);
            cellDate.SetCellValue(item.OvertimeDate.ToString("dd MMM, yyyy"));
            cellEmployee.SetCellValue(item.FullName + " (" + item.NickName + ")");
            cellTeam.SetCellValue(item.Team);
            cellShift.SetCellValue(item.Schedule);
            cellTimeIn.SetCellValue(item.TimeIn);
            cellTimeOut.SetCellValue(item.TimeOut);
            cellTotalHours.SetCellValue(Math.Round(item.TotalWorkHours, 2));
            cellNormalDay.SetCellValue(item.NormalHours);
            cellOffDay.SetCellValue(item.OffHours);
            cellPublicHoliday.SetCellValue(item.PublicHours);
            cellRemark.SetCellValue(item.Remark);
            cellStatus.SetCellValue(status.ToString());
        }

        /// <summary>
        /// Render Body WorkingTime Request.
        /// </summary>
        /// <param name="rowIndex">Row Index to render data.</param>
        /// <param name="cellNo">The cellNo.</param>
        /// <param name="cellTeam">The cellTeam.</param>
        /// <param name="cellNick">The cellNick.</param>
        /// <param name="cellName">The cellName.</param>
        /// <param name="cellRequest">The cellRequest.</param>
        /// <param name="cellDetails">The cellDetails.</param>
        /// <param name="cellRemarks">The cellRemarks.</param>
        /// <param name="cellCreateDate">The cellCreateDate.</param>
        /// <param name="cellStatus">The cellStatus.</param>
        /// <param name="item">Item data.</param>
        /// <param name="row">Row to render data.</param>
        private void RenderBodyWorkingTimeRequest(
            int rowIndex,
            ref ICell cellNo,
            ref ICell cellTeam,
            ref ICell cellNick,
            ref ICell cellName,
            ref ICell cellRequest,
            ref ICell cellDetails,
            ref ICell cellRemarks,
            ref ICell cellCreateDate,
            ref ICell cellStatus,
            RequestEntity item,
            IRow row)
        {
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
            if (item.RequestType.Id == (int)RequestType.Leave)
            {
                cellRequest.SetCellValue(item.LeaveType.Name);
            }
            else
            {
                cellRequest.SetCellValue(item.RequestType.Name);
            }

            cellDetails.SetCellValue(this.GetRequestDetail(item));
            cellRemarks.SetCellValue(item.Remark);
            cellCreateDate.SetCellValue(item.CreatedDate.ToShortFormatedDate());
            cellStatus.SetCellValue(item.Status.Name);
        }

        /// <summary>
        /// Get request details.
        /// </summary>
        /// <param name="request">Employee Attendance Request.</param>
        /// <returns>Request details.</returns>
        private string GetRequestDetail(EmployeeAttendanceRequest request)
        {
            var detail = string.Empty;
            var date = string.Empty;

            if (request.FromDate.HasValue && request.ToDate.HasValue)
            {
                date = RequestService.GetMessageDateString(request.FromDate.Value, request.ToDate.Value);
            }

            switch (request.RequestType.Id)
            {
                case (int)RequestType.Leave:
                    if (request.TotalDays > 1)
                    {
                        detail = string.Format(_MsgLeave, Convert.ToInt32(request.TotalDays), date);
                    }
                    else if (request.TotalDays == 1)
                    {
                        detail = string.Format(_MsgLeaveOneDay, date);
                    }
                    else if ((request.TotalDays - (decimal)0.5) == 0)
                    {
                        // 0.5 day
                        detail = string.Format(_MsgLeaveHalfDay, date, request.FromDate.Value.ToString("HH:mm"), request.ToDate.Value.ToString("HH:mm"));
                    }
                    else
                    {
                        detail = string.Format(
                            _MsgLeaveDuringWorking,
                            date,
                            request.FromDate.Value.ToString("HH:mm"),
                            request.ToDate.Value.ToString("HH:mm"),
                            (request.ToDate.Value - request.FromDate.Value).TotalMinutes);
                    }

                    break;

                case (int)RequestType.ChangeShift:
                    detail = string.Format(_MsgChangeShift, request.FromShift, request.ToShift, request.FromDate.ToDateMonthYearFormatedDate());
                    break;

                case (int)RequestType.CheckIn:
                case (int)RequestType.CheckOut:
                    detail = string.Format(_MsgCheckInOut, request.FromDate.Value.ToString("HH:mm"), request.FromShift, request.FromDate.ToDateMonthYearFormatedDate());
                    break;

                case (int)RequestType.LateIn:
                    detail = string.Format(_MsgLateIn, request.TotalMins, request.FromShift, date);
                    break;
                case (int)RequestType.EarlyOut:
                    detail = string.Format(_MsgEarlyOut, request.TotalMins, request.FromShift, date);
                    break;
                case (int)RequestType.KeepInView:
                    detail = string.Format(_MsgKeepInView, request.FromShift, request.FromDate.ToDateMonthYearFormatedDate());
                    break;
                default:
                    break;
            }

            return detail;
        }

        /// <summary>
        /// Get request details.
        /// </summary>
        /// <param name="request">Request Entity.</param>
        /// <returns>Request details.</returns>
        private string GetRequestDetail(RequestEntity request)
        {
            var detail = string.Empty;
            var date = RequestService.GetMessageDateString(request.FromDate, request.ToDate);

            switch (request.RequestType.Id)
            {
                case (int)RequestType.Leave:
                    if (request.TotalDays > 1)
                    {
                        detail = string.Format(_MsgLeave, Convert.ToInt32(request.TotalDays), date);
                    }
                    else if (request.TotalDays == 1)
                    {
                        detail = string.Format(_MsgLeaveOneDay, date);
                    }
                    else if ((request.TotalDays - (decimal)0.5) == 0)
                    {
                        // 0.5 day
                        detail = string.Format(_MsgLeaveHalfDay, date, request.FromDate.ToString("HH:mm"), request.ToDate.ToString("HH:mm"));
                    }
                    else
                    {
                        detail = string.Format(
                            _MsgLeaveDuringWorking,
                            date,
                            request.FromDate.ToString("HH:mm"),
                            request.ToDate.ToString("HH:mm"),
                            (request.ToDate - request.FromDate).TotalMinutes);
                    }

                    break;

                case (int)RequestType.ChangeShift:
                    detail = string.Format(_MsgChangeShift, request.FromShift.Name, request.ToShift.Name, request.FromDate.ToDateMonthYearFormatedDate());
                    break;

                case (int)RequestType.CheckIn:
                case (int)RequestType.CheckOut:
                    detail = string.Format(_MsgCheckInOut, request.FromDate.ToString("HH:mm"), request.FromShift.Name, request.FromDate.ToDateMonthYearFormatedDate());
                    break;

                case (int)RequestType.LateIn:
                    detail = string.Format(_MsgLateIn, request.TotalMins, request.FromShift.Name, date);
                    break;
                case (int)RequestType.EarlyOut:
                    detail = string.Format(_MsgEarlyOut, request.TotalMins, request.FromShift.Name, date);
                    break;
                case (int)RequestType.Overtime:
                    if (request.IsOvertimeHours)
                    {
                        detail = string.Format(
                            _MsgOvertimeHours,
                            request.FromDate.ToDateMonthYearFormatedDate(),
                            request.FromDate.ToString("HH:mm"),
                            request.ToDate.ToString("HH:mm"),
                            request.TotalDays.ToString("0.#"),
                            new OvertimeTypeModel { Id = request.OvertimeType }.Name);
                    }
                    else
                    {
                        detail = string.Format(
                            _MsgOvertimeShift,
                            request.FromDate.ToDateMonthYearFormatedDate(),
                            request.FromShift != null ? request.FromShift.Name : string.Empty,
                            new OvertimeTypeModel { Id = request.OvertimeType }.Name);
                    }

                    break;
                case (int)RequestType.KeepInView:
                    detail = string.Format(_MsgKeepInView, request.FromShift.Name, request.FromDate.ToDateMonthYearFormatedDate());
                    break;
                default:
                    break;
            }

            return detail;
        }
    }
}
