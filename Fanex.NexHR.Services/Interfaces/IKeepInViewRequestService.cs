namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Criteria.Request.KeepInView;

    public interface IKeepInViewRequestService
    {
        void SendMailForInsertSuccessfully(InsertKeepInViewRequestCriteria criteria);

        void SendMailForUpdateSuccessfully(UpdateKeepInViewRequestCriteria criteria);
    }
}