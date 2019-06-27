namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Request;

    public interface ICheckInOutRequestRepository
    {
        IList<CheckInOutModel> GetByDate(ICriteria criteria);

        CheckInOutModel GetById(ICriteria criteria);

        IList<CheckInOutModel> GetHistoryById(ICriteria criteria);
    }
}
