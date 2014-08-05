// --------------------------------------------------------------------------
// <copyright file="IChangeShiftRequestService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2013.11.14">Created</change>
// </history>

namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Request.ChangeShift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels.Request;

    /// <summary>
    /// Interface Change Shift Request Service.
    /// </summary>
    public interface IChangeShiftRequestService
    {
        CommandResult Insert(InsertChangeShiftRequestCriteria criteria);

        CommandResult Update(UpdateChangeShiftRequestCriteria criteria);

        IList<ChangeShiftDetailModel> GetChangeShiftRequestDetailById(GetChangeShiftRequestDetailByIdCriteria criteria);
    }
}
