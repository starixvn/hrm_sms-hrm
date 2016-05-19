namespace Fanex.NexHR.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Request;

    public interface ICheckInOutRequestRepository
    {
        RequestEntity GetCheckInOutRequestToIgnoreById(ICriteria criteria);
    }
}