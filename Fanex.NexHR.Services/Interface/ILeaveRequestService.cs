// --------------------------------------------------------------------------
// <copyright file="ILeaveRequestService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.11">Created</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    public interface ILeaveRequestService
    {
        long InsertLeaveRequest(LeaveRequestModel model, out int errorCode, out string message);

        bool UpdateLeaveRequest(LeaveRequestModel model, out int errorCode, out string message);

        bool DeleteWorkingTimeRequest(int id);

        IList<RequestEntity> GetLeaveRequests(
            string role,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? logInId = null);

        List<LeaveDetailModel> GetLeaveRequestDetailsById(long id);

        LeaveRequestViewModel GetLeaveRequestById(long id);

        IList<RequestEntity> GetHistoricLeaveRequests(long requestId, int requestTypeId);

        void SendReasonReeditToSupervisor(long requestId, int processBy, string reason, out int errorCode, out string errorMessage);

        EmployeeAnnualBalance GetAnnualLeaveAndKeepInViewBalance(int employeeId, long? requestId, DateTime effectDay);
    }
}
