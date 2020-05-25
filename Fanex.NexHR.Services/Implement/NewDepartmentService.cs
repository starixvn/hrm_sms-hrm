namespace Fanex.NexHR.Services.Implement
{
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.NewDepartment;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class NewDepartmentService : INewDepartmentService
    {
        private readonly INewDepartmentRepository departmentRepository;
        private readonly IBaseService baseService;

        public NewDepartmentService(INewDepartmentRepository departmentRepository,
            IBaseService baseService)
        {
            this.departmentRepository = departmentRepository;
            this.baseService = baseService;
        }

        public CommandResult Delete(int id)
        {
            var criteria = new DeleteDepartmentCriteria
            {
                departmentId = id,
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public IList<NewDepartment> GetByActive(bool? isActive)
        {
            var criteria = new GetDepartmentByActiveCriteria
            {
                IsActive = isActive
            };
            IList<NewDepartment> departments = departmentRepository.GetByActive(criteria);

            return departments ?? new List<NewDepartment>();
        }

        public NewDepartment GetById(int departmentId)
        {
            var criteria = new GetDepartmentByIdCriteria
            {
                Id = departmentId
            };
            return departmentRepository.GetById(criteria);
        }
    }
}
