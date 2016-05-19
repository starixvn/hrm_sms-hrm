namespace Fanex.NexHR.Repositories.Implement
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories.Interfaces;

    public class CheckInOutRequestRepository : ICheckInOutRequestRepository
    {
        public RequestEntity GetCheckInOutRequestToIgnoreById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.QueryEntity<RequestEntity, Shift, Employee, RequestTypeModel, RequestEntity>(
                    (request, shift, requester, type) =>
                    {
                        request.RequestType = type;
                        request.FromShift = shift;
                        request.Requester = requester;

                        return request;
                    },
                    "ShiftId, RequesterId, RequestType",
                    criteria.GetSpParams());
            }
        }
    }
}