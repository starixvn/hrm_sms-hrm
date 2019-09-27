namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Models.Request;

    public interface ILateInEarlyOutRequestService
    {
        IList<LateInEarlyOutModel> GetByDate(GetLateInEarlyOutRequestByDateCriteria criteria);

        LateInEarlyOutModel GetById(long requestId);

        IList<LateInEarlyOutModel> GetHistoryById(long requestId);

        CommandResult Delete(long requestId);

        EmployeeLateInEarlyOutModel GetActualLateInEarlyOut(GetActualLateInEarlyOutCriteria criteria);

        void SendMailForInsertSuccessfully(InsertLateInEarlyOutRequestCriteria criteria);

        void SendMailForUpdateSuccessfully(UpdateLateInEarlyOutRequestCriteria criteria);

        void SendMailForDeleteSuccessfully(long requestId);
    }
}