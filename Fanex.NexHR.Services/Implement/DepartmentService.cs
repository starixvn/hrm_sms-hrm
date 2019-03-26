namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;

    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository departmentRepository;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            this.departmentRepository = departmentRepository;
        }

        public Department GetById(GetDepartmentByIdCriteria criteria)
        {
            var department = new Department();

            if (criteria != null && criteria.IsValid())
            {
                department = departmentRepository.GetById(criteria);
            }

            return department;
        }

        public IList<Department> GetDepartment(GetDepartmentByParentIdCriteria criteria)
        {
            IList<Department> departments = null;

            if (criteria != null && criteria.IsValid())
            {
                departments = departmentRepository.GetDepartment(criteria);
            }

            return departments ?? new List<Department>();
        }

        public IList<Department> GetParentDepartment(GetParentByDepartmentCriteria criteria)
        {
            IList<Department> departments = null;

            if (criteria != null && criteria.IsValid())
            {
                departments = departmentRepository.GetDepartment(criteria);
            }

            return departments ?? new List<Department>();
        }
    }
}