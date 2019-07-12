namespace Fanex.NexHR.ViewModels.Report.Overtime
{
    using System;
    using Fanex.NexHR.Core.Enums;

    public class MonthlyFilterViewModel
    {
        private const byte FromDay = 10;
        private const byte ToDay = 9;

        public int Month { get; set; }
        public int Year { get; set; }
        public byte Calendar { get; set; }
        public int? EmployeeId { get; set; }

        public DateTime FromDate
        {
            get
            {
                if (Calendar == (byte)CalendarType.Normal)
                {
                    return new DateTime(Year, Month, 1);
                }
                else
                {
                    var previousMonth = new DateTime(Year, Month, 1).AddMonths(-1);
                    return new DateTime(previousMonth.Year, previousMonth.Month, FromDay);
                }
            }
        }

        public DateTime ToDate
        {
            get
            {
                if (Calendar == (byte)CalendarType.Normal)
                {
                    var dayInMonth = DateTime.DaysInMonth(Year, Month);
                    return new DateTime(Year, Month, dayInMonth);
                }
                else
                {
                    return new DateTime(Year, Month, ToDay);
                }
            }
        }
    }
}