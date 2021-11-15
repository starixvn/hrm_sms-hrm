namespace Fanex.NexHR.Models
{
    using HRM.UserControl.Client.Core.Models.Role;

    public class EmployeeRole
    {
        public int EmployeeId { get; set; }

        public Role Role { get; set; }
    }
}