// --------------------------------------------------------------------------
// <copyright file="ILeaveRequestRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.11">Created</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;

    public interface ILeaveRequestRepository
    {
        long InsertLeaveRequest(LeaveRequestModel model, out int errorCode, out string message);

        bool UpdateLeaveRequest(LeaveRequestModel model, out int errorCode, out string message);

        bool DeleteWorkingTimeRequest(long id);

        IList<RequestEntity> GetLeaveRequests(
            string role,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest,
            int? employeeId = null,
            byte? status = null,
            int? logInId = null);

        List<LeaveDetailModel> GetLeaveRequestDetailsById(long id);

        RequestEntity GetLeaveRequestById(long id);

        IList<RequestEntity> GetHistoricLeaveRequests(long requestId, int requestTypeId);

        EmployeeAnnualBalance GetAnnualLeaveAndKeepInViewBalance(int employeeId, long? requestId, DateTime effectDay);

        bool ResetByUser(long requestId, int employeeId, out int errorCode, out string message);
    }
}
