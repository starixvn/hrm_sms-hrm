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
        long Insert(object model, out int errorCode, out string message);

        bool Update(object model, out int errorCode, out string message);

        IList<RequestEntity> GetRequests(
            int requestType,
            string role,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            string status,
            bool isDailyRequest = false,
            int? employeeId = null,
            int? logInId = null,
            byte? isLocal = null);

        RequestEntity GetRequestById(long id, int requestTypeId);

        IList<LeaveDetailModel> GetLeaveRequestDetailsById(long id);

        EmployeeAnnualBalance GetAnnualLeaveAndKeepInViewBalance(int employeeId, long? requestId, DateTime effectDay);

        bool ResetByUser(long requestId, int employeeId, out int errorCode, out string message);
    }
}
