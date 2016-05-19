namespace Fanex.NexHR.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Request;

    public interface ILateInEarlyOutRequestRepository
    {
        RequestEntity GetLateInEarlyOutRequestToIgnoreById(ICriteria criteria);
    }
}