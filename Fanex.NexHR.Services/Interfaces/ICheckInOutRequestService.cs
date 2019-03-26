namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Criteria.Request.CheckInOut;

    public interface ICheckInOutRequestService
    {
        void SendMailForInsertSuccessfully(InsertCheckInOutRequestCriteria criteria);

        void SendMailForUpdateSuccessfully(UpdateCheckInOutRequestCriteria criteria);
    }
}