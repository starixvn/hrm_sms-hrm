namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Request.KeepInView;
    using Fanex.NexHR.Models.Request;

    public interface IKeepInViewRequestService
    {
        IList<KeepInViewModel> GetByDate(GetKeepInViewRequestByDateCriteria criteria);

        KeepInViewModel GetById(GetKeepInViewRequestByIdCriteria criteria);

        IList<KeepInViewModel> GetHistoryById(GetKeepInViewHistoryByIdCriteria criteria);

        void SendMailForInsertSuccessfully(InsertKeepInViewRequestCriteria criteria);

        void SendMailForUpdateSuccessfully(UpdateKeepInViewRequestCriteria criteria);

        void SendMailForDeleteSuccessfully(long requestId);
    }
}