// --------------------------------------------------------------------------
// <copyright file="IRequestService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Criteria.Request;

    public interface IRequestService
    {
        bool DeleteWorkingTimeRequest(long id);

        bool Process(string command, long requestId, int roleId, int processBy, string remark, out string errorMessage);

        IList<RequestEntity> GetRequests(
            RequestViewType requestViewType,
            int requestType,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            int statusId,
            bool isDailyRequest = false,
            int? employeeId = null,
            int? loginId = null,
            byte? isLocal = null);

        IList<RequestEntity> GetHistoryRequest(long requestId, int requestTypeId);

        IList<SupervisorNameModel> GetSupervisorByQuery(int empId, int role, int? requestType, string query, bool includeLoginUser);

        bool ProcessOvertime(string command, long requestId, int roleId, int processBy, out string errorMessage);

        bool ProcessOvertime(string command, string requestIds, int processBy, out string errorMessage);

        decimal GetNumberRequestInMonth(GetNumberRequestInMonthCriteria criteria);
    }
}
