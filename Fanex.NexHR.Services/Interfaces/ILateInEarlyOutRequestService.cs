// --------------------------------------------------------------------------
// <copyright file="ILateInEarlyOutRequestService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2013.11.14">Created</change>
// </history>

namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;

    /// <summary>
    /// Interface Late In Early Out Request Service.
    /// </summary>
    public interface ILateInEarlyOutRequestService
    {
        CommandResult Insert(InsertLateInEarlyOutRequestCriteria criteria);

        CommandResult Update(UpdateLateInEarlyOutRequestCriteria criteria);

        EmployeeLateInEarlyOutModel GetActualLateInEarlyOut(GetActualLateInEarlyOutCriteria criteria);
    }
}
