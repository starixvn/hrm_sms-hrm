namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Models.Request;

    public interface ILateInEarlyOutRequestService
    {
        IList<LateInEarlyOutRequest> GetByDate(GetLateInEarlyOutRequestByDateCriteria criteria);

        LateInEarlyOutRequest GetById(long requestId);

        IList<LateInEarlyOutRequest> GetHistoryById(long requestId);

        CommandResult Delete(long requestId);

        EmployeeLateInEarlyOutModel GetActualLateInEarlyOut(GetActualLateInEarlyOutCriteria criteria);

        CommandResult ApproveRequest(int actorId, ProcessRequestParam param);

        CommandResult ResetRequest(int actorId, ProcessRequestParam param);

        CommandResult RejectRequest(int actorId, ProcessRequestParam param);

        CommandResult VoidRequest(int actorId, ProcessRequestParam param);

        void SendMailForInsertSuccessfully(InsertLateInEarlyOutRequestCriteria criteria);

        void SendMailForUpdateSuccessfully(UpdateLateInEarlyOutRequestCriteria criteria);

        void SendMailForDeleteSuccessfully(long requestId);

        void SendMailAfterProcessRequest(long requestId, EmployeeSession actor, RequestCommand command);
    }
}