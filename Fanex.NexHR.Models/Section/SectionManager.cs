namespace Fanex.NexHR.Models.Section
{
    public class SectionManager
    {
        public int GroupId { get; set; }

        public int EmpId { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }

        public string DisplayName => string.IsNullOrWhiteSpace(Nickname) ? FullName : Nickname + " (" + FullName + ")";

        public byte RoleId { get; set; }
    }
}