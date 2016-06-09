namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.Data;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

    public class HolidayRepository : IHolidayRepository
    {
        public int Insert(Holiday model)
        {
            using (var db = new ObjectDb("Holiday_Insert"))
            {
                db.AddInParameter("@HolidayName", DbType.String, model.HolidayName);
                db.AddInParameter("@FromDate", DbType.DateTime, model.FromDate);
                db.AddInParameter("@ToDate", DbType.DateTime, model.ToDate);
                db.AddInParameter("@ModifiedBy", DbType.Int16, model.ModifiedBy);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, model.ModifiedDate);

                return (int)db.ExecuteScalar();
            }
        }

        public bool Delete(int id, int modifiedBy)
        {
            using (var db = new ObjectDb("Holiday_Delete"))
            {
                db.AddInParameter("@Id", DbType.Int32, id);
                db.AddInParameter("@ModifiedBy", DbType.Int16, modifiedBy);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, DateTime.Now);
                return db.ExecuteNonQuery() != 0;
            }
        }
        
        public bool Update(Holiday model)
        {
            using (ObjectDb db = new ObjectDb("Holiday_Update"))
            {
                db.AddInParameter("@Id", DbType.Int32, model.Id);
                db.AddInParameter("@HolidayName", DbType.String, model.HolidayName);
                db.AddInParameter("@FromDate", DbType.DateTime, model.FromDate);
                db.AddInParameter("@ToDate", DbType.DateTime, model.ToDate);
                db.AddInParameter("@ModifiedBy", DbType.Int16, model.ModifiedBy);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, model.ModifiedDate);

                return db.ExecuteNonQuery() != 0;
            }
        }

        public IList<Holiday> GetByYear(int year)
        {
            using (var db = new ObjectDb("Holiday_GetByYear"))
            {
                return db.Query<Holiday>(new { year }).ToList();
            }
        }

        public Holiday GetById(int id)
        {
            using (var db = new ObjectDb("Holiday_GetById"))
            {
                return db.QueryEntity<Holiday>(new { id });
            }
        }
    }
}
