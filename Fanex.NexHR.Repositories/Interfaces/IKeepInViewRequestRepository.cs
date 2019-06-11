namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Request;

    public interface IKeepInViewRequestRepository
    {
        IList<KeepInViewModel> GetByDate(ICriteria criteria);

        KeepInViewModel GetById(ICriteria criteria);

        IList<KeepInViewModel> GetHistoryById(ICriteria criteria);
    }
}
