// --------------------------------------------------------------------------
// <copyright file="ShiftRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.Data;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// Shift Repository.
    /// </summary>
    public class ShiftRepository : IShiftRepository
    {
        public IList<Shift> GetAll()
        {
            using (var db = new ObjectDb("Shift_GetShifts"))
            {
                return db.Query<Shift>().ToList();
            }
        }

        public Shift GetByDate(int empId, DateTime date)
        {
            using (var db = new ObjectDb("EmployeeCalendar_GetShiftByDate"))
            {
                var shiftData = db.QueryEntity<Shift>(new { empId, date });
                return shiftData;
            }
        }

        public Shift GetById(int shiftId)
        {
            using (var db = new ObjectDb("Shift_GetById"))
            {
                var shiftData = db.QueryEntity<Shift>(new { shiftId });
                return shiftData;
            }
        }

        /// <summary>
        /// Function to insert a shift to Database.
        /// </summary>
        /// <param name="model">Shift model.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>The result of insert.</returns>
        public int Insert(Shift model, out int errorCode, out string message)
        {
            using (var db = new ObjectDb("Shift_Insert"))
            {
                db.AddInParameter("@Name", DbType.String, model.Name);
                db.AddInParameter("@CheckInTime", DbType.String, model.BeginCheckInTime);
                db.AddInParameter("@CheckOutTime", DbType.String, model.BeginCheckOutTime);
                db.AddInParameter("@IsInAfterDay", DbType.Boolean, model.IsInAfterDay);
                db.AddInParameter("@IsOutAfterDay", DbType.Boolean, model.IsOutAfterDay);
                db.AddInParameter("@CreatedBy", DbType.Int16, model.CreatedBy);
                db.AddInParameter("@CreatedDate", DbType.DateTime, model.CreatedDate);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                db.ExecuteScalar();
                errorCode = (int)db.GetParameterValue("ErrCode");
                message = db.GetParameterValue("ErrMsg") as string;
                return errorCode;
            }
        }

        /// <summary>
        /// Function to update a shift to Database.
        /// </summary>
        /// <param name="model">Shift model.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>The result of update.</returns>
        public bool Update(Shift model, out int errorCode, out string message)
        {
            using (var db = new ObjectDb("Shift_Update"))
            {
                db.AddInParameter("@ShiftId", DbType.Int32, model.ShiftID);
                db.AddInParameter("@Name", DbType.String, model.Name);
                db.AddInParameter("@CheckInTime", DbType.String, model.BeginCheckInTime);
                db.AddInParameter("@CheckOutTime", DbType.String, model.BeginCheckOutTime);
                db.AddInParameter("@IsInAfterDay", DbType.Boolean, model.IsInAfterDay);
                db.AddInParameter("@IsOutAfterDay", DbType.Boolean, model.IsOutAfterDay);
                db.AddInParameter("@IsDisabled", DbType.Boolean, model.IsDisabled);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, model.ModifiedDate);
                db.AddInParameter("@ModifiedBy", DbType.Int32, model.ModifiedBy);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                db.ExecuteNonQuery();
                errorCode = (int)db.GetParameterValue("ErrCode");
                message = db.GetParameterValue("ErrMsg") as string;

                return errorCode == 0;
            }
        }

        /// <summary>
        /// Function to delete a shift.
        /// </summary>
        /// <param name="shiftId">The Shift Id.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>The result of delete.</returns>
        public bool Delete(int shiftId, out int errorCode, out string message)
        {
            using (var db = new ObjectDb("Shift_Delete"))
            {
                db.AddInParameter("@ShiftId", DbType.Int32, shiftId);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                db.ExecuteNonQuery();
                errorCode = (int)db.GetParameterValue("ErrCode");
                message = db.GetParameterValue("ErrMsg") as string;

                return errorCode == 0;
            }
        }
    }
}
