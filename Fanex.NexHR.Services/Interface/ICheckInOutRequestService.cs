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
    using System.Web.Mvc;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Interface Check In Out Request Service.
    /// </summary>
    public interface ICheckInOutRequestService
    {
        bool Insert(ModelStateDictionary modelState, CheckInOutRequestViewModel model);

        bool Update(ModelStateDictionary modelState, CheckInOutRequestViewModel model);

        RequestEntity GetRequestById(long id);
    }
}
