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
    using Fanex.NexHR.ViewModels;

    public interface ILeaveRequestService
    {
        long Insert(LeaveRequestModel model, out int errorCode, out string message);

        bool Update(LeaveRequestModel model, out int errorCode, out string message);

        bool DeleteWorkingTimeRequest(long id);

        List<LeaveDetailModel> GetLeaveRequestDetailsById(long id);

        LeaveRequestViewModel GetRequestById(long id);

        void SendReasonReeditToSupervisor(long requestId, int processBy, string reason, out int errorCode, out string errorMessage);

        EmployeeAnnualBalance GetAnnualLeaveAndKeepInViewBalance(int employeeId, long? requestId, DateTime effectDay);
    }
}
