namespace Fanex.NexHR.Services
{
    using Core.Services;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;

    public class DepartmentService : BaseService, IDepartmentService
    {
        private readonly IDepartmentRepository departmentRepository;

        public DepartmentService()
            : this(null)
        {
        }

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            this.departmentRepository = departmentRepository ?? new DepartmentRepository();
        }

        public Department GetById(GetDepartmentByIdCriteria criteria)
        {
            Department department = new Department();

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