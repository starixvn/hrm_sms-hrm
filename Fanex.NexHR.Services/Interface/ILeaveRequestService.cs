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
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.Leave;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    public interface ILeaveRequestService
    {
        CommandResult Insert(InsertLeaveRequestCriteria criteria);

        CommandResult Update(UpdateLeaveRequestCriteria criteria);

        bool DeleteWorkingTimeRequest(long id);

        IList<LeaveDetailModel> GetLeaveRequestDetailsById(long id);

        LeaveRequestViewModel GetRequestById(long id);

        void SendReasonReeditToSupervisor(long requestId, int processBy, string reason, out int errorCode, out string errorMessage);

        EmployeeLeaveBalance GetLeaveBalance(GetLeaveBalanceCriteria criteria);
    }
}
