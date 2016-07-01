namespace Fanex.NexHR.Repositories
{
    using System.Data;
    using Fanex.Data;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.ViewModels.TimesheetRecord;

    public class SupportRepository : ISupportRepository
    {
        public bool RecordTimeSheet(TimeSheetRecordViewModel model)
        {
            using (var db = new ObjectDb("Support_TimeSheetInput"))
            {
                db.AddInParameter("@EmpId", DbType.Int32, model.EmpId);
                db.AddInParameter("@RecordTime", DbType.DateTime, model.RecordTime);
                db.AddInParameter("@InOutType", DbType.Boolean, model.InOutType != 1);
                db.AddInParameter("@InsertDate", DbType.DateTime, model.InsertDate);

                var result = db.ExecuteNonQuery() != 0;
                return result;
            }
        }
    }
}
