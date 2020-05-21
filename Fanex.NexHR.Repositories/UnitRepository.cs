namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;

    public class UnitRepository : IUnitRepository
    {
        public Unit GetById(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var item = db.QueryEntity<Unit, UnitHead, Unit>(
                        (unit, manager) =>
                        {
                            return unit;
                        },
                        splitOn: "ManagerId",
                        parameterSet: criteria);

                return item;
            }
        }

        public IList<Unit> GetUnit(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var items = db.Query<Unit, UnitHead, Unit>(
                        (unit, manager) =>
                        {
                            return unit;
                        },
                        splitOn: "ManagerId",
                        parameterSet: criteria);

                return items.ToList();
            }
        }
    }
}