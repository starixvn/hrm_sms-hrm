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
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Models.Request;

    public interface IRequestRepository
    {
        RequestEntity GetRequestById(ICriteria criteria);

        IList<RequestEntity> GetRequests(ICriteria criteria);

        IList<RequestEntity> GetHistoryRequest(ICriteria criteria);

        RequestRelatedPeopleModel GetRequestRelatedPeople(GetRequestRelatedPeopleCriteria criteria);

        decimal GetNumberRequestInMonth(GetNumberRequestInMonthCriteria criteria);
    }
}
