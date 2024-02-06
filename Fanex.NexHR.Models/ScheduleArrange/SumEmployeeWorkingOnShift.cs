namespace Fanex.NexHR.Models.ScheduleArrange
{
    using Fanex.NexHR.Core.Enums;

    public class SumEmployeeWorkingOnShift
    {
        private const byte TotalDays = 7;

        public string Code { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public CheckInOutDateType CheckInDateType { get; set; }
        public CheckInOutDateType CheckOutDateType { get; set; }
        public int[] Weekdays { get; set; } = new int[TotalDays];
    }
}