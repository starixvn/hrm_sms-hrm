namespace Fanex.NexHR.Models.ScheduleArrange
{
    using Fanex.NexHR.Core.Enums;
    using Newtonsoft.Json;

    public class ShiftBase : EntityBase<int>
    {
        [JsonIgnore]
        public string CheckIn { get; set; }

        [JsonIgnore]
        public string CheckOut { get; set; }

        [JsonIgnore]
        public string BackgroundColor { get; set; }

        [JsonIgnore]
        public bool IsShift { get; set; }

        public CheckInOutDateType CheckInDateType { get; set; }
        public CheckInOutDateType CheckOutDateType { get; set; }
    }
}