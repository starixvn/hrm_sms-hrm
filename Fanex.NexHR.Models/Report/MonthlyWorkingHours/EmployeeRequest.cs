namespace Fanex.NexHR.Models.Report.MonthlyWorkingHours
{
    using System;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Resources;

    public class EmployeeRequest
    {
        public long RequestId { get; set; }
        public short RequestTypeId { get; set; }
        public byte RequestStatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime RequestedDate { get; set; }
        public string Remark { get; set; }

        public int EmployeeId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Section { get; set; }

        // leave
        public ushort LeaveTypeId { get; set; }
        public string LeaveType { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsHalfDay { get; set; }
        public decimal TotalDays { get; set; }
        public string LeaveCountDetails { get; set; }

        // Change shift
        public string FromShift { get; set; }
        public string ToShift { get; set; }

        // Late In - Early Out
        public int TotalMins { get; set; }
        public int ActualLateInEarlyOut { get; set; }

        // Keep In View
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public byte KeepInViewTypeId { get; set; }

        public string RequestDetails
        {
            get
            {
                var message = string.Empty;
                switch (RequestTypeId)
                {
                    case (int)RequestType.Leave:
                        message = GetMessageForLeave();
                        break;

                    case (int)RequestType.ChangeShift:
                        message = GetMessageForChangeShift();
                        break;

                    case (int)RequestType.CheckIn:
                    case (int)RequestType.CheckOut:
                        message = GetMessageForCheckInOut();
                        break;

                    case (int)RequestType.LateIn:
                        message = GetMessageForLateIn();
                        break;

                    case (int)RequestType.EarlyOut:
                        message = GetMessageForEarlyOut();
                        break;

                    case (int)RequestType.KeepInView:
                        message = GetMessageForKeepInView();
                        break;

                    default:
                        message = string.Empty;
                        break;
                }

                return message;
            }
        }

        public static string GetMessageDateString(DateTime fromDate, DateTime toDate)
        {
            var result = string.Empty;
            if ((toDate - fromDate).Days <= 0)
            {
                result = fromDate.ToDateMonthYearFormatedDate();
            }
            else if (fromDate.Year != toDate.Year)
            {
                result = fromDate.ToDateMonthYearFormatedDate() + " - " + toDate.ToDateMonthYearFormatedDate();
            }
            else
            {
                result = fromDate.Month != toDate.Month
                    ? fromDate.ToMonthAndDayFormatedDate() + " - " + toDate.ToDateMonthYearFormatedDate() 
                    : fromDate.ToString("dd") + " - " + toDate.ToDateMonthYearFormatedDate();
            }

            return result;
        }

        private string GetMessageForLeave()
        {
            return string.Format(
                RequestFormatConstants.MsgLeaveWithNoFormat,
                GetDateTimeForLeaveRequest(),
                GetLeaveDayDetail());
        }

        private string GetDateTimeForLeaveRequest()
        {
            var leaveDate = GetMessageDateString(FromDate, ToDate);

            if ((TotalDays - Constants.HalfDayOffValue) == 0)
            {
                leaveDate = string.Format(
                    RequestFormatConstants.MsgLeaveHalfDayWithNoFormat,
                    leaveDate,
                    FromTime.ToShortTime(),
                    ToTime.ToShortTime());
            }

            return leaveDate;
        }

        private string GetMessageForChangeShift()
        {
            var message = string.Empty;
            message = FromDate.Date == ToDate.Date
                ? string.Format(
                    RequestFormatConstants.MsgChangeShiftOneDayWithNoFormat,
                    FromShift,
                    ToShift,
                    FromDate.ToDateMonthYearFormatedDate())
                : string.Format(
                    RequestFormatConstants.MsgChangeShiftWithNoFormat,
                    GetMessageDateString(FromDate, ToDate));

            return message;
        }

        private string GetMessageForCheckInOut()
        {
            var time = RequestedDate.Day == FromDate.Day ? RequestedDate.ToShortTime() : (RequestedDate.ToShortTime() + "+");
            return string.Format(
                RequestFormatConstants.MsgCheckInOutWithNoFormat,
                time,
                FromShift,
                FromDate.ToDateMonthYearFormatedDate());
        }

        private string GetMessageForLateIn()
        {
            var actualLateIn = ActualLateInEarlyOut == 0 ? Labels.NA : ("+" + ActualLateInEarlyOut + "'");
            return string.Format(RequestFormatConstants.MsgLateInWithNoFormat, 
                TotalMins,
                FromShift,
                GetMessageDateString(FromDate, ToDate),
                actualLateIn);
        }

        private string GetMessageForEarlyOut()
        {
            var actualEarlyOut = ActualLateInEarlyOut == 0 ? Labels.NA : ("-" + ActualLateInEarlyOut + "'");
            return string.Format(
                RequestFormatConstants.MsgEarlyOutWithNoFormat,
                TotalMins,
                FromShift,
                GetMessageDateString(FromDate, ToDate),
                actualEarlyOut);
        }

        private string GetMessageForKeepInView()
        {
            return KeepInViewTypeId == (byte)KeepInViewTypes.KeepInViewByDay
                ? GetMessageForKeepInViewByDay()
                : GetMessageForKeepInViewByHour();
        }

        private string GetMessageForKeepInViewByDay()
        {
            return TotalDays.Equals(Constants.HalfDayOffValue)
                ? string.Format(
                    RequestFormatConstants.MsgKeepInViewHalfdayWithNoFormat,
                    FromDate.ToDateMonthYearFormatedDate(),
                    FromTime.ToShortTime(),
                    ToTime.ToShortTime())
                : string.Format(RequestFormatConstants.MsgKeepInViewOneDayWithNoFormat, FromDate.ToDateMonthYearFormatedDate());
        }

        private string GetMessageForKeepInViewByHour()
        {
            var minutes = (ToTime - FromTime).TotalMinutes;
            var hours = minutes / 60.0;
            return string.Format(
                    RequestFormatConstants.MsgKeepInViewByHoursWithNoFormat,
                    FromDate.ToDateMonthYearFormatedDate(),
                    FromTime.ToShortTime(),
                    ToTime.ToShortTime(),
                    hours.ToString("0.#"));
        }

        private string GetLeaveDayDetail()
        {
            return LeaveTypeId == (byte)LeaveTypes.GeneralLeave ? LeaveCountDetails : GetTotalDay(TotalDays);
        }

        private static string GetTotalDay(decimal totalDays)
        {
            return totalDays != 0 ? string.Format(RequestFormatConstants.MsgTotalLeaveDays, totalDays.ToString("0.#")) : string.Empty;
        }
    }
}