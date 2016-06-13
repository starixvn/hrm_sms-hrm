namespace Fanex.NexHR.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Models.Request;
    using System.Collections.Generic;

    public interface IRequestRepository
    {
        RequestEntity GetRequestById(ICriteria criteria);

        IList<RequestEntity> GetRequests(ICriteria criteria);

        IList<RequestEntity> GetHistoryRequest(ICriteria criteria);

        RequestRelatedPeopleModel GetRequestRelatedPeople(GetRequestRelatedPeopleCriteria criteria);

        decimal GetNumberRequestInMonth(GetNumberRequestInMonthCriteria criteria);

        VerifierAndApproverForRequest GetVerifierAndApprover(ICriteria criteria);

        RequestEntity GetRequestToIgnoreById(ICriteria criteria);
    }
}