// --------------------------------------------------------------------------
// <copyright file="IRequestService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>

namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Overtime;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;

    public interface IRequestService
    {
        CommandResult Delete(DeleteRequestCriteria criteria);

        CommandResult Process(ProcessRequestCriteria criteria);

        RequestEntity GetRequestById(GetRequestByIdCriteria criteria);

        IList<RequestEntity> GetRequests(GetRequestCriteria criteria);

        IList<RequestEntity> GetHistoryRequest(GetHistoryRequestCriteria criteria);

        CommandResult ProcessOvertime(ProcessSingleOvertimeCriteria criteria);

        CommandResult ProcessOvertime(ProcessMultipleOvertimeCriteria criteria);

        decimal GetNumberRequestInMonth(GetNumberRequestInMonthCriteria criteria);

        CommandResult ApproveRequest(ApproveRequestCriteria criteria);

        IList<LeaveDetailModel> GetLeaveRequestDetailToIgnoreById(GetLeaveRequestDetailToIgnoreByIdCriteria criteria);
    }
}