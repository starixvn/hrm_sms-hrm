// --------------------------------------------------------------------------
// <copyright file="SupportRepository.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.6">Created</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System.Data;
    using Fanex.Data;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.ViewModels.TimesheetRecord;

    /// <summary>
    /// Support Repository.
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
                db.AddInParameter("@InOutType", DbType.Boolean, model.InOutType != 1);
                db.AddInParameter("@InsertDate", DbType.DateTime, model.InsertDate);

                var result = db.ExecuteNonQuery() != 0;
                return result;
            }
        }
    }
}
