namespace Fanex.NexHR.Models.BackgroundJob
{
    public class EmployeeDepartmentSections
    {
        public int SectionId { get; set; }

        public int DepartmentId { get; set; }

        public string SectionCode { get; set; }

        public string DepartmentCode { get; set; }

        public int TotalEmployee { get; set; }

        public int AnnualLeaveBalance { get; set; }

        public double PercentLeave { get; set; }
    }
}
