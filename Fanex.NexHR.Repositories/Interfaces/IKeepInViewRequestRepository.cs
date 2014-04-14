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

        IList<RequestEntity> GetRequests(
            int requestType,
            string role,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            string status,
            bool isDailyRequest = false,
            int? employeeId = null,
            int? logInId = null,
            byte? isLocal = null);

        RequestEntity GetRequestById(long id, int requestTypeId);

        double ConvertHourFromStringToDouble(string hour);
    }
}
