namespace Fanex.NexHR.Models
{
    using Fanex.NexHR.Core.Enums;

    public class Shift : EntityBase<int>
    {
        public CheckInOutDateType CheckInDateType { get; set; }
        public string BeginCheckInTime { get; set; }

        public string EndCheckInTime { get; set; }
        public CheckInOutDateType CheckOutDateType { get; set; }

        public string BeginCheckOutTime { get; set; }

        public string EndCheckOutTime { get; set; }

        public short BreakTimeMins { get; set; }

        public string BeginBreakTime { get; set; }

        public string EndBreakTime { get; set; }

        public bool CrossDay { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsUsed { get; set; }
    }
}