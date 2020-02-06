namespace Fanex.NexHR.Models
{
    public class TeamLeader
    {
        public int GroupId { get; set; }

        public int EmpId { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }

        public bool IsLeader { get; set; }
    }
}