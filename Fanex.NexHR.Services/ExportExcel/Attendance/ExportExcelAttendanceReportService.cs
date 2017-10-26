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

    public class ExportExcelAttendanceReportService : IExportExcelAttendanceReportService
    {
        private const int RowStartHeader = 2;
        private const int RowStartBody = 3;
        private const int RowHighlight = 5;
        private const int RowOffdate = 6;
        private const string MsgLeave = "{1} ({0} days)";
        private const string MsgLeaveHalfDay = "{0} ({1}-{2}) 0.5 day";
        private const string MsgLeaveOneDay = "{0} (1 day)";
        private const string MsgLeaveDuringWorking = "{0} ({1}-{2}) {3} minutes";
        private const string MsgChangeShift = "{0} --> {1} ({2})";
        private const string MsgCheckInOut = "{0} {1} ({2})";
        private const string MsgLateIn = "+{0}' {1} ({2})";
        private const string MsgEarlyOut = "-{0}' {1} ({2})";
        private const string MsgKeepInView = "{0} ({1})";
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
            var requests = string.Empty;
            StringBuilder requestsBuilder = new StringBuilder();

            if (attendance.RequestList != null && attendance.RequestList.Any())
            {
                foreach (EmployeeAttendanceRequest request in attendance.RequestList)
                {
                    requestsBuilder.Append("- ");
                    requestsBuilder.Append(request.RequestType.Name);
                    requestsBuilder.Append(": ");
                    requestsBuilder.Append(GetRequestDetail(request));
                    requestsBuilder.Append("\r\n- Create date: ");
                    requestsBuilder.Append(request.CreatedDate.ToDateMonthYearFormatedDate());
                    requestsBuilder.Append((string.IsNullOrEmpty(request.Remark) ? string.Empty : "\r\n" + "- Remark: " + request.Remark));
                    requestsBuilder.Append("\r\n");
                }

                requests = requestsBuilder.ToString(0, requestsBuilder.Length - 2);
            }

            var record = new
            {
                Count = rowIndex,
                AttendanceDate = attendance.AttendanceDate.HasValue ? attendance.AttendanceDate.Value.ToDayOfWeekFormatedDate() : string.Empty,
                Code = attendance.Code,
                NickName = attendance.NickName,
                FullName = attendance.FullName,
                TeamCode = attendance.TeamCode,
                ShiftCode = !string.IsNullOrEmpty(attendance.ShiftCode) ? attendance.ShiftCode : string.Empty,
                InFixed = attendance.InFixed.HasValue ? attendance.InFixed.Value.ToShortTime() : string.Empty,
                OutFixed = attendance.OutFixed.HasValue ? attendance.OutFixed.Value.ToShortTime() : string.Empty,
                TotalWorkHours = attendance.TotalWorkHours != 0 ? Utilities.TotalHoursToTime((float)attendance.TotalWorkHours) : string.Empty,
                LateIn = Utilities.MinuteToTime(attendance.LateIn),
                EarlyOut = Utilities.MinuteToTime(attendance.EarlyOut),
                Remark = requests,
                Overtime = attendance.IsOvertime ? attendance.FormattedOvertimeStatus + " (" + attendance.OvertimeHours.ToString() + ")" : string.Empty,
                Notes = attendance.Notes
            };

            return record;
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
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
                        detail = string.Format(MsgLeave, Convert.ToInt32(request.TotalDays), date);
                    }
                    else if (request.TotalDays == 1)
                    {
                        detail = string.Format(MsgLeaveOneDay, date);
                    }
                    else if ((request.TotalDays - (decimal)0.5) == 0)
                    {
                        detail = string.Format(MsgLeaveHalfDay, date, request.FromDate.Value.ToString("HH:mm"), request.ToDate.Value.ToString("HH:mm"));
                    }
                    else
                    {
                        detail = string.Format(
                            MsgLeaveDuringWorking,
                            date,
                            request.FromDate.Value.ToString("HH:mm"),
                            request.ToDate.Value.ToString("HH:mm"),
                            (request.ToDate.Value - request.FromDate.Value).TotalMinutes);
                    }

                    break;

                case (int)RequestType.ChangeShift:
                    detail = string.Format(MsgChangeShift, request.FromShift, request.ToShift, request.FromDate.ToDateMonthYearFormatedDate());
                    break;

                case (int)RequestType.CheckIn:
                case (int)RequestType.CheckOut:
                    detail = string.Format(MsgCheckInOut, request.FromDate.Value.ToString("HH:mm"), request.FromShift, request.FromDate.ToDateMonthYearFormatedDate());
                    break;

                case (int)RequestType.LateIn:
                    detail = string.Format(MsgLateIn, request.TotalMins, request.FromShift, date);
                    break;

                case (int)RequestType.EarlyOut:
                    detail = string.Format(MsgEarlyOut, request.TotalMins, request.FromShift, date);
                    break;

                case (int)RequestType.KeepInView:
                    detail = string.Format(MsgKeepInView, request.FromShift, request.FromDate.ToDateMonthYearFormatedDate());
                    break;
            }

            return detail;
        }
    }
}