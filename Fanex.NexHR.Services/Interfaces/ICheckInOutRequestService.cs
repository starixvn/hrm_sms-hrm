namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Request.CheckInOut;
    using Fanex.NexHR.Models.Request;

    public interface ICheckInOutRequestService
    {
        IList<CheckInOutRequest> GetByDate(GetCheckInOutRequestByDateCriteria criteria);

        CheckInOutRequest GetById(long requestId);

        IList<CheckInOutRequest> GetHistoryById(long requestId);

        CommandResult Delete(long requestId);

        void SendMailForInsertSuccessfully(InsertCheckInOutRequestCriteria criteria);

        void SendMailForUpdateSuccessfully(UpdateCheckInOutRequestCriteria criteria);

        void SendMailForDeleteSuccessfully(long requestId);
    }
}