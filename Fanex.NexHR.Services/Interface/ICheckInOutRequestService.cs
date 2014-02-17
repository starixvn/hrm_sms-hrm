// --------------------------------------------------------------------------
// <copyright file="ICheckInOutRequestService.cs" company="Nexcel Solutions Vietnam">
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
    /// Interface Check In Out Request Service.
    /// </summary>
    public interface ICheckInOutRequestService
    {
        bool InsertCheckInOutRequest(ModelStateDictionary modelState, CheckInOutRequestViewModel model);

        bool UpdateCheckInOutRequest(ModelStateDictionary modelState, CheckInOutRequestViewModel model);

        RequestEntity GetCheckInOutRequestById(long id);

        IList<RequestEntity> GetCheckInOutRequests(
            string role,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? logInId = null);

        IList<RequestEntity> GetHistoricCheckInOutRequests(long requestId, int requestTypeId);
    }
}
