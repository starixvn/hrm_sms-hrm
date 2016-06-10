namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.CheckInOut;
    using Fanex.NexHR.Models.Request;

    public interface ICheckInOutRequestService
    {
        RequestEntity GetCheckInOutRequestToIgnoreById(GetCheckInOutRequestToIgnoreByIdCriteria criteria);

        void SendMailForInsertSuccessfully(InsertCheckInOutRequestCriteria criteria);

        void SendMailForUpdateSuccessfully(UpdateCheckInOutRequestCriteria criteria);
    }
}