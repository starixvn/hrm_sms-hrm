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
    using Fanex.NexHR.Criteria.Request.KeepInView;
    using Fanex.NexHR.Models;

    public interface IKeepInViewRequestService
    {
        CommandResult Insert(InsertKeepInViewRequestCriteria criteria);

        CommandResult Update(UpdateKeepInViewRequestCriteria criteria);
    }
}
