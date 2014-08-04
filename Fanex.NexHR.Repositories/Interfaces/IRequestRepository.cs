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
    using Fanex.Data.Repository;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;

    public interface IRequestRepository
    {
        bool DeleteWorkingTimeRequest(long id);

        WorkingTimeRequest GetWorkingTimeRequest(long requestId);

        RequestEntity GetRequestById(ICriteria criteria);

        IList<RequestEntity> GetRequests(ICriteria criteria);

        IList<RequestEntity> GetHistoryRequest(long requestId, int requestTypeId);

        IList<SupervisorNameModel> GetSupervisorByQuery(int empId, int role, int? requestType, string query, bool includeLoginUser);

        RequestRelatedPeopleModel GetRequestRelatedPeople(GetRequestRelatedPeopleCriteria criteria);

        decimal GetNumberRequestInMonth(GetNumberRequestInMonthCriteria criteria);
    }
}
