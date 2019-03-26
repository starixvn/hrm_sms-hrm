namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Models;

    public interface IDepartmentService
    {
        Department GetById(GetDepartmentByIdCriteria criteria);

        IList<Department> GetDepartment(GetDepartmentByParentIdCriteria criteria);

        IList<Department> GetParentDepartment(GetParentByDepartmentCriteria criteria);
    }
}