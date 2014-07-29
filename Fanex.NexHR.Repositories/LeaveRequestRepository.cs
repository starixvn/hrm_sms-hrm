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
        public IList<LeaveDetailModel> GetLeaveRequestDetailsById(long requestId)
        {
            using (ObjectDb db = new ObjectDb("WorkingTimeRequest_GetLeaveRequestDetailByRequestID"))
            {
                var leaveRequestDetails = db.Query<LeaveDetailModel>(new { RequestID = requestId }).ToList();
                return leaveRequestDetails;
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
