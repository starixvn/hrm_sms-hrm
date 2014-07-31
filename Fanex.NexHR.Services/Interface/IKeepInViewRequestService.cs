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
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    public interface IKeepInViewRequestService
    {
        bool Insert(ModelStateDictionary modelState, KeepInViewRequestViewModel model, bool isTopManager);

        bool Update(ModelStateDictionary modelState, KeepInViewRequestViewModel model, bool isTopManager);
    }
}
