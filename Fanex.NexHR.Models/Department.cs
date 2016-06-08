namespace Fanex.NexHR.Models
{
    using System;

    public class Department : EntityBase<int>
    {
        public Employee Manager { get; set; }

        public Department Parent { get; set; }

        public bool IsActive { get; set; }
    }
}
