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
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.ViewModels;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
    public class ExportExcelService : IExportExcelService
    {
        private const int _RowStartBodyLeaveSummary = 4;
        private const int _RowStartBodyWorkingTimeRequest = 3;
        private const int _RowStartBodyOvertimeReport = 4;
        private const int _RowStartBodyOvertimeRecord = 3;
        private const int _RowStartBodyWorkingCalendar = 5;
        private const int _RowStartBodyYearlyOvertimeReport = 5;

        private const string _LeaveSummaryFilePathExcelTemplate = "~\\ExcelTemplates\\LeaveSummaryTemplate.xls";
        private const string _WorkingTimeRequestFilePathExcelTemplate = "~\\ExcelTemplates\\WorkingTimeRequestTemplate.xls";
        private const string _EmployeeOvertimeRecordFilePathExcelTemplate = "~\\ExcelTemplates\\EmployeeOvertimeTemplate.xls";
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

        public HSSFWorkbook ExportEmployeeOvertimeRecord(IList<OvertimeExcelReport> overtimes, int? calendar, DateTime fromDate, DateTime toDate)
        {
            int totalDay = Convert.ToInt32((toDate - fromDate).TotalDays) + 1;
            HSSFWorkbook hssfworkbook;

            var templatePath = HttpContext.Current.Server.MapPath(_EmployeeOvertimeRecordFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            ISheet templateSheet = hssfworkbook.GetSheet("sheet1");
            ISheet sheet = hssfworkbook.CloneSheet("sheet1", "OvertimeRecord");

            //// render Header
            IRow rowHeaderTitle = sheet.GetRow(0);
            ICell cellHeader = rowHeaderTitle.GetCell(0);
            cellHeader.SetCellValue("OVERTIME RECORD - SMS STAFF Local From "
                + fromDate.ToString("dd MMM yyyy") + " - " + toDate.ToString("dd MMM yyyy"));

            IRow rowHeaderDate = sheet.GetRow(1);
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
                IRow rowHeaderOvertimeType = sheet.GetRow(2);
                OvertimeExcelReport headerRowEntity = overtimes.First();
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
                overtimes.RemoveAt(0);
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
                foreach (OvertimeExcelReportViewModel overtime in model)
                {
                    if (overtime.Name != null && overtime.ListOvertimeEntity.Any())
                    {
                        if (rowIndex <= _RowStartBodyOvertimeRecord)
                        {
                            sheet.CopyRow(templateSheet, rowTeamCopy, rowIndex);
                        }

                        IRow rowTeam = sheet.GetRow(rowIndex);
                        rowTeam.Height = rowTeamHeight;
                        ICell cellTeam = rowTeam.GetCell(0);
                        cellTeam.SetCellValue(overtime.Name.ToUpper());
                        sheet.MergeCells(rowIndex, 0, rowIndex, 2);
                        rowIndex++;
                        numberStart = rowIndex + 1;
                        foreach (OvertimeExcelReport item in overtime.ListOvertimeEntity)
                        {
                            var rowCopy = _RowStartBodyOvertimeRecord + (index % 2);
                            sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                            IRow row = sheet.GetRow(rowIndex);
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
                                OvertimeDailyReport data = item.OvertimeDailyReport[indexCount];
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

                        ICell cellCountMemberInTeam = rowTeam.GetCell(3);
                        cellCountMemberInTeam.SetCellValue(numberCount - 1);

                        sheet.CopyRow(templateSheet, rowTeamCopy, rowIndex);
                        IRow rowCalculateTotalOTOfTeam = sheet.GetRow(rowIndex);

                        ICell cellSumNormal = rowCalculateTotalOTOfTeam.GetCell(7);
                        ICell cellSumOff = rowCalculateTotalOTOfTeam.GetCell(8);
                        ICell cellSumPH = rowCalculateTotalOTOfTeam.GetCell(9);
                        ICell cellSumTax = rowCalculateTotalOTOfTeam.GetCell(10);
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

        public void ExportOvertimeReport(IList<OvertimeDaily> overtimeDetails, DateTime fromDate, DateTime toDate, string templatePath)
        {
            string fileName = "OvertimeReport_" + DateTime.Now.ToString("yyyyMMdd"),
                reportTitle = "Overtime Report Details" + " from " + fromDate.ToString("dd-MMM-yyyy") + " to " + toDate.ToString("dd-MMM-yyyy");
            var writer = new ExcelTemplateWriter(templatePath, fileName, "Overtime Detail Report");

            /// Fill report title.
            writer.WriteRow(0, new object[] { reportTitle });
            writer.CopyRows(1, 1);

            /// Fill report header.
            writer.CopyRows(2, 1);

            FillContentRowOvertimeDetailReport(overtimeDetails, ref writer);
            writer.SyncColumnsWidth(14);
            writer.CreateFreezePane(0, 3);

            writer.Render();
        }

        public void ExportYearlyOvertimeReport(IList<OvertimeAnnualReportViewModel> overtimeDetails, int year, int? calendar, string templatePath)
        {
            string fileName = "YearlyOvertimeReport_" + DateTime.Now.ToString("yyyyMMdd"),
                calendarType = (calendar.HasValue && calendar.Value == 1) ? "Fiscal Calendar" : "Normal Calendar";
            var writer = new ExcelTemplateWriter(templatePath, fileName, "Yearly Overtime Report");

            /// Fill report title.
            writer.CopyRows(0, 2);

            /// Fill report header.
            writer.CopyRows(2, 2);
            writer.WriteCellRange(2, 1, 4, 12, "Overtime Summary In " + year + " (" + calendarType + ")");

            FillContentRowYearlyOvertimeReport(overtimeDetails, ref writer);
            FillContentRowFooterYearlyOvertimeReport(overtimeDetails, ref writer);
            writer.SyncColumnsWidth(17);
            writer.CreateFreezePane(0, 4);

            writer.Render();
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
            string fileName = "EmployeeDetailsInfo_" + DateTime.Now.ToString("yyyyMMdd");
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

            sheet.CreateFreezePane(0, 5);
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

        #region Overtime Detail

        private void FillContentRowOvertimeDetailReport(IList<OvertimeDaily> overtimeDetails, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (OvertimeDaily overtimeDetail in overtimeDetails)
            {
                FillRowOvertimeDetailReport(overtimeDetail, ref writer, ref rowIndex);
            }
        }

        private void FillRowOvertimeDetailReport(OvertimeDaily overtimeDetail, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordOvertimeDetailReport(overtimeDetail, rowIndex);
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

        private object CreateRecordOvertimeDetailReport(OvertimeDaily overtimeDetail, int rowIndex)
        {
            CurrentRequestStatus status = (CurrentRequestStatus)overtimeDetail.Status;

            var record = new
            {
                Count = rowIndex,
                Date = overtimeDetail.OvertimeDate.ToDateMonthYearFormatedDate(),
                Code = overtimeDetail.Code,
                Employee = overtimeDetail.FullName + " (" + overtimeDetail.NickName + ")",
                Team = overtimeDetail.Team,
                Schedule = overtimeDetail.Schedule,
                TimeIn = overtimeDetail.TimeIn,
                TimeOut = overtimeDetail.TimeOut,
                TotalHours = Math.Round(overtimeDetail.TotalWorkHours, 2),
                NormalDay = overtimeDetail.NormalHours,
                OffDay = overtimeDetail.OffHours,
                PublicHoliday = overtimeDetail.PublicHours,
                Remark = overtimeDetail.Remark,
                Status = status.ToString()
            };

            return record;
        }

        #endregion Overtime Detail

        #region Yearly Overtime

        private void FillContentRowYearlyOvertimeReport(IList<OvertimeAnnualReportViewModel> overtimeDetails, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (OvertimeAnnualReportViewModel overtimeDetail in overtimeDetails)
            {
                FillRowYearlyOvertimeReport(overtimeDetail, ref writer, ref rowIndex);
            }
        }

        private void FillRowYearlyOvertimeReport(OvertimeAnnualReportViewModel overtimeDetail, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordYearlyOvertimeReport(overtimeDetail, rowIndex);
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

        private object CreateRecordYearlyOvertimeReport(OvertimeAnnualReportViewModel overtimeDetail, int rowIndex)
        {
            double? nullValue = null,
                totalHours = overtimeDetail.Jan + overtimeDetail.Feb + overtimeDetail.Mar + overtimeDetail.Apr
                    + overtimeDetail.May + overtimeDetail.Jun + overtimeDetail.Jul + overtimeDetail.Aug
                    + overtimeDetail.Sep + overtimeDetail.Oct + overtimeDetail.Nov + overtimeDetail.Dec;

            var record = new
            {
                Count = rowIndex,
                Section = overtimeDetail.Team,
                FullName = overtimeDetail.FullName,
                NickName = overtimeDetail.NickName,
                Jan = overtimeDetail.Jan != 0 ? (overtimeDetail.Jan * 1.00) : nullValue,
                Feb = overtimeDetail.Feb != 0 ? (overtimeDetail.Feb * 1.00) : nullValue,
                Mar = overtimeDetail.Mar != 0 ? (overtimeDetail.Mar * 1.00) : nullValue,
                Apr = overtimeDetail.Apr != 0 ? (overtimeDetail.Apr * 1.00) : nullValue,
                May = overtimeDetail.May != 0 ? (overtimeDetail.May * 1.00) : nullValue,
                Jun = overtimeDetail.Jun != 0 ? (overtimeDetail.Jun * 1.00) : nullValue,
                Jul = overtimeDetail.Jul != 0 ? (overtimeDetail.Jul * 1.00) : nullValue,
                Aug = overtimeDetail.Aug != 0 ? (overtimeDetail.Aug * 1.00) : nullValue,
                Sep = overtimeDetail.Sep != 0 ? (overtimeDetail.Sep * 1.00) : nullValue,
                Oct = overtimeDetail.Oct != 0 ? (overtimeDetail.Oct * 1.00) : nullValue,
                Nov = overtimeDetail.Nov != 0 ? (overtimeDetail.Nov * 1.00) : nullValue,
                Dec = overtimeDetail.Dec != 0 ? (overtimeDetail.Dec * 1.00) : nullValue,
                TotalHours = totalHours != 0 ? (totalHours * 1.00) : nullValue
            };

            return record;
        }

        private void FillContentRowFooterYearlyOvertimeReport(IList<OvertimeAnnualReportViewModel> overtimeDetails, ref ExcelTemplateWriter writer)
        {
            float jan = overtimeDetails.Sum(x => x.Jan),
                feb = overtimeDetails.Sum(x => x.Feb),
                mar = overtimeDetails.Sum(x => x.Mar),
                apr = overtimeDetails.Sum(x => x.Apr),
                may = overtimeDetails.Sum(x => x.May),
                jun = overtimeDetails.Sum(x => x.Jun),
                jul = overtimeDetails.Sum(x => x.Jul),
                aug = overtimeDetails.Sum(x => x.Aug),
                sep = overtimeDetails.Sum(x => x.Sep),
                oct = overtimeDetails.Sum(x => x.Oct),
                nov = overtimeDetails.Sum(x => x.Nov),
                dec = overtimeDetails.Sum(x => x.Dec);

            var record = new
            {
                Count = Labels.TotalHours,
                Section = string.Empty,
                FullName = string.Empty,
                NickName = string.Empty,
                Jan = jan * 1.00,
                Feb = feb * 1.00,
                Mar = mar * 1.00,
                Apr = apr * 1.00,
                May = may * 1.00,
                Jun = jun * 1.00,
                Jul = jul * 1.00,
                Aug = aug * 1.00,
                Sep = sep * 1.00,
                Oct = oct * 1.00,
                Nov = nov * 1.00,
                Dec = dec * 1.00,
                TotalHours = (jan + feb + mar + apr + may + jun + jul + aug + sep + oct + nov + dec) * 1.00
            };

            writer.WriteRow(6, record);
        }

        #endregion Yearly Overtime

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