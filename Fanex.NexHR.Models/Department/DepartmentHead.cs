namespace Fanex.NexHR.Models
{
    public class DepartmentHead
    {
        public int EmpId { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }

        public string DisplayName => (string.IsNullOrWhiteSpace(Nickname) || string.IsNullOrWhiteSpace(FullName))
            ? string.Empty : Nickname + " (" + FullName + ")";
    }
}