namespace Fanex.NexHR.Models
{
    public class TeamLeader
    {
        public int Id { get; set; }

        public Employee Employee { get; set; }

        public bool IsLeader { get; set; }
    }
}