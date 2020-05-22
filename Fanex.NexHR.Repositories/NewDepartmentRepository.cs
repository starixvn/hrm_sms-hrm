namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Criteria.NewDepartment;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

    public class NewDepartmentRepository : INewDepartmentRepository

    {
        public IList<NewDepartment> GetByActive(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var departments = db.Query<NewDepartment, Unit, NewDepartment>(
                        (d, u) =>
                        {
                            d.Unit = u;

                            return d;
                        },
                        splitOn: "Name",
                        parameterSet: criteria);

                return departments.ToList();
            }
        }

        public NewDepartment GetById(GetDepartmentByIdCriteria criteria)
        {
            throw new System.NotImplementedException();
        }
    }
}
