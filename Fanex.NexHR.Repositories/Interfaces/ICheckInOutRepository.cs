// --------------------------------------------------------------------------
// <copyright file="ICheckInOutRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2013.11.14">Created</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Request;

    /// <summary>
    /// Interface Check InOut Repository.
    /// </summary>
    public interface ICheckInOutRepository
    {
        long Insert(object model, out int errorCode, out string message);

        bool Update(object model, out int errorCode, out string message);
    }
}
