namespace Fanex.NexHR.Models.Overtime
{
    public class Requester
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Nickname { get; set; }
        public int WorkingHours { get; set; }
        public decimal SpecialOT { get; set; }
        public EntityBase<int> Section { get; set; }
    }
}