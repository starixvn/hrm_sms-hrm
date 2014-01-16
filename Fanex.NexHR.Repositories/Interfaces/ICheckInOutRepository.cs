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
    using System.Linq;
    using System.Text;
    using Fanex.NexHR.Models.Request;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ICheckInOutRepository
    {
        long InsertCheckInOutRequest(
           int empId,
           int requestType,
           string remark,
           DateTime requestDate,
           DateTime createdDate,
           int createdBy,
           int? verifierId,
           int? approverId,
           string refNo,
            DateTime attendantDate,
           out int errorCode,
           out string message);

        bool UpdateCheckInOutRequest(
            int requestId,
            int empId,
            int? requestType,
            string remark,
            DateTime requestDate,
            int? verifierId,
            int? approverId,
            DateTime modifiedDate,
            int modifiedBy,
            string refNo,
            DateTime attendantDate);

        RequestEntity GetCheckInOutRequestById(long id);

        IList<RequestEntity> GetCheckInOutRequests(
            string role,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest,
            int? employeeId = null,
            byte? status = null,
            int? logInId = null);

        IList<RequestEntity> GetCheckInOutRequestHistory(long requestId, int requestTypeId);
    }
}
