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
                var item = db.QueryEntity<Unit>(criteria);

                return item;
            }
        }

        public IList<Unit> GetUnit(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                IList<Unit> items = db.Query<Unit>(criteria).ToList();

                return items.ToList();
            }
        }
    }
}