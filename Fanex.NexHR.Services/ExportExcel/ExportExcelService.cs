namespace Fanex.NexHR.Services.ExportExcel
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
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.ViewModels.Calendar;
    using Fanex.NexHR.ViewModels.Overtime;
    using Fanex.NexHR.ViewModels.Report;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
    public class ExportExcelService : IExportExcelService
    {
        private const int _RowStartBodyAttendance = 3;
        private const int _RowStartBodyLeaveSummary = 4;
        private const int _RowStartBodyWorkingTimeRequest = 3;
        private const int _RowStartBodyOvertimeReport = 4;
        private const int _RowStartBodyOvertimeRecord = 3;
        private const int _RowStartBodyWorkingCalendar = 5;
        private const int _RowStartBodyYearlyOvertimeReport = 5;
        private const int _RowStartBodyManpower = 4;
        private const int _RowStartBodyManpowerCategory = 4;

        private const string _AttendanceFilePathExcelTemplate = "~\\ExcelTemplates\\AttendanceReportTemplate.xls";
        private const string _LeaveSummaryFilePathExcelTemplate = "~\\ExcelTemplates\\LeaveSummaryTemplate.xls";
        private const string _UnpaidLeaveSummaryFilePathExcelTemplate = "~\\ExcelTemplates\\UnpaidLeaveSummaryTemplate.xls";
        private const string _WorkingTimeRequestFilePathExcelTemplate = "~\\ExcelTemplates\\WorkingTimeRequestTemplate.xls";
        private const string _OvertimeReportFilePathExcelTemplate = "~\\ExcelTemplates\\OvertimeReportTemplate.xls";
        private const string _YearlyOvertimeReportFilePathExcelTemplate = "~\\ExcelTemplates\\YearlyOvertimeReportTemplate.xls";
        private const string _EmployeeBasicInfoFilePathExcelTemplate = "~\\ExcelTemplates\\EmployeeBasicInfoTemplate.xls";
        private const string _EmployeeDetailsInfoFilePathExcelTemplate = "~\\ExcelTemplates\\EmployeeDetailsInfoTemplate.xls";
        private const string _EmployeeOvertimeRecordFilePathExcelTemplate = "~\\ExcelTemplates\\EmployeeOvertimeRecordTemplate.xls";
        private const string _EmployeeWorkingCalendarFilePathExcelTemplate = "~\\ExcelTemplates\\WorkingCalendarTemplate.xls";
        private const string _ManpowerFilePathExcelTemplate = "~\\ExcelTemplates\\ManpowerReportTemplate.xls";
        private const string _ManpowerCategoryFilePathExcelTemplate = "~\\ExcelTemplates\\ManpowerCategoryReportTemplate.xls";

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

        public HSSFWorkbook ExportWorkingCalendarSummary(List<WorkingCalendarSummary> calendars, DateTime fromDate, DateTime toDate)
        {
            var exportExcel = new ReportExporting();
            HSSFWorkbook hssfworkbook = exportExcel.ExportWorkingCalendarSummary(calendars, fromDate, toDate);

            return hssfworkbook;
        }

        public HSSFWorkbook ExportHRRequestStatistic(List<RequestsMonitoring> requests, DateTime fromDate, DateTime toDate)
        {
            var exportExcel = new ReportExporting();
            HSSFWorkbook hssfworkbook = exportExcel.ExportHRRequestStatistic(requests, fromDate, toDate);

            return hssfworkbook;
        }

        public void ExportEmployeeSupervisor(IList<SupervisorReportModel> supervisors, string templatePath)
        {
            string fileName = "EmployeeSupervisor_" + DateTime.Now.ToString("yyyyMMdd");
            var writer = new ExcelTemplateWriter(templatePath, fileName, "Employee Supervisor");

            /// Fill report title.
            writer.CopyRows(0, 2);
            /// Fill report header.
            writer.CopyRows(2, 1);

            FillContentRowEmployeeSupervisorReport(supervisors, ref writer);
            writer.SyncColumnsWidth(10);

            writer.Render();
        }

        public void ExportSectionSupervisor(IList<SectionSupervisorReportModel> supervisors, string templatePath)
        {
            string fileName = "SectionSupervisor_" + DateTime.Now.ToString("yyyyMMdd");
            var writer = new ExcelTemplateWriter(templatePath, fileName, "Section Supervisor");

            // Fill report title.
            writer.CopyRows(0, 2);
            // Fill report header.
            writer.CopyRows(2, 1);

            FillContentRowSectionSupervisorReport(supervisors, ref writer);
            writer.SyncColumnsWidth(10);

            writer.Render();
        }

        public HSSFWorkbook ExportAttendance(IList<EmployeeAttendanceModel> attendanceList, DateTime fromDate, DateTime toDate)
        {
            HSSFWorkbook hssfworkbook;
            var templatePath = HttpContext.Current.Server.MapPath(_AttendanceFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "AttendanceReport");

            //// render body
            var rowIndex = _RowStartBodyAttendance;
            var index = 1;
            var height = sheet.GetRow(_RowStartBodyAttendance).Height;

            foreach (var item in attendanceList)
            {
                var requests = string.Empty;
                var rowCopy = _RowStartBodyAttendance + (index % 2);
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
                row.GetCell(6).SetCellValue(string.IsNullOrEmpty(item.ShiftCode) ? string.Empty : item.ShiftCode);
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
                            + "\r\n- Create date: " + request.CreatedDate.ToDateMonthYearFormatedDate()
                            + (string.IsNullOrEmpty(request.Remark) ? string.Empty : "\r\n" + "- Remark: " + request.Remark) + "\r\n";
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

        public HSSFWorkbook ExportManpower(IList<ManpowerModel> manpowers, int year)
        {
            HSSFWorkbook hssfworkbook;
            var templatePath = HttpContext.Current.Server.MapPath(_ManpowerFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var title = Labels.ManpowerReportTitle + " " + year;
            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "ManpowerReport");

            var rowTitle = sheet.GetRow(0);
            var cellTitle = rowTitle.GetCell(1);
            cellTitle.SetCellValue(title);

            //// render body
            var rowIndex = _RowStartBodyManpower;
            var index = 1;
            var height = sheet.GetRow(_RowStartBodyAttendance).Height;

            var totalManpower = new ManpowerModel();

            foreach (var item in manpowers)
            {
                var rowCopy = _RowStartBodyManpower + (index % 2);

                sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                var row = sheet.GetRow(rowIndex);
                row.Height = height;

                RenderBodyManpowerReport(row, item, year, index);
                totalManpower = AddTotalManpower(totalManpower, item);
                rowIndex++;
                index++;
            }

            sheet.CopyRow(templateSheet, _RowStartBodyManpower + 2, rowIndex);
            var totalrow = sheet.GetRow(rowIndex);
            totalrow.Height = height;

            RenderFooterManpowerReport(totalrow, totalManpower, year);

            //// remove sheet & row template
            hssfworkbook.RemoveSheetAt(0);

            return hssfworkbook;
        }

        public HSSFWorkbook ExportManpowerCategory(IList<ManpowerCategoryModel> manpowers, int year)
        {
            HSSFWorkbook hssfworkbook;
            var templatePath = HttpContext.Current.Server.MapPath(_ManpowerCategoryFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var title = Labels.ManpowerCategoryReportTitle + " " + year;
            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "ManpowerCategoryReport");

            var rowTitle = sheet.GetRow(0);
            var cellTitle = rowTitle.GetCell(1);
            cellTitle.SetCellValue(title);

            //// render body
            var rowIndex = _RowStartBodyManpowerCategory;
            var index = 1;
            var height = sheet.GetRow(_RowStartBodyAttendance).Height;

            var totalManpower = new ManpowerCategoryModel();

            foreach (ManpowerCategoryModel item in manpowers)
            {
                var rowCopy = _RowStartBodyManpower + (index % 2);
                sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                var row = sheet.GetRow(rowIndex);
                row.Height = height;

                RenderBodyManpowerCategoryReport(row, item, index);
                totalManpower = AddTotalManpowerCategory(totalManpower, item);
                rowIndex++;
                index++;
            }

            sheet.CopyRow(templateSheet, _RowStartBodyManpower + 2, rowIndex);
            var totalrow = sheet.GetRow(rowIndex);
            totalrow.Height = height;

            RenderFooterManpowerCategoryReport(totalrow, totalManpower);

            //// remove sheet & row template
            hssfworkbook.RemoveSheetAt(0);

            return hssfworkbook;
        }

        public HSSFWorkbook ExportLeaveSummary(IList<LeaveSummaryByTeam> leaveSummaryList, int year)
        {
            HSSFWorkbook hssfworkbook;

            var templatePath = HttpContext.Current.Server.MapPath(_LeaveSummaryFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
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

        public HSSFWorkbook ExportUnpaidLeaveSummary(IList<LeaveSummaryByTeam> leaveSummaryList, int year)
        {
            HSSFWorkbook hssfworkbook;

            var templatePath = HttpContext.Current.Server.MapPath(_UnpaidLeaveSummaryFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, System.IO.FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "UnpaidLeaveSummary");

            //// render hearder
            var rowTakeLeave = sheet.GetRow(2);
            var cellTakeLeave = rowTakeLeave.GetCell(3);
            cellTakeLeave.SetCellValue("Unpaid Leave's Report " + year);

            var rowAnnualBalance = sheet.GetRow(2);
            var cellAnnualBalance = rowAnnualBalance.GetCell(15);
            cellAnnualBalance.SetCellValue("Total Of UPL " + year);

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
                    sheet.MergeCells(rowIndex, 0, rowIndex, 15);
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
                        row.GetCell(3).SetCellValue(leaveSummary.Jan);
                        row.GetCell(4).SetCellValue(leaveSummary.Feb);
                        row.GetCell(5).SetCellValue(leaveSummary.Mar);
                        row.GetCell(6).SetCellValue(leaveSummary.Apr);
                        row.GetCell(7).SetCellValue(leaveSummary.May);
                        row.GetCell(8).SetCellValue(leaveSummary.Jun);
                        row.GetCell(9).SetCellValue(leaveSummary.Jul);
                        row.GetCell(10).SetCellValue(leaveSummary.Aug);
                        row.GetCell(11).SetCellValue(leaveSummary.Sep);
                        row.GetCell(12).SetCellValue(leaveSummary.Oct);
                        row.GetCell(13).SetCellValue(leaveSummary.Nov);
                        row.GetCell(14).SetCellValue(leaveSummary.Dec);
                        row.GetCell(15).SetCellValue(leaveSummary.LeaveBalance);

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

        public HSSFWorkbook ExportEmployeeOvertimeRecord(List<OvertimeExcelReport> overtimes, int? calendar, DateTime fromDate, DateTime toDate)
        {
            int totalDay = Convert.ToInt32((toDate - fromDate).TotalDays) + 1;
            HSSFWorkbook hssfworkbook;

            var templatePath = HttpContext.Current.Server.MapPath(_EmployeeOvertimeRecordFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
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

            if (overtimes != null && overtimes.Any())
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
                            var rowCopy = _RowStartBodyOvertimeRecord + (index % 2);
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
                                    row.GetCell(i + 10).SetCellValue(string.IsNullOrEmpty(cellValue) ? defaultCellValue : Convert.ToDouble(cellValue));

                                    indexCount++;
                                }

                                if (i % 2 == 1)
                                {
                                    var cellValue = data.TotalOvertimeType1;
                                    row.GetCell(i + 10).SetCellValue(string.IsNullOrEmpty(cellValue) ? defaultCellValue : Convert.ToDouble(cellValue));
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
                        string formulaNormal = "SUM(H{0}:H{1})",
                            formulaOff = "SUM(I{0}:I{1})",
                            formulaPH = "SUM(J{0}:J{1})",
                            formulaTax = "SUM(K{0}:K{1})";

                        string data1 = Convert.ToString(numberStart),
                            data2 = Convert.ToString(rowIndex);
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

        public HSSFWorkbook ExportWorkingTimeRequest(IList<RequestEntity> requestList)
        {
            HSSFWorkbook hssfworkbook;
            var templatePath = HttpContext.Current.Server.MapPath(_WorkingTimeRequestFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "WorkingTimeRequest");
            var rowIndex = _RowStartBodyWorkingTimeRequest;
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
                var rowCopy = _RowStartBodyWorkingTimeRequest + (rowIndex % 2);
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

        public HSSFWorkbook ExportOvertimeReport(IList<OvertimeDaily> requestList, DateTime fromDate, DateTime toDate)
        {
            HSSFWorkbook hssfworkbook;
            var templatePath = HttpContext.Current.Server.MapPath(_OvertimeReportFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "OvertimeReport");
            var rowIndex = _RowStartBodyOvertimeReport;
            var rowCopyDate = _RowStartBodyOvertimeReport - 3;

            var height = sheet.GetRow(_RowStartBodyOvertimeReport).Height;
            ICell cellFromDate = null;
            ICell cellToDate = null;
            ICell cellNo = null;
            ICell cellDate = null;
            ICell cellEmployeeCode = null;
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
                var rowCopy = _RowStartBodyOvertimeReport + (index % 2);
                sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                var row = sheet.GetRow(rowIndex);
                row.Height = height;
                RenderBodyOvertimeReport(
                    rowIndex - 1,
                    ref cellNo,
                    ref cellDate,
                    ref cellEmployeeCode,
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

        public HSSFWorkbook ExportYearlyOvertimeReport(IList<OvertimeAnnualReportViewModel> requestList, int year, int? calendar)
        {
            HSSFWorkbook hssfworkbook;
            var templatePath = HttpContext.Current.Server.MapPath(_YearlyOvertimeReportFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "YearlyOvertimeReport");
            var rowIndex = _RowStartBodyYearlyOvertimeReport;

            string calendarType = string.Empty;
            int index = 1;
            float totalYearlyOTHours = 0;
            var height = sheet.GetRow(_RowStartBodyOvertimeReport).Height;

            calendarType = calendar == 1 ? "Fiscal Calendar" : "Normal Calendar";

            // Header
            var rowHeader = sheet.GetRow(3);
            rowHeader.Height = height;
            var cellYear = rowHeader.GetCell(4);
            cellYear.SetCellValue("Overtime Summary In " + year + " (" + calendarType + ")");
            sheet.RemoveRow(sheet.GetRow(5));
            sheet.RemoveRow(sheet.GetRow(6));
            sheet.RemoveRow(sheet.GetRow(7));
            foreach (var item in requestList)
            {
                var rowCopy = _RowStartBodyYearlyOvertimeReport + (index % 2);
                sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                var row = sheet.GetRow(rowIndex);
                row.Height = height;
                float totalHours = 0;

                row.GetCell(0).SetCellValue(index);
                row.GetCell(1).SetCellValue(item.Team);
                row.GetCell(2).SetCellValue(item.FullName);
                row.GetCell(3).SetCellValue(item.NickName);

                SetNumberValue(row.GetCell(4), item.Jan);
                SetNumberValue(row.GetCell(5), item.Feb);
                SetNumberValue(row.GetCell(6), item.Mar);
                SetNumberValue(row.GetCell(7), item.Apr);
                SetNumberValue(row.GetCell(8), item.May);
                SetNumberValue(row.GetCell(9), item.Jun);
                SetNumberValue(row.GetCell(10), item.Jul);
                SetNumberValue(row.GetCell(11), item.Aug);
                SetNumberValue(row.GetCell(12), item.Sep);
                SetNumberValue(row.GetCell(13), item.Oct);
                SetNumberValue(row.GetCell(14), item.Nov);
                SetNumberValue(row.GetCell(15), item.Dec);

                totalHours += item.Jan + item.Feb + item.Mar + item.Apr + item.May + item.Jun + item.Jul + item.Aug + item.Sep
                    + item.Oct + item.Nov + item.Dec;

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

        public HSSFWorkbook ExportEmployeeBasicInfo(IList<Employee> employees)
        {
            HSSFWorkbook hssfworkbook;
            var templatePath = HttpContext.Current.Server.MapPath(_EmployeeBasicInfoFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "EmployeeBasicInfo");
            var rowIndex = _RowStartBodyOvertimeReport;

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
                var rowCopy = _RowStartBodyOvertimeReport + (index % 2);
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

        public HSSFWorkbook ExportEmployeeDetailsInfo(IList<Employee> employees)
        {
            HSSFWorkbook hssfworkbook;
            var templatePath = HttpContext.Current.Server.MapPath(_EmployeeDetailsInfoFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "EmployeeDetailsInfo");
            var rowIndex = _RowStartBodyOvertimeReport;
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
                var rowCopy = _RowStartBodyOvertimeReport + (index % 2);
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

        public HSSFWorkbook ExportEmployeeWorkingCalendar(IList<WorkingCalendarsViewModel> workingCalendars, DateTime fromDate, DateTime toDate)
        {
            HSSFWorkbook hssfworkbook;
            var templatePath = HttpContext.Current.Server.MapPath(_EmployeeWorkingCalendarFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "WorkingCalendar");
            var rowIndex = _RowStartBodyWorkingCalendar;

            var rowDate = sheet.GetRow(1);
            ICell cellFromDate = rowDate.GetCell(2);
            ICell cellToDate = rowDate.GetCell(4);
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

        private void FillContentRowEmployeeSupervisorReport(IList<SupervisorReportModel> supervisors, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (SupervisorReportModel supervisor in supervisors)
            {
                FillRowEmployeeSupervisorReport(supervisor, ref writer, ref rowIndex);
            }
        }

        private void FillRowEmployeeSupervisorReport(SupervisorReportModel supervisor, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordEmployeeSupervisorReport(supervisor, ref rowIndex);
            if (rowIndex % 2 == 1)
            {
                writer.WriteRow(3, record);
            }
            else
            {
                writer.WriteRow(4, record);
            }

            rowIndex++;
        }

        private object CreateRecordEmployeeSupervisorReport(SupervisorReportModel supervisor, ref int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                NickName = supervisor.NickName,
                FullName = supervisor.FullName,
                Section = supervisor.Section.Code,
                Roles = supervisor.Roles2String,
                Verifier = string.Join(", ", supervisor.Supervisors.Where(x => x.ActionType == (int)SupervisorActionType.Verifier).Select(x => x.NickName)),
                Approver = string.Join(", ", supervisor.Supervisors.Where(x => x.ActionType == (int)SupervisorActionType.Approver).Select(x => x.NickName)),
                ShiftArrangement = string.Join(", ", supervisor.Supervisors.Where(x => x.ActionType == (int)SupervisorActionType.ShiftArrangement).Select(x => x.NickName)),
                OnBehalfRequest = string.Join(", ", supervisor.Supervisors.Where(x => x.ActionType == (int)SupervisorActionType.OnBehalfRequest).Select(x => x.NickName)),
                DisciplineManager = string.Join(", ", supervisor.Supervisors.Where(x => x.ActionType == (int)SupervisorActionType.DisciplineManager).Select(x => x.NickName))
            };

            return record;
        }

        private void FillContentRowSectionSupervisorReport(IList<SectionSupervisorReportModel> supervisors, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (SectionSupervisorReportModel supervisor in supervisors)
            {
                FillRowSectionSupervisorReport(supervisor, ref writer, ref rowIndex);
            }
        }

        private void FillRowSectionSupervisorReport(SectionSupervisorReportModel supervisor, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordSectionSupervisorReport(supervisor, ref rowIndex);
            if (rowIndex % 2 == 1)
            {
                writer.WriteRow(3, record);
            }
            else
            {
                writer.WriteRow(4, record);
            }

            rowIndex++;
        }

        private object CreateRecordSectionSupervisorReport(SectionSupervisorReportModel supervisor, ref int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                Section = supervisor.Code,
                Verifier = string.Join(", ", supervisor.Supervisors.Where(x => x.ActionType == (int)SupervisorActionType.Verifier).Select(x => x.NickName)),
                Approver = string.Join(", ", supervisor.Supervisors.Where(x => x.ActionType == (int)SupervisorActionType.Approver).Select(x => x.NickName)),
                ShiftArrangement = string.Join(", ", supervisor.Supervisors.Where(x => x.ActionType == (int)SupervisorActionType.ShiftArrangement).Select(x => x.NickName)),
                OnBehalfRequest = string.Join(", ", supervisor.Supervisors.Where(x => x.ActionType == (int)SupervisorActionType.OnBehalfRequest).Select(x => x.NickName)),
                DisciplineManager = string.Join(", ", supervisor.Supervisors.Where(x => x.ActionType == (int)SupervisorActionType.DisciplineManager).Select(x => x.NickName))
            };

            return record;
        }

        private void SetNumberValue(ICell cell, float value)
        {
            if (value != 0)
            {
                cell.SetCellValue(value);
            }
        }

        private void RenderFooterForOvertimeReport(IList<OvertimeAnnualReportViewModel> requestList, float totalYearlyOTHours, IRow rowFooter)
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

            string gender = string.Empty,
                category = string.Empty;

            gender = item.Gender == 0 ? "F" : "M";
            category = item.IsLocal ? "Local" : "Expat";

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

            string gender = string.Empty,
                category = string.Empty;

            gender = item.Gender == 0 ? "F" : "M";
            category = item.IsLocal ? "Local" : "Expat";

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

        private void RenderBodyOvertimeReport(
           int rowIndex,
           ref ICell cellNo,
           ref ICell cellDate,
           ref ICell cellEmployeeCode,
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
           OvertimeDaily item,
           IRow row)
        {
            cellNo = row.GetCell(0);
            cellDate = row.GetCell(1);
            cellEmployeeCode = row.GetCell(2);
            cellEmployee = row.GetCell(3);
            cellTeam = row.GetCell(4);
            cellShift = row.GetCell(5);
            cellTimeIn = row.GetCell(6);
            cellTimeOut = row.GetCell(7);
            cellTotalHours = row.GetCell(8);
            cellNormalDay = row.GetCell(9);
            cellOffDay = row.GetCell(10);
            cellPublicHoliday = row.GetCell(11);
            cellRemark = row.GetCell(12);
            cellStatus = row.GetCell(13);

            CurrentRequestStatus status = (CurrentRequestStatus)item.Status;

            cellNo.SetCellValue(rowIndex - _RowStartBodyLeaveSummary + 2);
            cellDate.SetCellValue(item.OvertimeDate.ToString("dd MMM, yyyy"));
            cellEmployeeCode.SetCellValue(item.Code);
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
            cellRequest.SetCellValue(item.RequestType.Id == (int)RequestType.Leave ? item.LeaveType.Name : item.RequestType.Name);

            cellDetails.SetCellValue(this.GetRequestDetail(item));
            cellRemarks.SetCellValue(item.Remark);
            cellCreateDate.SetCellValue(item.CreatedDate.ToShortFormatedDate());
            cellStatus.SetCellValue(item.Status.Name);
        }

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
            }

            return detail;
        }

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
                    detail = string.Format(_MsgCheckInOut, request.RequestedDate.ToString("HH:mm"), request.FromShift.Name, request.FromDate.ToDateMonthYearFormatedDate());
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
            }

            return detail;
        }

        private void SetValueForCellManpower(ICell cell, int value, int year, int month)
        {
            var isShowValue = false;

            if (DateTime.Now.Year > year)
            {
                isShowValue = true;
            }
            else if (DateTime.Now.Year == year)
            {
                if (DateTime.Now.Month >= month)
                {
                    isShowValue = true;
                }
            }

            if (isShowValue)
            {
                cell.SetCellValue(value);
            }
            else
            {
                cell.SetCellValue("-");
            }
        }

        private void RenderBodyManpowerReport(IRow row, ManpowerModel item, int year, int index)
        {
            var columnIndex = 0;
            var month = 1;

            row.GetCell(columnIndex++).SetCellValue(index);
            row.GetCell(columnIndex++).SetCellValue(item.GroupCode);

            row.GetCell(columnIndex++).SetCellValue(item.ProbationResigned);
            row.GetCell(columnIndex++).SetCellValue(item.ConfirmedResigned);
            row.GetCell(columnIndex++).SetCellValue(item.NewHire);
            row.GetCell(columnIndex++).SetCellValue(item.ActiveUser);

            SetValueForCellManpower(row.GetCell(columnIndex++), item.ProbationResignedJan, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ConfirmedResignedJan, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.NewHireJan, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ActiveUserJan, year, month);

            month++;
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ProbationResignedFeb, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ConfirmedResignedFeb, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.NewHireFeb, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ActiveUserFeb, year, month);

            month++;
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ProbationResignedMar, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ConfirmedResignedMar, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.NewHireMar, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ActiveUserMar, year, month);

            month++;
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ProbationResignedApr, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ConfirmedResignedApr, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.NewHireApr, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ActiveUserApr, year, month);

            month++;
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ProbationResignedMay, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ConfirmedResignedMay, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.NewHireMay, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ActiveUserMay, year, month);

            month++;
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ProbationResignedJun, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ConfirmedResignedJun, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.NewHireJun, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ActiveUserJun, year, month);

            month++;
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ProbationResignedJul, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ConfirmedResignedJul, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.NewHireJul, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ActiveUserJul, year, month);

            month++;
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ProbationResignedAug, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ConfirmedResignedAug, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.NewHireAug, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ActiveUserAug, year, month);

            month++;
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ProbationResignedSep, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ConfirmedResignedSep, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.NewHireSep, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ActiveUserSep, year, month);

            month++;
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ProbationResignedOct, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ConfirmedResignedOct, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.NewHireOct, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ActiveUserOct, year, month);

            month++;
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ProbationResignedNov, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ConfirmedResignedNov, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.NewHireNov, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ActiveUserNov, year, month);

            month++;
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ProbationResignedDec, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ConfirmedResignedDec, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.NewHireDec, year, month);
            SetValueForCellManpower(row.GetCell(columnIndex++), item.ActiveUserDec, year, month);
        }

        private void RenderFooterManpowerReport(IRow totalrow, ManpowerModel totalManpower, int year)
        {
            var columnIndex = 2;
            var month = 1;
            totalrow.GetCell(columnIndex++).SetCellValue(totalManpower.ProbationResigned);
            totalrow.GetCell(columnIndex++).SetCellValue(totalManpower.ConfirmedResigned);
            totalrow.GetCell(columnIndex++).SetCellValue(totalManpower.NewHire);
            totalrow.GetCell(columnIndex++).SetCellValue(totalManpower.ActiveUser);

            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ProbationResignedJan, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ConfirmedResignedJan, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.NewHireJan, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ActiveUserJan, year, month);

            month++;
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ProbationResignedFeb, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ConfirmedResignedFeb, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.NewHireFeb, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ActiveUserFeb, year, month);

            month++;
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ProbationResignedMar, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ConfirmedResignedMar, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.NewHireMar, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ActiveUserMar, year, month);

            month++;
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ProbationResignedApr, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ConfirmedResignedApr, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.NewHireApr, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ActiveUserApr, year, month);

            month++;
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ProbationResignedMay, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ConfirmedResignedMay, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.NewHireMay, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ActiveUserMay, year, month);

            month++;
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ProbationResignedJun, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ConfirmedResignedJun, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.NewHireJun, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ActiveUserJun, year, month);

            month++;
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ProbationResignedJul, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ConfirmedResignedJul, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.NewHireJul, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ActiveUserJul, year, month);

            month++;
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ProbationResignedAug, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ConfirmedResignedAug, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.NewHireAug, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ActiveUserAug, year, month);

            month++;
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ProbationResignedSep, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ConfirmedResignedSep, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.NewHireSep, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ActiveUserSep, year, month);

            month++;
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ProbationResignedOct, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ConfirmedResignedOct, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.NewHireOct, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ActiveUserOct, year, month);

            month++;
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ProbationResignedNov, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ConfirmedResignedNov, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.NewHireNov, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ActiveUserNov, year, month);

            month++;
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ProbationResignedDec, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ConfirmedResignedDec, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.NewHireDec, year, month);
            SetValueForCellManpower(totalrow.GetCell(columnIndex++), totalManpower.ActiveUserDec, year, month);
        }

        private void RenderBodyManpowerCategoryReport(IRow row, ManpowerCategoryModel item, int index)
        {
            var columnIndex = 0;

            row.GetCell(columnIndex++).SetCellValue(index);
            row.GetCell(columnIndex++).SetCellValue(item.GroupCode);

            row.GetCell(columnIndex++).SetCellValue(item.TotalExpat);
            row.GetCell(columnIndex++).SetCellValue(item.TotalLocal);
            row.GetCell(columnIndex++).SetCellValue(item.Total);
            row.GetCell(columnIndex++).SetCellValue(item.Management);
            row.GetCell(columnIndex++).SetCellValue(item.Management * 1.00 / item.Total);
            row.GetCell(columnIndex++).SetCellValue(item.ExecutiveWithPosition);
            row.GetCell(columnIndex++).SetCellValue(item.ExecutiveWithPosition * 1.00 / item.Total);
            row.GetCell(columnIndex++).SetCellValue(item.Executive);
            row.GetCell(columnIndex++).SetCellValue(item.Executive * 1.00 / item.Total);
            row.GetCell(columnIndex++).SetCellValue(item.Staff);
            row.GetCell(columnIndex++).SetCellValue(item.Staff * 1.00 / item.Total);
            row.GetCell(columnIndex++).SetCellValue(item.Worker);
            row.GetCell(columnIndex++).SetCellValue(item.Worker * 1.00 / item.Total);
        }

        private void RenderFooterManpowerCategoryReport(IRow totalrow, ManpowerCategoryModel totalManpower)
        {
            var columnIndex = 2;
            totalrow.GetCell(columnIndex++).SetCellValue(totalManpower.TotalExpat);
            totalrow.GetCell(columnIndex++).SetCellValue(totalManpower.TotalLocal);
            totalrow.GetCell(columnIndex++).SetCellValue(totalManpower.Total);
            totalrow.GetCell(columnIndex++).SetCellValue(totalManpower.Management);
            totalrow.GetCell(columnIndex++).SetCellValue(totalManpower.Management * 1.00 / totalManpower.Total);
            totalrow.GetCell(columnIndex++).SetCellValue(totalManpower.ExecutiveWithPosition);
            totalrow.GetCell(columnIndex++).SetCellValue(totalManpower.ExecutiveWithPosition * 1.00 / totalManpower.Total);
            totalrow.GetCell(columnIndex++).SetCellValue(totalManpower.Executive);
            totalrow.GetCell(columnIndex++).SetCellValue(totalManpower.Executive * 1.00 / totalManpower.Total);
            totalrow.GetCell(columnIndex++).SetCellValue(totalManpower.Staff);
            totalrow.GetCell(columnIndex++).SetCellValue(totalManpower.Staff * 1.00 / totalManpower.Total);
            totalrow.GetCell(columnIndex++).SetCellValue(totalManpower.Worker);
            totalrow.GetCell(columnIndex++).SetCellValue(totalManpower.Worker * 1.00 / totalManpower.Total);
        }

        private ManpowerModel AddTotalManpower(ManpowerModel totalManpower, ManpowerModel manpower)
        {
            totalManpower.ProbationResigned += manpower.ProbationResigned;
            totalManpower.ConfirmedResigned += manpower.ConfirmedResigned;
            totalManpower.NewHire += manpower.NewHire;
            totalManpower.ActiveUser += manpower.ActiveUser;

            totalManpower.ProbationResignedJan += manpower.ProbationResignedJan;
            totalManpower.ConfirmedResignedJan += manpower.ConfirmedResignedJan;
            totalManpower.NewHireJan += manpower.NewHireJan;
            totalManpower.ActiveUserJan += manpower.ActiveUserJan;

            totalManpower.ProbationResignedFeb += manpower.ProbationResignedFeb;
            totalManpower.ConfirmedResignedFeb += manpower.ConfirmedResignedFeb;
            totalManpower.NewHireFeb += manpower.NewHireFeb;
            totalManpower.ActiveUserFeb += manpower.ActiveUserFeb;

            totalManpower.ProbationResignedMar += manpower.ProbationResignedMar;
            totalManpower.ConfirmedResignedMar += manpower.ConfirmedResignedMar;
            totalManpower.NewHireMar += manpower.NewHireMar;
            totalManpower.ActiveUserMar += manpower.ActiveUserMar;

            totalManpower.ProbationResignedApr += manpower.ProbationResignedApr;
            totalManpower.ConfirmedResignedApr += manpower.ConfirmedResignedApr;
            totalManpower.NewHireApr += manpower.NewHireApr;
            totalManpower.ActiveUserApr += manpower.ActiveUserApr;

            totalManpower.ProbationResignedMay += manpower.ProbationResignedMay;
            totalManpower.ConfirmedResignedMay += manpower.ConfirmedResignedMay;
            totalManpower.NewHireMay += manpower.NewHireMay;
            totalManpower.ActiveUserMay += manpower.ActiveUserMay;

            totalManpower.ProbationResignedJun += manpower.ProbationResignedJun;
            totalManpower.ConfirmedResignedJun += manpower.ConfirmedResignedJun;
            totalManpower.NewHireJun += manpower.NewHireJun;
            totalManpower.ActiveUserJun += manpower.ActiveUserJun;

            totalManpower.ProbationResignedJul += manpower.ProbationResignedJul;
            totalManpower.ConfirmedResignedJul += manpower.ConfirmedResignedJul;
            totalManpower.NewHireJul += manpower.NewHireJul;
            totalManpower.ActiveUserJul += manpower.ActiveUserJul;

            totalManpower.ProbationResignedAug += manpower.ProbationResignedAug;
            totalManpower.ConfirmedResignedAug += manpower.ConfirmedResignedAug;
            totalManpower.NewHireAug += manpower.NewHireAug;
            totalManpower.ActiveUserAug += manpower.ActiveUserAug;

            totalManpower.ProbationResignedSep += manpower.ProbationResignedSep;
            totalManpower.ConfirmedResignedSep += manpower.ConfirmedResignedSep;
            totalManpower.NewHireSep += manpower.NewHireSep;
            totalManpower.ActiveUserSep += manpower.ActiveUserSep;

            totalManpower.ProbationResignedOct += manpower.ProbationResignedOct;
            totalManpower.ConfirmedResignedOct += manpower.ConfirmedResignedOct;
            totalManpower.NewHireOct += manpower.NewHireOct;
            totalManpower.ActiveUserOct += manpower.ActiveUserOct;

            totalManpower.ProbationResignedNov += manpower.ProbationResignedNov;
            totalManpower.ConfirmedResignedNov += manpower.ConfirmedResignedNov;
            totalManpower.NewHireNov += manpower.NewHireNov;
            totalManpower.ActiveUserNov += manpower.ActiveUserNov;

            totalManpower.ProbationResignedDec += manpower.ProbationResignedDec;
            totalManpower.ConfirmedResignedDec += manpower.ConfirmedResignedDec;
            totalManpower.NewHireDec += manpower.NewHireDec;
            totalManpower.ActiveUserDec += manpower.ActiveUserDec;

            return totalManpower;
        }

        private ManpowerCategoryModel AddTotalManpowerCategory(ManpowerCategoryModel totalManpower, ManpowerCategoryModel manpower)
        {
            totalManpower.TotalExpat += manpower.TotalExpat;
            totalManpower.TotalLocal += manpower.TotalLocal;
            totalManpower.Total += manpower.Total;
            totalManpower.Management += manpower.Management;
            totalManpower.ExecutiveWithPosition += manpower.ExecutiveWithPosition;
            totalManpower.Executive += manpower.Executive;
            totalManpower.Staff += manpower.Staff;
            totalManpower.Worker += manpower.Worker;

            return totalManpower;
        }
    }
}