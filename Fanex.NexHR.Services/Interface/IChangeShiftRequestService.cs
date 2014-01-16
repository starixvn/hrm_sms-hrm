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
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IChangeShiftRequestService
    {
        bool InsertChangeShiftRequest(ModelStateDictionary modelState, ChangeShiftViewModel model);

        bool UpdateChangeShiftRequest(ModelStateDictionary modelState, ChangeShiftViewModel model);

        IList<RequestEntity> GetChangeShiftRequests(
            string role,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? logInId = null);

        RequestEntity GetChangeShiftRequestById(long id);

        List<ShiftDetails> GetShiftDetailsById(long id);

        IList<RequestEntity> GetHistoricChangeShiftRequests(long requestId, int requestTypeId);
    }
}
