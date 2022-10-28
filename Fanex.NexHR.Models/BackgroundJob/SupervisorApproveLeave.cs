namespace Fanex.NexHR.Models.BackgroundJob
{
    public class SupervisorApproveLeave
    {
        public int EmpId { get; set; }

        public int OwnerId { get; set; }

        public string Code { get; set; }

        public string FullName { get; set; }

        public string Nickname { get; set; }

        public string Email { get; set; }

        public int SectionId { get; set; }

        public int DepartmentId { get; set; }

        public string SectionCode { get; set; }

        public string DepartmentCode { get; set; }
    }
}
