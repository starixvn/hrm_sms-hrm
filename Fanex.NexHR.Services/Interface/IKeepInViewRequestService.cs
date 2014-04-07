// --------------------------------------------------------------------------
// <copyright file="IKeepInViewRequestService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Paul.Nguyen" date="2013.11.14">Created</change>
// </history>
namespace Fanex.NexHR.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    public interface IKeepInViewRequestService
    {
        bool InsertKeepInViewRequest(ModelStateDictionary modelState, KeepInViewRequestViewModel keepInViewRequestViewModel, bool isTopManager);

        bool UpdateKeepInViewRequest(ModelStateDictionary modelState, KeepInViewRequestViewModel keepInViewRequestViewModel, bool isTopManager);

        RequestEntity GetKeepInViewRequestById(long id);

        IList<RequestEntity> GetKeepInViewRequests(
            string role,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? logInId = null);

        IList<RequestEntity> GetHistoricKeepInViewRequests(long requestId, int requestTypeId);
    }
}
