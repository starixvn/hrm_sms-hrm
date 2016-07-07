namespace Fanex.NexHR.Models
{
    public class UserChangedShift
    {
        public int EmpId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public int Day { get; set; }

        public string Code { get; set; }

        public int ShiftId { get; set; }

        public string CodeFrom { get; set; }

        public int ShiftIdFrom { get; set; }
    }
}
