// --------------------------------------------------------------------------
// <copyright file="IRequestRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.9.9">Created</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    public interface IRequestRepository
    {
        bool DeleteWorkingTimeRequest(long id);

        int ProcessRequest(
            string command,
            long requestId,
            int roleId,
            int processedBy,
            string remark,
            DateTime processedDate,
            out string message);

        IList<WorkingTimeRequest> GetWorkingTimeRequests(int id, int? requestType = null, byte? status = null);

        WorkingTimeRequest GetWorkingTimeRequest(long workingTimeId);

        IList<RequestEntity> GetRequests(
            int requestType,
            string role,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            string status,
            bool isDailyRequest,
            int? employeeId = null,
            int? logInId = null, 
            byte? isLocal = null);

        IList<SupervisorNameModel> GetSupervisorByQuery(int empId, int role, int? requestType, string query, bool includeLoginUser);

        void GetSupervisorAndHrByRequestId(
            long requestId,
            int action,
            ref SupervisorNameModel employee,
            ref SupervisorNameModel verifier, 
            ref SupervisorNameModel approver, 
            ref IList<SupervisorNameModel> listHr,
            ref EmailSettingsModel emailSettings);

        int ProcessOvertime(
            string command,
            long requestId,
            int roleId,
            int processedBy,
            DateTime processedDate,
            out string message);

        int ProcessOvertime(
            string command,
            string requestIds,
            int processedBy,
            DateTime processedDate,
            out string message);
    }
}
