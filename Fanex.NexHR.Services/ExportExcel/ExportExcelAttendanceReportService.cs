namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.ViewModels;
    using Models;
    using Models.Request;

    public interface IExportExcelAttendanceReportService
    {
        void Export(IList<EmployeeAttendanceModel> attendances, DateTime fromDate, DateTime toDate);
    }

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    public class ExportExcelAttendanceReportService : IExportExcelAttendanceReportService
    {
        private const int RowStartHeader = 2;
        private const int RowStartBody = 3;
        private const int RowHighlight = 5;
        private const int RowOffdate = 6;
        private const int ShiftCodeColumn = 6;
        private const int AttendanceDateColumn = 1;
        private const int RowWeekend = 7;

        private const string AttendanceTemplatePath = @"~\ExcelTemplates\AttendanceReportTemplate.xls";

        public void Export(IList<EmployeeAttendanceModel> attendances, DateTime fromDate, DateTime toDate)
        {
            const int columns = 17;
            string templatePath = HttpContext.Current.Server.MapPath(AttendanceTemplatePath);
            string fileName = "AttendanceReport_" + DateTime.Now.ToString("yyyyMMdd");

            var writer = new ExcelTemplateWriter(templatePath, fileName, "Attendance Report");

            /// Fill report title.
            writer.CopyRows(0, 2);

            /// Fill report header.
            writer.CopyRows(RowStartHeader, 1);

            FillContentRowReport(attendances, ref writer);

            writer.SyncColumnsWidth(columns);
            writer.CreateFreezePane(0, RowStartBody);

            writer.Render();
        }

        private void FillContentRowReport(IList<EmployeeAttendanceModel> attendances, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (EmployeeAttendanceModel attendance in attendances)
            {
                FillRowReport(attendance, ref writer, ref rowIndex);
            }
        }

        private void FillRowReport(EmployeeAttendanceModel attendance, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordReport(attendance, rowIndex);

            writer.WriteRow(rowIndex % 2 != 0 ? RowStartBody : RowStartBody + 1, record);

            SetColorHighlightRow(attendance, ref writer, rowIndex);
            rowIndex++;
        }

        private static void SetColorHighlightRow(EmployeeAttendanceModel attendance, ref ExcelTemplateWriter writer, int rowIndex)
        {
            if (attendance.IsOffdayOT)
            {
                writer.CopyCellStyle(RowHighlight, ShiftCodeColumn, rowIndex + RowStartHeader, desCol: ShiftCodeColumn);
            }

            if (attendance.IsPublicHoliday && !string.IsNullOrEmpty(attendance.ShiftCode))
            {
                writer.CopyCellStyle(RowOffdate, ShiftCodeColumn, rowIndex + RowStartHeader, desCol: ShiftCodeColumn);
            }

            if (attendance.IsWeekend)
            {
                writer.CopyCellStyle(RowWeekend, AttendanceDateColumn, rowIndex + RowStartHeader, desCol: AttendanceDateColumn);
            }
        }

        private object CreateRecordReport(EmployeeAttendanceModel attendance, int rowIndex)
        {
            string requests = RequestsRemark(attendance);

            string shiftCode = !string.IsNullOrEmpty(attendance.ShiftCode) ? attendance.ShiftCode : string.Empty;
            string totalWorkHours = attendance.TotalWorkHours != 0 ? Utilities.TotalHoursToTime((float)attendance.TotalWorkHours) : string.Empty;
            string overtime = attendance.IsOvertime ? attendance.FormattedOvertimeStatus + " (" + attendance.OvertimeHours.ToString() + ")" : string.Empty;

            var record = new
            {
                Count = rowIndex,
                attendance.AttendanceDate,
                attendance.DepartmentCode,
                attendance.SectionCode,
                attendance.TeamCode,
                attendance.Code,
                attendance.FullName,
                attendance.NickName,
                ShiftCode = shiftCode,
                attendance.InFixed,
                attendance.OutFixed,
                TotalWorkHours = totalWorkHours,
                LateIn = Utilities.MinuteToTime(attendance.LateIn),
                EarlyOut = attendance.IsOffDay == 1 && !string.IsNullOrWhiteSpace(shiftCode) ?
                                string.Empty : Utilities.MinuteToTime(attendance.EarlyOut),
                Remark = requests,
                Overtime = overtime,
                attendance.Notes
            };

            return record;
        }

        private static string RequestsRemark(EmployeeAttendanceModel attendance)
        {
            var requests = string.Empty;
            StringBuilder requestsBuilder = new StringBuilder();

            if (attendance.RequestList != null && attendance.RequestList.Any())
            {
                foreach (EmployeeAttendanceRequest item in attendance.RequestList)
                {
                    var actualLateInErlyOut = item.RequestType.Id == (byte)RequestType.LateIn ? attendance.LateIn : attendance.EarlyOut;
                    var request = GetRequestEntity(item, actualLateInErlyOut);
                    requestsBuilder.Append("- ");
                    requestsBuilder.Append(request.RequestType.Name);
                    requestsBuilder.Append(": ");
                    requestsBuilder.Append(RequestService.ToRequestMessage(request, isNoFormat: true));
                    requestsBuilder.Append("\r\n- Create date: ");
                    requestsBuilder.Append(request.CreatedDate.ToDateMonthYearFormatedDate());
                    requestsBuilder.Append((string.IsNullOrEmpty(request.Remark) ? string.Empty : "\r\n" + "- Remark: " + request.Remark));
                    requestsBuilder.Append("\r\n");
                }

                requests = requestsBuilder.ToString(0, requestsBuilder.Length - 2);
            }

            return requests;
        }

        private static RequestEntity GetRequestEntity(EmployeeAttendanceRequest request, int actualLateInErlyOut)
        {
            var requestEntity = new RequestEntity
            {
                RequestType = request.RequestType,
                Remark = request.Remark,
                RequestedDate = request.RequestDate,
                FromDate = request.FromDate ?? DateTime.Now,
                ToDate = request.ToDate ?? DateTime.Now,
                CreatedDate = request.CreatedDate,
                TotalDays = request.TotalDays ?? 0,
                LeaveType = request.LeaveType,
                FromTime = request.FromTime ?? DateTime.Now,
                ToTime = request.ToTime ?? DateTime.Now,
                FromShift = new Shift
                {
                    Name = request.FromShift
                },
                ToShift = new Shift
                {
                    Name = request.ToShift
                },
                TotalMins = request.TotalMins ?? 0,
                ActualLateInEarlyOut = actualLateInErlyOut
            };

            return requestEntity;
        }
    }
}