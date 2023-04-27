namespace GO.Website.Extensions
{
    using System;

    public static class DateTimeExtensions
    {
        public static string ToStartOfDayString(this DateTime datetime, string format = "MMM dd, yyyy HH:mm")
        {
            var startOfDay = new DateTime(datetime.Year, datetime.Month, datetime.Day, 0, 0, 0);
            return startOfDay.ToString(format);
        }

        public static string ToEndOfDayString(this DateTime datetime, string format = "MMM dd, yyyy HH:mm")
        {
            var endOfDay = new DateTime(datetime.Year, datetime.Month, datetime.Day, 23, 59, 59);
            return endOfDay.ToString(format);
        }
    }
}