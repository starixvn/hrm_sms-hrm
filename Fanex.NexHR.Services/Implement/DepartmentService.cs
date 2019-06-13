namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;

    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository departmentRepository;
        private readonly IBaseService baseService;

        public DepartmentService(
            IDepartmentRepository departmentRepository,
            IBaseService baseService)
        {
            this.departmentRepository = departmentRepository;
            this.baseService = baseService;
        }

        public Department GetById(int id)
        {
            var criteria = new GetDepartmentByIdCriteria
            {
                DepartmentId = id
            };
            return departmentRepository.GetById(criteria);
        }

        public IList<Department> GetDepartment(int? parentId = null, bool? isActive = true)
        {
            var criteria = new GetDepartmentByParentIdCriteria
            {
                ParentId = parentId,
                IsActive = isActive
            };
            IList<Department> departments = departmentRepository.GetDepartment(criteria);

            return departments ?? new List<Department>();
        }

        public IList<Department> GetParentDepartment(int departmentId)
        {
            var criteria = new GetParentByDepartmentCriteria
            {
                DepartmentId = departmentId
            };
            IList<Department> departments = departmentRepository.GetDepartment(criteria);

            return departments ?? new List<Department>();
        }

        public CommandResult Delete(int actorId, int id)
        {
            var criteria = new DeleteDepartmentCriteria
            {
                DepartmentId = id,
                ModifiedBy = actorId,
            };
            return baseService.ExecuteNonQuery(criteria);
        }
    }
}