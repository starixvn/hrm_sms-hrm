// --------------------------------------------------------------------------
// <copyright file="LeaveRequestRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.11">Created</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// LeaveRequest Repository.
    /// </summary>
    public class LeaveRequestRepository : BaseRequestRepository, ILeaveRequestRepository
    {
        /// <summary>
        /// Function to insert leave request.
        /// </summary>
        /// <param name="model">LeaveRequest Model.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Result of insert.</returns>
        public override long Insert(object model, out int errorCode, out string message)
        {
            var leaveRequest = (LeaveRequestModel)model;

            using (var db = new ObjectDb("WorkingTimeRequest_Insert_LeaveMultiRequest"))
            {
                db.AddInParameter("@EmpId", DbType.Int32, leaveRequest.EmployeeId);
                db.AddInParameter("@RequestType", DbType.Int16, (int)RequestTypes.Leave);
                db.AddInParameter("@LeaveTypeID", DbType.Int32, leaveRequest.LeaveTypeId);
                db.AddInParameter("@Remark", DbType.String, leaveRequest.Remark);
                db.AddInParameter("@FromDate", DbType.DateTime, leaveRequest.FromDate);
                db.AddInParameter("@ToDate", DbType.DateTime, leaveRequest.ToDate);
                db.AddInParameter("@TotalDays", DbType.Decimal, leaveRequest.TotalDays);
                db.AddInParameter("@CreatedDate", DbType.DateTime, DateTime.Now);
                db.AddInParameter("@CreatedBy", DbType.Int32, leaveRequest.CreatedBy);
                db.AddInParameter("@VerifierId", DbType.Int32, leaveRequest.VerifierId);
                db.AddInParameter("@ApproverId", DbType.Int32, leaveRequest.ApproverId);
                db.AddInParameter("@RefNo", DbType.String, leaveRequest.RefNo);
                db.AddInParameter("@ListDetail", DbType.String, leaveRequest.RequestDetailXML);

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
        /// Function to update leave request.
        /// </summary>
        /// <param name="model">LeaveRequest Model.</param>
        /// <param name="errorCode">Error Code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Result of update.</returns>
        public override bool Update(object model, out int errorCode, out string message)
        {
            var leaveRequest = (LeaveRequestModel)model;

            using (var db = new ObjectDb("WorkingTimeRequest_Update_LeaveMultiRequest"))
            {
                db.AddInParameter("@RequestId", DbType.Int64, leaveRequest.RequestId);
                db.AddInParameter("@EmpId", DbType.Int32, leaveRequest.EmployeeId);
                db.AddInParameter("@RequestType", DbType.Int16, (int)RequestTypes.Leave);
                db.AddInParameter("@LeaveTypeID", DbType.Int32, leaveRequest.LeaveTypeId);
                db.AddInParameter("@Remark", DbType.String, leaveRequest.Remark);
                db.AddInParameter("@FromDate", DbType.DateTime, leaveRequest.FromDate);
                db.AddInParameter("@ToDate", DbType.DateTime, leaveRequest.ToDate);
                db.AddInParameter("@TotalDays", DbType.Decimal, leaveRequest.TotalDays);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, DateTime.Now);
                db.AddInParameter("@ModifiedBy", DbType.Int32, leaveRequest.ModifiedBy);
                db.AddInParameter("@VerifierId", DbType.Int32, leaveRequest.VerifierId);
                db.AddInParameter("@ApproverId", DbType.Int32, leaveRequest.ApproverId);
                db.AddInParameter("@RefNo", DbType.String, leaveRequest.RefNo);
                db.AddInParameter("@ListDetail", DbType.String, leaveRequest.RequestDetailXML);

                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                var result = db.ExecuteNonQuery() != 0;

                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                return result;
            }
        }

        public List<LeaveDetailModel> GetLeaveRequestDetailsById(long requestId)
        {
            using (ObjectDb db = new ObjectDb("WorkingTimeRequest_GetLeaveRequestDetailByRequestID"))
            {
                var leaveRequestDetails = db.Query<LeaveDetailModel>(new { RequestID = requestId }).ToList();
                return leaveRequestDetails;
            }
        }

        public EmployeeAnnualBalance GetAnnualLeaveAndKeepInViewBalance(int employeeId, long? requestId, DateTime effectDay)
        {
            using (ObjectDb db = new ObjectDb("WorkingTimeRequest_AvaibleBalanceLeave"))
            {
                var employeeBalance = db.QueryEntity<EmployeeAnnualBalance>(new { empId = employeeId, requestId = requestId, EffectedDay = effectDay });

                return employeeBalance;
            }
        }

        /// <summary>
        /// Function to reset request when user reedit a leave request.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="employeeId">User reedit.</param>
        /// <param name="errorCode">Error Code.</param>
        /// <param name="message">Error Message.</param>
        /// <returns>Result of reset request.</returns>
        public bool ResetByUser(long requestId, int employeeId, out int errorCode, out string message)
        {
            using (var db = new ObjectDb("WorkingTimeRequest_ResetRequest_byMember"))
            {
                db.AddInParameter("@RequestId", DbType.Int64, requestId);
                db.AddInParameter("@EmpId", DbType.Int64, employeeId);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                var result = db.ExecuteNonQuery() != 0;

                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                return result;
            }
        }
    }
}
