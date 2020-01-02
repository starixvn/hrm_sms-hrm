namespace Fanex.NexHR.Models
{
    public class Department : EntityBase<int>
    {
        public DepartmentHead Manager { get; set; }

        public Department Parent { get; set; }

        public bool IsActive { get; set; }
    }
}