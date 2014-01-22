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
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    public interface IKeepInViewRequestRepository
    {
        bool DeleteWorkingTimeRequest(long id);

        long InsertKeepInViewRequest(
            int empId,
            short requestType,
            string remark,
            DateTime date,
            bool isHalfDay,
            int shiftId,
            DateTime? fromTime,
            DateTime? toTime,
            int createdBy,
            DateTime createdDate,
            int? verifierId,
            int? approverId,
            string refNo,
            out int errorCode,
            out string message);

        bool UpdateKeepInViewRequest(
            long id,
            int empId,
            short requestType,
            string remark,
            DateTime date,
            bool isHalfDay,
            int shiftId,
            DateTime? fromTime,
            DateTime? toTime,
            int modifiedBy,
            DateTime modifiedDate,
            int? verifierId,
            int? approverId,
            string refNo,
            out int errorCode,
            out string messagee);

        IList<RequestEntity> GetKeepInViewRequests(
            string role,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest,
            int? employeeId = null,
            byte? status = null,
            int? logInId = null,
            byte? isLocal = null);

        RequestEntity GetKeepInViewRequestById(long id);

        IList<RequestEntity> GetHistoricKeepInViewRequests(long requestId, int requestTypeId);
    }
}
