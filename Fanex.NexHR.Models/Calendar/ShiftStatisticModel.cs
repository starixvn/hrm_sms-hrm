namespace Fanex.NexHR.Models
{
    public class ShiftStatisticModel
    {
        public int ShiftId { get; set; }

        public string ShiftCode { get; set; }

        public int WorkMonday { get; set; }

        public int WorkTuesday { get; set; }

        public int WorkWednesday { get; set; }

        public int WorkThursday { get; set; }

        public int WorkFriday { get; set; }

        public int WorkSaturday { get; set; }

        public int WorkSunday { get; set; }
    }
}