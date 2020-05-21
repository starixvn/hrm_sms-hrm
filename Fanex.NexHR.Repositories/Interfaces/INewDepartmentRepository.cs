namespace Fanex.NexHR.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Criteria.NewDepartment;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public interface INewDepartmentRepository
    {
        IList<NewDepartment> GetByActive(ICriteria criteria);
        NewDepartment GetById(GetDepartmentByIdCriteria criteria);
    }
}