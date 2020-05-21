namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

    public class NewDepartmentRepository : INewDepartmentRepository

    {
        public IList<NewDepartment> GetByActive(ICriteria criteria)
        {
            return null;
        }
    }
}
