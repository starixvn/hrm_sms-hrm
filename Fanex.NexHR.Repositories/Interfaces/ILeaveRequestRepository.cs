namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Request;

    public interface ILeaveRequestRepository
    {
        IList<LeaveModel> GetByDate(ICriteria criteria);

        LeaveModel GetById(ICriteria criteria);

        IList<LeaveModel> GetHistoryById(ICriteria criteria);
    }
}
