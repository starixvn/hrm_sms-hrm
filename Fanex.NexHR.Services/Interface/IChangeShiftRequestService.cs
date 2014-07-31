// --------------------------------------------------------------------------
// <copyright file="IChangeShiftRequestService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2013.11.14">Created</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Interface Change Shift Request Service.
    /// </summary>
    public interface IChangeShiftRequestService
    {
        bool Insert(ModelStateDictionary modelState, ChangeShiftViewModel model);

        bool Update(ModelStateDictionary modelState, ChangeShiftViewModel model);

        List<ShiftDetails> GetChangeShiftDetailsById(long id);
    }
}
