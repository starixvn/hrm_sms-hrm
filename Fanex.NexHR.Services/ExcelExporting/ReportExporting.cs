// --------------------------------------------------------------------------
// <copyright file="ReportExporting.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.16">Created</change>
// </history>

namespace Fanex.NexHR.Services.ExcelExporting
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using NPOI.HSSF.UserModel;

    /// <summary>
    /// Report Exporting.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
    public class ReportExporting
    {
        private const int _RowStartBodyHrRequestStatistic = 4;
        private const int _RowStartBodyWorkingCalendarSummary = 5;
        private const int _RowStartBodyEmployeeSupervisor = 4;
        private const int _RowStartBodyTeamSupervisor = 4;
        private const string _HrRequestStatisticFilePathExcelTemplate = "~\\ExcelTemplates\\HrRequestStatisticTemplate.xls";
        private const string _EmployeeWorkingCalendarSummaryFilePathExcelTemplate = "~\\ExcelTemplates\\WorkingCalendarSummaryTemplate.xls";
        private const string _EmployeeSupervisorFilePathExcelTemplate = "~\\ExcelTemplates\\EmployeeSupervisorTemplate.xls";
        private const string _TeamSupervisorFilePathExcelTemplate = "~\\ExcelTemplates\\TeamSupervisorTemplate.xls";

        /// <summary>
        /// Method Export HR Request Statistic.
        /// </summary>
        /// <param name="requests">List of Request Monitoring Entity.</param>
        /// <param name="fromDate">Export From Date.</param>
        /// <param name="toDate">Export To Date.</param>
        /// <returns>The instance of HSSFWorkbook supports for exporting excel file.</returns>
        public HSSFWorkbook ExportHrRequestStatistic(List<RequestsMonitoring> requests, DateTime fromDate, DateTime toDate)
        {
            HSSFWorkbook hssfworkbook;

            var templatePath = HttpContext.Current.Server.MapPath(_HrRequestStatisticFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, System.IO.FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "Request Statistic");

            var rowDate = sheet.GetRow(1);
            rowDate.GetCell(4).SetCellValue(fromDate.ToString("dd MMM, yyyy"));
            rowDate.GetCell(6).SetCellValue(toDate.ToString("dd MMM, yyyy"));

            var rowIndex = _RowStartBodyHrRequestStatistic;
            var rowCopy = _RowStartBodyHrRequestStatistic;
            var height = sheet.GetRow(_RowStartBodyHrRequestStatistic).Height;
            var index = 1;

            foreach (var item in requests)
            {
                rowCopy = _RowStartBodyHrRequestStatistic + (index % 2);
                sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                var row = sheet.GetRow(rowIndex);
                row.Height = height;

                row.GetCell(0).SetCellValue(index);
                row.GetCell(1).SetCellValue(item.Requester.Id);
                row.GetCell(2).SetCellValue(item.Requester.FullName);
                row.GetCell(3).SetCellValue(item.Requester.NickName);
                row.GetCell(4).SetCellValue(item.Requester.Team.Name);
                row.GetCell(5).SetCellValue(item.TotalLeave);
                row.GetCell(6).SetCellValue(item.TotalUnpaidLeave);
                row.GetCell(7).SetCellValue(item.TotalChangeShift);
                row.GetCell(8).SetCellValue(item.TotalLateIn);
                row.GetCell(9).SetCellValue(item.TotalEarlyOut);
                row.GetCell(10).SetCellValue(item.TotalCheckInOut);

                rowIndex++;
                index++;
            }

            hssfworkbook.RemoveSheetAt(0);

            return hssfworkbook;
        }

        /// <summary>
        /// Method Export Total Working Calendar Summary.
        /// </summary>
        /// <param name="calendars">List of Working Calendar Entity.</param>
        /// <param name="fromDate">Export From Date.</param>
        /// <param name="toDate">Export To Date.</param>
        /// <returns>The instance of HSSFWorkbook supports for exporting excel file.</returns>
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
            rowDate.GetCell(2).SetCellValue(fromDate.ToString("dd MMM, yyyy"));
            rowDate.GetCell(4).SetCellValue(toDate.ToString("dd MMM, yyyy"));

            var rowIndex = _RowStartBodyWorkingCalendarSummary;
            var rowCopy = _RowStartBodyWorkingCalendarSummary;
            var height = sheet.GetRow(_RowStartBodyWorkingCalendarSummary).Height;
            var index = 1;
            int countItem = 0;
            int totalCellInRow = 0;
            int startColumn = 5;

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
                    var customCellWidth = 3071;

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
                    rowHeaderDate.GetCell(0).SetCellValue("#");
                    rowHeaderDate.GetCell(1).SetCellValue("ID");
                    rowHeaderDate.GetCell(2).SetCellValue("Full Name");
                    rowHeaderDate.GetCell(3).SetCellValue("Nick Name");
                    rowHeaderDate.GetCell(4).SetCellValue("Team");

                    foreach (var item in firstItemInGroup.Calendar)
                    {
                        if (startColumn < totalCellInRow)
                        {
                            rowHeaderDate.GetCell(startColumn).SetCellValue(item.WorkDate.ToString("dd MMM, yyyy"));
                            rowHeaderDay.GetCell(startColumn).SetCellValue(item.WorkDate.ToString("ddd"));
                        }

                        startColumn++;
                    }
                }

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

        /// <summary>
        /// Export Employee Supervisor.
        /// </summary>
        /// <param name="supervisors">SupervisorReportGroup Model.</param>
        /// <returns>The instance of HSSFWorkbook supports for exporting excel file.</returns>
        public HSSFWorkbook ExportEmployeeSupervisor(List<SupervisorReportGroupModel> supervisors)
        {
            HSSFWorkbook hssfworkbook;

            var templatePath = HttpContext.Current.Server.MapPath(_EmployeeSupervisorFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, System.IO.FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "Employee Supervisor");

            var rowIndex = _RowStartBodyEmployeeSupervisor;
            var rowCopy = _RowStartBodyEmployeeSupervisor;
            var height = sheet.GetRow(_RowStartBodyEmployeeSupervisor).Height;
            var groupIndex = 1;

            foreach (var item in supervisors)
            {
                rowCopy = _RowStartBodyEmployeeSupervisor + (groupIndex % 2);
                var row = sheet.GetRow(rowIndex);
                foreach (var supervisor in item.SupervisorAction)
                {
                    sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                    row = sheet.GetRow(rowIndex);
                    row.Height = height;
                    row.GetCell(0).SetCellValue(groupIndex);
                    row.GetCell(1).SetCellValue(supervisor.NickName);
                    row.GetCell(2).SetCellValue(supervisor.FullName);
                    row.GetCell(3).SetCellValue(supervisor.Code);
                    row.GetCell(4).SetCellValue(supervisor.ActionName);
                    row.GetCell(5).SetCellValue(supervisor.Supervisors);
                    rowIndex++;
                }

                groupIndex++;

                var firstRow = rowIndex - item.SupervisorAction.Count;
                var lastRow = rowIndex - 1;
                MergeCell(row, firstRow, lastRow);
            }

            hssfworkbook.RemoveSheetAt(0);

            return hssfworkbook;
        }

        /// <summary>
        /// Export Excel Team Supervisors.
        /// </summary>
        /// <param name="supervisors">List Of Team Supervisor Report Model.</param>
        /// <returns>The instance of HSSFWorkbook supports for exporting excel file.</returns>
        public HSSFWorkbook ExportTeamSupervisor(List<TeamSupervisorReportModel> supervisors)
        {
            HSSFWorkbook hssfworkbook;

            var templatePath = HttpContext.Current.Server.MapPath(_TeamSupervisorFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, System.IO.FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "Team Supervisor");

            var rowIndex = _RowStartBodyEmployeeSupervisor;
            var rowCopy = _RowStartBodyEmployeeSupervisor;
            var height = sheet.GetRow(_RowStartBodyEmployeeSupervisor).Height;
            var groupIndex = 1;

            foreach (var item in supervisors)
            {
                rowCopy = _RowStartBodyEmployeeSupervisor + (groupIndex % 2);
                var row = sheet.GetRow(rowIndex);
                foreach (var supervisor in item.SupervisorAction)
                {
                    sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                    row = sheet.GetRow(rowIndex);
                    row.Height = height;
                    row.GetCell(0).SetCellValue(groupIndex);
                    row.GetCell(1).SetCellValue(supervisor.Code);
                    row.GetCell(2).SetCellValue(supervisor.ActionName);
                    row.GetCell(3).SetCellValue(supervisor.Supervisors);
                    rowIndex++;
                }

                groupIndex++;

                var firstRow = rowIndex - item.SupervisorAction.Count;
                var lastRow = rowIndex - 1;
                MergeCellTeam(row, firstRow, lastRow);
            }

            hssfworkbook.RemoveSheetAt(0);

            return hssfworkbook;
        }

        private void MergeCell(NPOI.SS.UserModel.IRow row, int firstRow, int lastRow)
        {
            NPOI.SS.Util.CellRangeAddress cra1 = new NPOI.SS.Util.CellRangeAddress(firstRow, lastRow, 0, 0);
            NPOI.SS.Util.CellRangeAddress cra2 = new NPOI.SS.Util.CellRangeAddress(firstRow, lastRow, 1, 1);
            NPOI.SS.Util.CellRangeAddress cra3 = new NPOI.SS.Util.CellRangeAddress(firstRow, lastRow, 2, 2);
            NPOI.SS.Util.CellRangeAddress cra4 = new NPOI.SS.Util.CellRangeAddress(firstRow, lastRow, 3, 3);
            row.Sheet.AddMergedRegion(cra1);
            row.Sheet.AddMergedRegion(cra2);
            row.Sheet.AddMergedRegion(cra3);
            row.Sheet.AddMergedRegion(cra4);
        }

        private void MergeCellTeam(NPOI.SS.UserModel.IRow row, int firstRow, int lastRow)
        {
            NPOI.SS.Util.CellRangeAddress cra1 = new NPOI.SS.Util.CellRangeAddress(firstRow, lastRow, 0, 0);
            NPOI.SS.Util.CellRangeAddress cra2 = new NPOI.SS.Util.CellRangeAddress(firstRow, lastRow, 1, 1);
            row.Sheet.AddMergedRegion(cra1);
            row.Sheet.AddMergedRegion(cra2);
        }
    }
}
