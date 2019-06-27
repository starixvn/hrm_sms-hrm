namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Request;

    public interface ILateInEarlyOutRequestRepository
    {
        IList<LateInEarlyOutModel> GetByDate(ICriteria criteria);

        LateInEarlyOutModel GetById(ICriteria criteria);

        IList<LateInEarlyOutModel> GetHistoryById(ICriteria criteria);
    }
}
