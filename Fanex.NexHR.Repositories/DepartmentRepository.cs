namespace Fanex.NexHR.Repositories
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class DepartmentRepository : IDepartmentRepository
    {
        public Department GetById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var department = db.QueryEntity<Department, Department, Employee, Department>(
                        (depart, parent, manager) =>
                        {
                            depart.Manager = manager;
                            depart.Parent = parent;
                            return depart;
                        },
                        splitOn: "ParentId, ManagerId",
                        parameters: criteria.GetSpParams());

                return department;
            }
        }

        public IList<Department> GetDepartment(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var departments = db.Query<Department, Department, Employee, Department>(
                        (depart, parent, manager) =>
                        {
                            depart.Manager = manager;
                            depart.Parent = parent;
                            return depart;
                        },
                        splitOn: "ParentId, ManagerId",
                        parameterValues: criteria.GetSpParams()).ToList();

                return departments;
            }
        }
    }
}
