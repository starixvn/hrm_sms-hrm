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

        private const string _AttendanceFilePathExcelTemplate = "~\\ExcelTemplates\\AttendanceReportTemplate.xls";
        private const string _LeaveSummaryFilePathExcelTemplate = "~\\ExcelTemplates\\LeaveSummaryTemplate.xls";
        private const string _UnpaidLeaveSummaryFilePathExcelTemplate = "~\\ExcelTemplates\\UnpaidLeaveSummaryTemplate.xls";
        private const string _WorkingTimeRequestFilePathExcelTemplate = "~\\ExcelTemplates\\WorkingTimeRequestTemplate.xls";
        private const string _OvertimeReportFilePathExcelTemplate = "~\\ExcelTemplates\\OvertimeReportTemplate.xls";
        private const string _YearlyOvertimeReportFilePathExcelTemplate = "~\\ExcelTemplates\\YearlyOvertimeReportTemplate.xls";
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

            /// Fill report title.
            writer.CopyRows(0, 2);
            /// Fill report header.
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

        public void ExportManpower(IList<ManpowerModel> manpowers, int year, string templatePath)
        {
            string fileName = "ManpowerReport_" + DateTime.Now.ToString("yyyyMMdd"),
                reportTitle = Labels.ManpowerReportTitle + " " + year;
            var writer = new ExcelTemplateWriter(templatePath, fileName, "Manpower Report");

            /// Fill report title.
            writer.WriteRow(0, new object[] { reportTitle });
            writer.CopyRows(1, 1);

            /// Fill report header.
            writer.CopyRows(2, 2);

            FillContentRowManpowerReport(manpowers, year, ref writer);
            FillContentRowFooterManpowerReport(manpowers, year, ref writer);
            writer.SyncColumnsWidth(54);
            writer.CreateFreezePane(0, 4);

            writer.Render();
        }

        public void ExportManpowerCategory(IList<ManpowerCategoryModel> manpowers, int year, string templatePath)
        {
            string fileName = "ManpowerCategoryReport_" + DateTime.Now.ToString("yyyyMMdd"),
                reportTitle = Labels.ManpowerCategoryReportTitle + " " + year;
            var writer = new ExcelTemplateWriter(templatePath, fileName, "Manpower Report");

            /// Fill report title.
            writer.WriteRow(0, new object[] { reportTitle });
            writer.CopyRows(1, 1);

            /// Fill report header.
            writer.CopyRows(2, 2);

            FillContentRowManpowerCategoryReport(manpowers, ref writer);
            FillContentRowFooterManpowerCategoryReport(manpowers, ref writer);
            writer.SyncColumnsWidth(10);
            writer.CreateFreezePane(0, 4);

            writer.Render();
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

        public void ExportEmployeeBasicInfo(IList<Employee> employees, string templatePath)
        {
            string fileName = "EmployeeBasicInfo_" + DateTime.Now.ToString("yyyyMMdd");
            var writer = new ExcelTemplateWriter(templatePath, fileName, "Employee Basic Info");

            /// Fill report title.
            writer.CopyRows(0, 2);
            /// Fill report header.
            writer.CopyRows(2, 1);

            FillContentRowEmployeeBasicInfoReport(employees, ref writer);
            writer.SyncColumnsWidth(16);
            writer.CreateFreezePane(0, 3);

            writer.Render();
        }

        public void ExportEmployeeDetailsInfo(IList<Employee> employees, string templatePath)
        {
            string fileName = "EmployeeDetailsInfo_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            var writer = new ExcelTemplateWriter(templatePath, fileName, "Employee Details Info");

            /// Fill report title.
            writer.CopyRows(0, 2);
            /// Fill report header.
            writer.CopyRows(2, 1);

            FillContentRowEmployeeDetailsInfoReport(employees, ref writer);
            writer.SyncColumnsWidth(42);
            writer.CreateFreezePane(0, 3);

            writer.Render();
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

        #region Employee Basic Info

        private void FillContentRowEmployeeBasicInfoReport(IList<Employee> employees, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (Employee employee in employees)
            {
                FillRowEmployeeBasicInfoReport(employee, ref writer, ref rowIndex);
            }
        }

        private void FillRowEmployeeBasicInfoReport(Employee employee, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordEmployeeBasicInfoReport(employee, rowIndex);
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

        private object CreateRecordEmployeeBasicInfoReport(Employee employee, int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                Code = employee.Code,
                FullName = employee.FullName,
                NickName = employee.NickName,
                Section = employee.Team.Name,
                GroupDate = employee.JoinedDate.HasValue ? employee.JoinedDate.Value.ToDateMonthYearFormatedDate() : string.Empty,
                CompanyDate = employee.CompanyDate.HasValue ? employee.CompanyDate.Value.ToDateMonthYearFormatedDate() : string.Empty,
                ConfirmedDate = employee.ConfirmedDate.HasValue ? employee.ConfirmedDate.Value.ToDateMonthYearFormatedDate() : string.Empty,
                ResignedDate = employee.ResignedDate.HasValue ? employee.ResignedDate.Value.ToDateMonthYearFormatedDate() : string.Empty,
                DoB = employee.DateOfBirth.ToDateMonthYearFormatedDate(),
                Gender = employee.Gender == 0 ? "F" : "M",
                CellPhone = employee.Phone,
                Email = employee.Email,
                CurrentAddress = employee.CurrentAddress,
                Unit = employee.Unit,
                Category = employee.IsLocal ? Labels.Local : Labels.Expat
            };

            return record;
        }

        #endregion Employee Basic Info

        #region Employee Details Info

        private void FillContentRowEmployeeDetailsInfoReport(IList<Employee> employees, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (Employee employee in employees)
            {
                FillRowEmployeeDetailsInfoReport(employee, ref writer, ref rowIndex);
            }
        }

        private void FillRowEmployeeDetailsInfoReport(Employee employee, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordEmployeeDetailsInfoReport(employee, rowIndex);
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

        private object CreateRecordEmployeeDetailsInfoReport(Employee employee, int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                Code = employee.Code,
                DomainUsername = employee.DomainUsername,
                FullName = employee.FullName,
                NickName = employee.NickName,
                Section = employee.Team.Name,
                DoB = employee.DateOfBirth.ToDateMonthYearFormatedDate(),
                PlaceOfBirth = employee.PlaceOfBirth,
                Ethnic = employee.Ethnic,
                Native = employee.Native,
                Religion = employee.Religion,
                Gender = employee.Gender == 0 ? "F" : "M",
                MarriageName = employee.MarriageName,
                CountryName = employee.CountryName,
                CardNo = employee.CardNo,
                DateOfIdCard = employee.DateOfIdCard.HasValue ? employee.DateOfIdCard.Value.ToDateMonthYearFormatedDate() : string.Empty,
                PlaceOfIdCard = employee.PlaceOfIdCard,
                PassportNo = employee.PassportNo,
                PassportExp = employee.PassportExp.HasValue ? employee.PassportExp.Value.ToDateMonthYearFormatedDate() : string.Empty,
                Qualification = employee.Qualification,
                Major = employee.Major,
                Phone = employee.Phone,
                HomePhone = employee.HomePhone,
                Email = employee.Email,
                Address = employee.Address,
                CurrentAddress = employee.CurrentAddress,
                Skype = employee.Skype,
                Unit = employee.Unit,
                JobTitle = employee.JobTitle,
                GroupDate = employee.JoinedDate.HasValue ? employee.JoinedDate.Value.ToDateMonthYearFormatedDate() : string.Empty,
                CompanyDate = employee.CompanyDate.HasValue ? employee.CompanyDate.Value.ToDateMonthYearFormatedDate() : string.Empty,
                ConfirmedDate = employee.ConfirmedDate.HasValue ? employee.ConfirmedDate.Value.ToDateMonthYearFormatedDate() : string.Empty,
                ResignedDate = employee.ResignedDate.HasValue ? employee.ResignedDate.Value.ToDateMonthYearFormatedDate() : string.Empty,
                WorkingDays = employee.WorkingDays,
                WorkingHours = employee.WorkingHours,
                OffDaysbyWeek = employee.OffDaysbyWeek,
                AnnualLeaveBalance = employee.AnnualLeaveBalance,
                Category = employee.IsLocal ? Labels.Local : Labels.Expat,
                BankAccount = employee.BankAccount,
                TaxCode = employee.TaxCode,
                SocialInsuranceNo = employee.SocialInsuranceNo,
                RegisteredHospital = employee.RegisteredHospital
            };

            return record;
        }

        #endregion Employee Details Info

        #region Employee Supervisor

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
            object record = CreateRecordEmployeeSupervisorReport(supervisor, rowIndex);
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

        private object CreateRecordEmployeeSupervisorReport(SupervisorReportModel supervisor, int rowIndex)
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

        #endregion Employee Supervisor

        #region Section Supervisor

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
            object record = CreateRecordSectionSupervisorReport(supervisor, rowIndex);
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

        private object CreateRecordSectionSupervisorReport(SectionSupervisorReportModel supervisor, int rowIndex)
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

        #endregion Section Supervisor

        #region Manpower

        private void FillContentRowManpowerReport(IList<ManpowerModel> manpowers, int year, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (ManpowerModel manpower in manpowers)
            {
                FillRowManpowerReport(manpower, year, ref writer, ref rowIndex);
            }
        }

        private void FillRowManpowerReport(ManpowerModel manpower, int year, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordManpowerReport(manpower, year, rowIndex);
            if (rowIndex % 2 == 1)
            {
                writer.WriteRow(4, record);
            }
            else
            {
                writer.WriteRow(5, record);
            }

            rowIndex++;
        }

        private dynamic GetValueForCellManpower(int value, int year, int month)
        {
            bool isShowValue = false;
            dynamic result;

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
                result = value;
            }
            else
            {
                result = "-";
            }

            return result;
        }

        private object CreateRecordManpowerReport(ManpowerModel manpower, int year, int rowIndex)
        {
            int month = 1;
            var record = new
            {
                Count = rowIndex,
                GroupCode = manpower.GroupCode,
                ProbationResigned = manpower.ProbationResigned,
                ConfirmedResigned = manpower.ConfirmedResigned,
                NewHire = manpower.NewHire,
                ActiveUser = manpower.ActiveUser,
                ProbationResignedJan = GetValueForCellManpower(manpower.ProbationResignedJan, year, month),
                ConfirmedResignedJan = GetValueForCellManpower(manpower.ConfirmedResignedJan, year, month),
                NewHireJan = GetValueForCellManpower(manpower.NewHireJan, year, month),
                ActiveUserJan = GetValueForCellManpower(manpower.ActiveUserJan, year, month),
                ProbationResignedFeb = GetValueForCellManpower(manpower.ProbationResignedFeb, year, ++month),
                ConfirmedResignedFeb = GetValueForCellManpower(manpower.ConfirmedResignedFeb, year, month),
                NewHireFeb = GetValueForCellManpower(manpower.NewHireFeb, year, month),
                ActiveUserFeb = GetValueForCellManpower(manpower.ActiveUserFeb, year, month),
                ProbationResignedMar = GetValueForCellManpower(manpower.ProbationResignedMar, year, ++month),
                ConfirmedResignedMar = GetValueForCellManpower(manpower.ConfirmedResignedMar, year, month),
                NewHireMar = GetValueForCellManpower(manpower.NewHireMar, year, month),
                ActiveUserMar = GetValueForCellManpower(manpower.ActiveUserMar, year, month),
                ProbationResignedApr = GetValueForCellManpower(manpower.ProbationResignedApr, year, ++month),
                ConfirmedResignedApr = GetValueForCellManpower(manpower.ConfirmedResignedApr, year, month),
                NewHireApr = GetValueForCellManpower(manpower.NewHireApr, year, month),
                ActiveUserApr = GetValueForCellManpower(manpower.ActiveUserApr, year, month),
                ProbationResignedMay = GetValueForCellManpower(manpower.ProbationResignedMay, year, ++month),
                ConfirmedResignedMay = GetValueForCellManpower(manpower.ConfirmedResignedMay, year, month),
                NewHireMay = GetValueForCellManpower(manpower.NewHireMay, year, month),
                ActiveUserMay = GetValueForCellManpower(manpower.ActiveUserMay, year, month),
                ProbationResignedJun = GetValueForCellManpower(manpower.ProbationResignedJun, year, ++month),
                ConfirmedResignedJun = GetValueForCellManpower(manpower.ConfirmedResignedJun, year, month),
                NewHireJun = GetValueForCellManpower(manpower.NewHireJun, year, month),
                ActiveUserJun = GetValueForCellManpower(manpower.ActiveUserJun, year, month),
                ProbationResignedJul = GetValueForCellManpower(manpower.ProbationResignedJul, year, ++month),
                ConfirmedResignedJul = GetValueForCellManpower(manpower.ConfirmedResignedJul, year, month),
                NewHireJul = GetValueForCellManpower(manpower.NewHireJul, year, month),
                ActiveUserJul = GetValueForCellManpower(manpower.ActiveUserJul, year, month),
                ProbationResignedAug = GetValueForCellManpower(manpower.ProbationResignedAug, year, ++month),
                ConfirmedResignedAug = GetValueForCellManpower(manpower.ConfirmedResignedAug, year, month),
                NewHireAug = GetValueForCellManpower(manpower.NewHireAug, year, month),
                ActiveUserAug = GetValueForCellManpower(manpower.ActiveUserAug, year, month),
                ProbationResignedSep = GetValueForCellManpower(manpower.ProbationResignedSep, year, ++month),
                ConfirmedResignedSep = GetValueForCellManpower(manpower.ConfirmedResignedSep, year, month),
                NewHireSep = GetValueForCellManpower(manpower.NewHireSep, year, month),
                ActiveUserSep = GetValueForCellManpower(manpower.ActiveUserSep, year, month),
                ProbationResignedOct = GetValueForCellManpower(manpower.ProbationResignedOct, year, ++month),
                ConfirmedResignedOct = GetValueForCellManpower(manpower.ConfirmedResignedOct, year, month),
                NewHireOct = GetValueForCellManpower(manpower.NewHireOct, year, month),
                ActiveUserOct = GetValueForCellManpower(manpower.ActiveUserOct, year, month),
                ProbationResignedNov = GetValueForCellManpower(manpower.ProbationResignedNov, year, ++month),
                ConfirmedResignedNov = GetValueForCellManpower(manpower.ConfirmedResignedNov, year, month),
                NewHireNov = GetValueForCellManpower(manpower.NewHireNov, year, month),
                ActiveUserNov = GetValueForCellManpower(manpower.ActiveUserNov, year, month),
                ProbationResignedDec = GetValueForCellManpower(manpower.ProbationResignedDec, year, ++month),
                ConfirmedResignedDec = GetValueForCellManpower(manpower.ConfirmedResignedDec, year, month),
                NewHireDec = GetValueForCellManpower(manpower.NewHireDec, year, month),
                ActiveUserDec = GetValueForCellManpower(manpower.ActiveUserDec, year, month)
            };

            return record;
        }

        private void FillContentRowFooterManpowerReport(IList<ManpowerModel> manpowers, int year, ref ExcelTemplateWriter writer)
        {
            int month = 1;
            var record = new
            {
                Count = Labels.Total,
                GroupCode = string.Empty,
                ProbationResigned = manpowers.Sum(t => t.ProbationResigned),
                ConfirmedResigned = manpowers.Sum(t => t.ConfirmedResigned),
                NewHire = manpowers.Sum(t => t.NewHire),
                ActiveUser = manpowers.Sum(t => t.ActiveUser),
                ProbationResignedJan = GetValueForCellManpower(manpowers.Sum(t => t.ProbationResignedJan), year, month),
                ConfirmedResignedJan = GetValueForCellManpower(manpowers.Sum(t => t.ConfirmedResignedJan), year, month),
                NewHireJan = GetValueForCellManpower(manpowers.Sum(t => t.NewHireJan), year, month),
                ActiveUserJan = GetValueForCellManpower(manpowers.Sum(t => t.ActiveUserJan), year, month),
                ProbationResignedFeb = GetValueForCellManpower(manpowers.Sum(t => t.ProbationResignedFeb), year, ++month),
                ConfirmedResignedFeb = GetValueForCellManpower(manpowers.Sum(t => t.ConfirmedResignedFeb), year, month),
                NewHireFeb = GetValueForCellManpower(manpowers.Sum(t => t.NewHireFeb), year, month),
                ActiveUserFeb = GetValueForCellManpower(manpowers.Sum(t => t.ActiveUserFeb), year, month),
                ProbationResignedMar = GetValueForCellManpower(manpowers.Sum(t => t.ProbationResignedMar), year, ++month),
                ConfirmedResignedMar = GetValueForCellManpower(manpowers.Sum(t => t.ConfirmedResignedMar), year, month),
                NewHireMar = GetValueForCellManpower(manpowers.Sum(t => t.NewHireMar), year, month),
                ActiveUserMar = GetValueForCellManpower(manpowers.Sum(t => t.ActiveUserMar), year, month),
                ProbationResignedApr = GetValueForCellManpower(manpowers.Sum(t => t.ProbationResignedApr), year, ++month),
                ConfirmedResignedApr = GetValueForCellManpower(manpowers.Sum(t => t.ConfirmedResignedApr), year, month),
                NewHireApr = GetValueForCellManpower(manpowers.Sum(t => t.NewHireApr), year, month),
                ActiveUserApr = GetValueForCellManpower(manpowers.Sum(t => t.ActiveUserApr), year, month),
                ProbationResignedMay = GetValueForCellManpower(manpowers.Sum(t => t.ProbationResignedMay), year, ++month),
                ConfirmedResignedMay = GetValueForCellManpower(manpowers.Sum(t => t.ConfirmedResignedMay), year, month),
                NewHireMay = GetValueForCellManpower(manpowers.Sum(t => t.NewHireMay), year, month),
                ActiveUserMay = GetValueForCellManpower(manpowers.Sum(t => t.ActiveUserMay), year, month),
                ProbationResignedJun = GetValueForCellManpower(manpowers.Sum(t => t.ProbationResignedJun), year, ++month),
                ConfirmedResignedJun = GetValueForCellManpower(manpowers.Sum(t => t.ConfirmedResignedJun), year, month),
                NewHireJun = GetValueForCellManpower(manpowers.Sum(t => t.NewHireJun), year, month),
                ActiveUserJun = GetValueForCellManpower(manpowers.Sum(t => t.ActiveUserJun), year, month),
                ProbationResignedJul = GetValueForCellManpower(manpowers.Sum(t => t.ProbationResignedJul), year, ++month),
                ConfirmedResignedJul = GetValueForCellManpower(manpowers.Sum(t => t.ConfirmedResignedJul), year, month),
                NewHireJul = GetValueForCellManpower(manpowers.Sum(t => t.NewHireJul), year, month),
                ActiveUserJul = GetValueForCellManpower(manpowers.Sum(t => t.ActiveUserJul), year, month),
                ProbationResignedAug = GetValueForCellManpower(manpowers.Sum(t => t.ProbationResignedAug), year, ++month),
                ConfirmedResignedAug = GetValueForCellManpower(manpowers.Sum(t => t.ConfirmedResignedAug), year, month),
                NewHireAug = GetValueForCellManpower(manpowers.Sum(t => t.NewHireAug), year, month),
                ActiveUserAug = GetValueForCellManpower(manpowers.Sum(t => t.ActiveUserAug), year, month),
                ProbationResignedSep = GetValueForCellManpower(manpowers.Sum(t => t.ProbationResignedSep), year, ++month),
                ConfirmedResignedSep = GetValueForCellManpower(manpowers.Sum(t => t.ConfirmedResignedSep), year, month),
                NewHireSep = GetValueForCellManpower(manpowers.Sum(t => t.NewHireSep), year, month),
                ActiveUserSep = GetValueForCellManpower(manpowers.Sum(t => t.ActiveUserSep), year, month),
                ProbationResignedOct = GetValueForCellManpower(manpowers.Sum(t => t.ProbationResignedOct), year, ++month),
                ConfirmedResignedOct = GetValueForCellManpower(manpowers.Sum(t => t.ConfirmedResignedOct), year, month),
                NewHireOct = GetValueForCellManpower(manpowers.Sum(t => t.NewHireOct), year, month),
                ActiveUserOct = GetValueForCellManpower(manpowers.Sum(t => t.ActiveUserOct), year, month),
                ProbationResignedNov = GetValueForCellManpower(manpowers.Sum(t => t.ProbationResignedNov), year, ++month),
                ConfirmedResignedNov = GetValueForCellManpower(manpowers.Sum(t => t.ConfirmedResignedNov), year, month),
                NewHireNov = GetValueForCellManpower(manpowers.Sum(t => t.NewHireNov), year, month),
                ActiveUserNov = GetValueForCellManpower(manpowers.Sum(t => t.ActiveUserNov), year, month),
                ProbationResignedDec = GetValueForCellManpower(manpowers.Sum(t => t.ProbationResignedDec), year, ++month),
                ConfirmedResignedDec = GetValueForCellManpower(manpowers.Sum(t => t.ConfirmedResignedDec), year, month),
                NewHireDec = GetValueForCellManpower(manpowers.Sum(t => t.NewHireDec), year, month),
                ActiveUserDec = GetValueForCellManpower(manpowers.Sum(t => t.ActiveUserDec), year, month)
            };

            writer.WriteRow(6, record);
        }

        #endregion Manpower

        #region Manpower Category

        private void FillContentRowManpowerCategoryReport(IList<ManpowerCategoryModel> manpowers, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (ManpowerCategoryModel manpower in manpowers)
            {
                FillRowManpowerCategoryReport(manpower, ref writer, ref rowIndex);
            }
        }

        private void FillRowManpowerCategoryReport(ManpowerCategoryModel manpower, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordManpowerCategoryReport(manpower, rowIndex);
            if (rowIndex % 2 == 1)
            {
                writer.WriteRow(4, record);
            }
            else
            {
                writer.WriteRow(5, record);
            }

            rowIndex++;
        }

        private object CreateRecordManpowerCategoryReport(ManpowerCategoryModel manpower, int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                GroupCode = manpower.GroupCode,
                TotalExpat = manpower.TotalExpat,
                TotalLocal = manpower.TotalLocal,
                Total = manpower.Total,
                Management = manpower.Management,
                AVGManagement = (manpower.Management * 1.00) / manpower.Total,
                ExecutiveWithPosition = manpower.ExecutiveWithPosition,
                AVGExecutiveWithPosition = (manpower.ExecutiveWithPosition * 1.00) / manpower.Total,
                Executive = manpower.Executive,
                AVGExecutive = (manpower.Executive * 1.00) / manpower.Total,
                Staff = manpower.Staff,
                AVGStaff = (manpower.Staff * 1.00) / manpower.Total,
                Worker = manpower.Worker,
                AVGWorker = (manpower.Worker * 1.00) / manpower.Total
            };

            return record;
        }

        private void FillContentRowFooterManpowerCategoryReport(IList<ManpowerCategoryModel> manpowers, ref ExcelTemplateWriter writer)
        {
            var record = new
            {
                Count = Labels.Total,
                GroupCode = string.Empty,
                TotalExpat = manpowers.Sum(t => t.TotalExpat),
                TotalLocal = manpowers.Sum(t => t.TotalLocal),
                Total = manpowers.Sum(t => t.Total),
                Management = manpowers.Sum(t => t.Management),
                AVGManagement = (manpowers.Sum(t => t.Management) * 1.00) / manpowers.Sum(t => t.Total),
                ExecutiveWithPosition = manpowers.Sum(t => t.ExecutiveWithPosition),
                AVGExecutiveWithPosition = (manpowers.Sum(t => t.ExecutiveWithPosition) * 1.00) / manpowers.Sum(t => t.Total),
                Executive = manpowers.Sum(t => t.Executive),
                AVGExecutive = (manpowers.Sum(t => t.Executive) * 1.00) / manpowers.Sum(t => t.Total),
                Staff = manpowers.Sum(t => t.Staff),
                AVGStaff = (manpowers.Sum(t => t.Staff) * 1.00) / manpowers.Sum(t => t.Total),
                Worker = manpowers.Sum(t => t.Worker),
                AVGWorker = (manpowers.Sum(t => t.Worker) * 1.00) / manpowers.Sum(t => t.Total)
            };

            writer.WriteRow(6, record);
        }

        #endregion Manpower Category

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
    }
}