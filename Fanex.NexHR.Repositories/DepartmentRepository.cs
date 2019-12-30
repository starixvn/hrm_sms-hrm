namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;

    public class DepartmentRepository : IDepartmentRepository
    {
        public Department GetById(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var department = db.QueryEntity<Department, Department, DepartmentHead, Department>(
                        (depart, parent, manager) =>
                        {
                            depart.Manager = manager;
                            depart.Parent = parent;
                            return depart;
                        },
                        splitOn: "ParentId, ManagerId",
                        parameterSet: criteria);

                return department;
            }
        }

        public IList<Department> GetDepartment(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var departments = db.Query<Department, Department, DepartmentHead, Department>(
                        (depart, parent, manager) =>
                        {
                            depart.Manager = manager;
                            depart.Parent = parent;
                            return depart;
                        },
                        splitOn: "ParentId, ManagerId",
                        parameterSet: criteria);

                return departments.ToList();
            }
        }
    }
}