namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Models.Request;

    public interface ILateInEarlyOutRequestService
    {
        IList<LateInEarlyOutModel> GetByDate(GetLateInEarlyOutRequestByDateCriteria criteria);

        LateInEarlyOutModel GetById(GetLateInEarlyOutRequestByIdCriteria criteria);

        IList<LateInEarlyOutModel> GetHistoryById(GetLateInEarlyOutRequestHistoryByIdCriteria criteria);

        EmployeeLateInEarlyOutModel GetActualLateInEarlyOut(GetActualLateInEarlyOutCriteria criteria);

        void SendMailForInsertSuccessfully(InsertLateInEarlyOutRequestCriteria criteria);

        void SendMailForUpdateSuccessfully(UpdateLateInEarlyOutRequestCriteria criteria);

        void SendMailForDeleteSuccessfully(long requestId);
    }
}