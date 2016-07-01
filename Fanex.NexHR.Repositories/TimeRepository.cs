namespace Fanex.NexHR.Repositories
{
    using Fanex.Data;
    using Fanex.NexHR.Repositories.Interfaces;

    public class TimeRepository : ITimeRepository
    {
        public bool ImportTimeRecord()
        {
            using (var db = new ObjectDb("TimeRecord_ImportData"))
            {
                return db.ExecuteNonQuery() != 0;
            }
        }
    }
}
