namespace Fanex.NexHR.ViewModels.Department
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class DepartmentViewModel
    {
        public bool? IsActive { get; set; }

        public IList<Department> Departments { get; set; }

        public int ParentId { get; set; }
    }
}
