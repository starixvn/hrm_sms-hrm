namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
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
                var department = db.QueryEntity<NewDepartment, Unit, NewDepartment>(
                        (depart, unit) =>
                        {
                            depart.Unit = unit;
                            return depart;
                        },
                        splitOn: "Unit",
                        parameterSet: criteria);

                return null;
            }
        }

        public NewDepartment GetById(GetDepartmentByIdCriteria criteria)
        {
            throw new System.NotImplementedException();
        }
    }
}
