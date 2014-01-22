// --------------------------------------------------------------------------
// <copyright file="ILateInEarlyOutRepository.cs" company="Nexcel Solutions Vietnam">
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
    /// TODO: Update summary.
    /// </summary>
    public interface ILateInEarlyOutRepository
    {
        long InsertLateInEarlyOutRequest(
            int empId,
            short requestType,
            string remark,
            DateTime createdDate,
            int createdBy,
            int? verifierId,
            int? approverId,
            out int errorCode,
            out string message,
            DateTime requestDate,
            int? lateInMins = null,
            int? earlyOutMins = null);

        bool UpdateLateInEarlyOutRequest(
            int requestId,
            int empId,
            int? requestType,
            string remark,
            DateTime requestDate,
            int? verifierId,
            int? approverId,
            DateTime modifiedDate,
            int modifiedBy,
            out int errorCode,
            out string message,
            int? lateInMins = null,
            int? earlyOutMins = null);

        IList<RequestEntity> GetLateInEarlyOutRequests(
           string role,
           DateTime fromDate,
           DateTime toDate,
           string groupIds,
            bool isDailyRequest,
           int? employeeId = null,
           byte? status = null,
           int? longInId = null,
            byte? isLocal = null);

        RequestEntity GetLateInEarlyOutRequestById(long id);

        IList<RequestEntity> GetHistoricLateInEarlyOutRequests(long requestId, int requestTypeId);
    }
}
