// --------------------------------------------------------------------------
// <copyright file="ChangeShiftRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2013.11.14">Created</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.Data;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Change shift Repository.
    /// </summary>
    public class ChangeShiftRepository : IChangeShiftRepository
    {
        /// <summary>
        /// Inserts the change shift request.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <returns>Request's id.</returns>
        public long Insert(object model, out int errorCode, out string message)
        {
            var request = (ChangeShiftViewModel)model;
            var fromDate = Convert.ToDateTime(request.FromDate);
            var toDate = Convert.ToDateTime(request.ToDate);
            using (var db = new ObjectDb("WorkingTimeRequest_Insert_ChangeShift"))
            {
                db.AddInParameter("@EmpId", DbType.Int32, request.EmployeeId);
                db.AddInParameter("@RequestType", DbType.Int16, request.RequestType);
                db.AddInParameter("@Remark", DbType.String, request.Remark);
                db.AddInParameter("@FromDate", DbType.DateTime, fromDate);
                db.AddInParameter("@ToDate", DbType.DateTime, toDate);
                db.AddInParameter("@CreatedDate", DbType.DateTime, DateTime.Now);
                db.AddInParameter("@CreatedBy", DbType.Int32, request.By);
                db.AddInParameter("@VerifierId", DbType.Int32, request.VerifierId);
                db.AddInParameter("@ApproverId", DbType.Int32, request.ApproverId);
                db.AddInParameter("@HRId", DbType.Int32, null);
                db.AddInParameter("@ListDetail", DbType.Xml, request.Value);

                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                var result = db.ExecuteScalar();
                var requestId = result == null ? -1 : Convert.ToInt32(result);
                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                return requestId;
            }
        }

        /// <summary>
        /// Updates the change shift request.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <returns>Result update.</returns>
        public bool Update(object model, out int errorCode, out string message)
        {
            var request = (ChangeShiftViewModel)model;
            var fromDate = Convert.ToDateTime(request.FromDate);
            var toDate = Convert.ToDateTime(request.ToDate);

            using (var db = new ObjectDb("WorkingTimeRequest_Update_ChangeShiftRequest"))
            {
                db.AddInParameter("@RequestId", DbType.Int32, request.Id);
                db.AddInParameter("@EmpId", DbType.Int32, request.EmployeeId);
                db.AddInParameter("@RequestType", DbType.Int16, request.RequestType);
                db.AddInParameter("@Remark", DbType.String, request.Remark);
                db.AddInParameter("@FromDate", DbType.DateTime, fromDate);
                db.AddInParameter("@ToDate", DbType.DateTime, toDate);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, DateTime.Now);
                db.AddInParameter("@ModifiedBy", DbType.Int32, request.By);
                db.AddInParameter("@VerifierId", DbType.Int32, request.VerifierId);
                db.AddInParameter("@ApproverId", DbType.Int32, request.ApproverId);
                db.AddInParameter("@HRId", DbType.Int32, null);
                db.AddInParameter("@ListDetail", DbType.Xml, request.Value);

                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                var result = db.ExecuteNonQuery() != 0;
                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                return result;
            }
        }

        public List<ShiftDetails> GetChangeShiftDetailsById(long id)
        {
            using (ObjectDb db = new ObjectDb("WorkingTimeRequest_GetChangeShiftTypeDetailByRequestID"))
            {
                return db.Query<ShiftDetails>(new { RequestID = id }).ToList();
            }
        }
    }
}
