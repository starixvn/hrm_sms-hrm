namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Request.KeepInView;
    using Fanex.NexHR.Models;

    public interface IKeepInViewRequestService
    {
        void SendMailForInsertSuccessfully(InsertKeepInViewRequestCriteria criteria);

        void SendMailForUpdateSuccessfully(UpdateKeepInViewRequestCriteria criteria);
    }
}
