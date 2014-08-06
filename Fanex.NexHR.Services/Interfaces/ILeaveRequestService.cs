// --------------------------------------------------------------------------
// <copyright file="ILeaveRequestService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.11">Created</change>
// </history>

namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.Leave;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;

    public interface ILeaveRequestService
    {
        CommandResult Insert(InsertLeaveRequestCriteria criteria);

        CommandResult Update(UpdateLeaveRequestCriteria criteria);

        IList<LeaveDetailModel> GetLeaveRequestDetailById(GetLeaveRequestDetailByIdCriteria criteria);

        CommandResult SendReasonReeditToSupervisor(ResetLeaveRequestCriteria criteria);

        EmployeeLeaveBalance GetLeaveBalance(GetLeaveBalanceCriteria criteria);
    }
}
