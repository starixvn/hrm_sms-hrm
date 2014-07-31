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
    using System.Web.Mvc;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Interface Late In Early Out Request Service.
    /// </summary>
    public interface ILateInEarlyOutRequestService
    {
        bool Insert(ModelStateDictionary modelState, LateInEarlyOutRequestViewModel model);

        bool Update(ModelStateDictionary modelState, LateInEarlyOutRequestViewModel model);
    }
}
