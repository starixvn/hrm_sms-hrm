// --------------------------------------------------------------------------
// <copyright file="ILateInEarlyOutRequestService.cs" company="Nexcel Solutions Vietnam">
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
    public interface ILateInEarlyOutRequestService
    {
        bool InsertLateInEarlyOutRequest(ModelStateDictionary modelState, LateInEarlyOutRequestViewModel model);

        bool UpdateLateInEarlyOutRequest(ModelStateDictionary modelState, LateInEarlyOutRequestViewModel model);

        IList<RequestEntity> GetLateInEarlyOutRequests(
           string role,
           DateTime fromDate,
           DateTime toDate,
           string groupIds,
           bool isDailyRequest = false,
           int? employeeId = null,
           byte? status = null,
           int? logInId = null);

        RequestEntity GetLateInEarlyOutRequestById(long id);

        IList<RequestEntity> GetHistoricLateInEarlyOutRequests(long requestId, int requestTypeId);
    }
}
