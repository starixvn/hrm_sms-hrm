namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;

    public interface IDepartmentRepository
    {
        Department GetById(ICriteria criteria);

        IList<Department> GetDepartment(ICriteria criteria);
    }
}
