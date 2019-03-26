namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;

    public interface IEmployeeRepository
    {
        Employee GetEmployeeById(ICriteria criteria);

        Employee GetEmployeeByCode(ICriteria criteria);

        IList<Employee> GetEmployees(ICriteria criteria);

        IList<Employee> GetEmployeesByRole(ICriteria criteria);
    }
}