namespace Fanex.NexHR.Models.ScheduleArrange
{
    public class WeeklyDefaultShift
    {
        public int EmployeeId { get; set; }

        public int MonShiftId { get; set; }
        public string MonShiftCode { get; set; }

        public int TueShiftId { get; set; }
        public string TueShiftCode { get; set; }

        public int WedShiftId { get; set; }
        public string WedShiftCode { get; set; }

        public int ThuShiftId { get; set; }
        public string ThuShiftCode { get; set; }

        public int FriShiftId { get; set; }
        public string FriShiftCode { get; set; }

        public int SatShiftId { get; set; }
        public string SatShiftCode { get; set; }

        public int SunShiftId { get; set; }
        public string SunShiftCode { get; set; }
    }
}