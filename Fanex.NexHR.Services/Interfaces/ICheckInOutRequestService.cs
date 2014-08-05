// --------------------------------------------------------------------------
// <copyright file="ICheckInOutRequestService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2013.11.14">Created</change>
// </history>

namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Request.CheckInOut;
    using Fanex.NexHR.Models;

    /// <summary>
    /// Interface Check In Out Request Service.
    /// </summary>
    public interface ICheckInOutRequestService
    {
        CommandResult Insert(InsertCheckInOutRequestCriteria criteria);

        CommandResult Update(UpdateCheckInOutRequestCriteria criteria);
    }
}
