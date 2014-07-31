// --------------------------------------------------------------------------
// <copyright file="IKeepInViewRequestRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Paul.Nguyen" date="2013.11.14">Created</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Request;

    public interface IKeepInViewRequestRepository
    {
        long Insert(object model, out int errorCode, out string message);

        bool Update(object model, out int errorCode, out string message);

        double ConvertHourFromStringToDouble(string hour);
    }
}
