namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Request.CheckInOut;
    using Fanex.NexHR.Models.Request;

    public interface ICheckInOutRequestService
    {
        IList<CheckInOutModel> GetByDate(GetCheckInOutRequestByDateCriteria criteria);

        CheckInOutModel GetById(GetCheckInOutRequestByIdCriteria criteria);

        IList<CheckInOutModel> GetHistoryById(GetCheckInOutRequestHistoryByIdCriteria criteria);

        void SendMailForInsertSuccessfully(InsertCheckInOutRequestCriteria criteria);

        void SendMailForUpdateSuccessfully(UpdateCheckInOutRequestCriteria criteria);

        void SendMailForDeleteSuccessfully(long requestId);
    }
}