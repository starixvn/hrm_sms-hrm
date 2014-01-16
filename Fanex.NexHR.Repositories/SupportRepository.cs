// -----------------------------------------------------------------------
// <copyright file="SupportRepository.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Repositories
{
    using System.Data;
    using Fanex.Data;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.ViewModels.TimesheetRecord;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SupportRepository : ISupportRepository
    {
        /// <summary>
        /// Method Insert Manual TimeSheet Record.
        /// </summary>
        /// <param name="model">TimeSheetRecord ViewModel.</param>
        /// <returns>Boolean to check.</returns>
        public bool RecordTimeSheet(TimeSheetRecordViewModel model)
        {
            using (var db = new ObjectDb("Support_TimeSheetInput"))
            {
                db.AddInParameter("@EmpId", DbType.Int32, model.EmpId);
                db.AddInParameter("@RecordTime", DbType.DateTime, model.RecordTime);
                db.AddInParameter("@InOutType", DbType.Boolean, model.InOutType);
                db.AddInParameter("@InsertDate", DbType.DateTime, model.InsertDate);

                var result = db.ExecuteNonQuery() != 0;
                return result;
            }
        }
    }
}
