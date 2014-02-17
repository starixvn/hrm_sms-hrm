// --------------------------------------------------------------------------
// <copyright file="IChangeShiftRepository.cs" company="Nexcel Solutions Vietnam">
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
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Interface Change shit repository.
    /// </summary>
    public interface IChangeShiftRepository
    {
        long InsertChangeShiftRequest(ChangeShiftViewModel model, out int errorCode, out string message);

        long UpdateChangeShiftRequest(ChangeShiftViewModel model, out int errorCode, out string message);

        IList<RequestEntity> GetChangeShiftRequests(
            string role,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest,
            int? employeeId = null,
            byte? status = null,
            int? logInId = null,
            byte? isLocal = null);

        RequestEntity GetChangeShiftRequestById(long id);

        List<ShiftDetails> GetShiftDetailsById(long id);

        IList<RequestEntity> GetHistoricChangeShiftRequests(long requestId, int requestTypeId);
    }
}
