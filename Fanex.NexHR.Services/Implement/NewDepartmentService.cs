namespace Fanex.NexHR.Services.Implement
{
    using Fanex.NexHR.Criteria.NewDepartment;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;

    public class NewDepartmentService : INewDepartmentService
    {
        private readonly INewDepartmentRepository departmentRepository;

        public NewDepartmentService(INewDepartmentRepository departmentRepository)
        {
            this.departmentRepository = departmentRepository;
        }

        public IList<NewDepartment> GetAll(bool? isActive)
        {
            var criteria = new GetDepartmentByActiveCriteria
            {
                IsActive = isActive
            };
            IList<NewDepartment> departments = departmentRepository.GetByActive(criteria);

            return departments ?? new List<NewDepartment>();
        }
    }
}
