namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Request.CheckInOut;
    using Fanex.NexHR.Models.Request;

    public interface ICheckInOutRequestService
    {
        IList<CheckInOutRequest> GetByDate(GetCheckInOutRequestByDateCriteria criteria);

        CheckInOutRequest GetById(long requestId);

        IList<CheckInOutRequest> GetHistoryById(long requestId);

        CommandResult Delete(long requestId);

        CommandResult ApproveRequest(int actorId, ProcessRequestParam param);

        CommandResult ResetRequest(int actorId, ProcessRequestParam param);

        CommandResult RejectRequest(int actorId, ProcessRequestParam param);

        CommandResult VoidRequest(int actorId, ProcessRequestParam param);

        void SendMailCreateRequest(long requestId);

        void SendMailEditRequest(long requestId);

        void SendMailDeleteRequest(long requestId);

        void SendMailProcessRequest(long requestId, EmployeeSession actor, RequestCommand command);
    }
}