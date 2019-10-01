namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Request.KeepInView;
    using Fanex.NexHR.Models.Request;

    public interface IKeepInViewRequestService
    {
        IList<KeepInViewRequest> GetByDate(GetKeepInViewRequestByDateCriteria criteria);

        KeepInViewRequest GetById(long requestId);

        IList<KeepInViewRequest> GetHistoryById(long requestId);

        CommandResult Delete(long requestId);

        void SendMailForInsertSuccessfully(InsertKeepInViewRequestCriteria criteria);

        void SendMailForUpdateSuccessfully(UpdateKeepInViewRequestCriteria criteria);

        void SendMailForDeleteSuccessfully(long requestId);
    }
}