namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.ViewModels;
    using Models;
    using Models.Request;

    public class ExportExcelAttendanceReportService : IExportExcelAttendanceReportService
    {
        private const int RowStartHeader = 2;
        private const int RowStartBody = 3;
        private const int RowHighlight = 5;
        private const int RowOffdate = 6;
        private const string AttendanceTemplatePath = @"~\ExcelTemplates\AttendanceReportTemplate.xls";

        public void Export(IList<EmployeeAttendanceModel> attendances, DateTime fromDate, DateTime toDate)
        {
            string templatePath = HttpContext.Current.Server.MapPath(AttendanceTemplatePath);
            string fileName = "AttendanceReport_" + DateTime.Now.ToString("yyyyMMdd");

            var writer = new ExcelTemplateWriter(templatePath, fileName, "Attendance Report");

            /// Fill report title.
            writer.CopyRows(0, 2);

            /// Fill report header.
            writer.CopyRows(RowStartHeader, 1);

            FillContentRowReport(attendances, ref writer);

            writer.SyncColumnsWidth(15);
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

            if (attendance.LateIn > 30 || attendance.EarlyOut > 30)
            {
                writer.WriteRow(RowHighlight, record);
            }
            else if (attendance.IsWeekend)
            {
                writer.WriteRow(RowOffdate, record);
            }
            else if (rowIndex % 2 != 0)
            {
                writer.WriteRow(RowStartBody, record);
            }
            else
            {
                writer.WriteRow(RowStartBody + 1, record);
            }

            rowIndex++;
        }

        private object CreateRecordReport(EmployeeAttendanceModel attendance, int rowIndex)
        {
            string requests = RequestsRemark(attendance);

            string attendanceDate = attendance.AttendanceDate.HasValue ? attendance.AttendanceDate.Value.ToDayOfWeekFormatedDate() : string.Empty;
            string shiftCode = !string.IsNullOrEmpty(attendance.ShiftCode) ? attendance.ShiftCode : string.Empty;
            string inFixed = attendance.InFixed.HasValue ? attendance.InFixed.Value.ToShortTime() : string.Empty;
            string outFixed = attendance.OutFixed.HasValue ? attendance.OutFixed.Value.ToShortTime() : string.Empty;
            string totalWorkHours = attendance.TotalWorkHours != 0 ? Utilities.TotalHoursToTime((float)attendance.TotalWorkHours) : string.Empty;
            string overtime = attendance.IsOvertime ? attendance.FormattedOvertimeStatus + " (" + attendance.OvertimeHours.ToString() + ")" : string.Empty;

            var record = new
            {
                Count = rowIndex,
                AttendanceDate = attendanceDate,
                Code = attendance.Code,
                NickName = attendance.NickName,
                FullName = attendance.FullName,
                TeamCode = attendance.TeamCode,
                ShiftCode = shiftCode,
                InFixed = inFixed,
                OutFixed = outFixed,
                TotalWorkHours = totalWorkHours,
                LateIn = Utilities.MinuteToTime(attendance.LateIn),
                EarlyOut = Utilities.MinuteToTime(attendance.EarlyOut),
                Remark = requests,
                Overtime = overtime,
                Notes = attendance.Notes
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
                    var actualLateInErlyOut = item.RequestType.Id == (int)RequestType.LateIn ? attendance.LateIn : attendance.EarlyOut;
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
                RequestedDate = request.FromDate.HasValue ? request.FromDate.Value : DateTime.Now,
                FromDate = request.FromDate.HasValue ? request.FromDate.Value : DateTime.Now,
                ToDate = request.ToDate.HasValue ? request.ToDate.Value : DateTime.Now,
                CreatedDate = request.CreatedDate.HasValue ? request.CreatedDate.Value : DateTime.Now,
                TotalDays = request.TotalDays.HasValue ? request.TotalDays.Value : 0,
                LeaveType = request.LeaveType,
                FromTime = request.FromTime.HasValue ? request.FromTime.Value : DateTime.Now,
                ToTime = request.ToTime.HasValue ? request.ToTime.Value : DateTime.Now,
                FromShift = new Shift
                {
                    Name = request.FromShift
                },
                ToShift = new Shift
                {
                    Name = request.ToShift
                },
                TotalMins = request.TotalMins.HasValue ? request.TotalMins.Value : 0,
                ActualLateInEarlyOut = actualLateInErlyOut
            };

            return requestEntity;
        }
    }
}