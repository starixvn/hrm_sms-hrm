namespace Fanex.NexHR.Models.Section
{
    public class SectionSupervisor
    {
        public int OwnerId { get; set; }

        public string Nickname { get; set; }

        public string FullName { get; set; }

        public bool CheckInOuts { get; set; }

        public bool LateInEarlyOuts { get; set; }

        public bool ChangeShifts { get; set; }

        public bool Leaves { get; set; }

        public bool KeepInViews { get; set; }

        public bool Overtimes { get; set; }

        public bool ArrangeShifts { get; set; }
    }
}