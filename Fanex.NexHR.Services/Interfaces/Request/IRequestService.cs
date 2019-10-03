namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Request.ChangeShift;

    public interface IRequestService
    {
        RequestEntity GetRequestById(GetRequestByIdCriteria criteria);

        IList<RequestEntity> GetRequests(GetRequestCriteria criteria);

        decimal GetNumberRequestInMonth(GetNumberRequestInMonthCriteria criteria);

        VerifierAndApproverForRequest GetVerifierAndApprover(int employeeId, RequestType requestType);

        RequestEntity GetCheckInOutDetailToReviewDiscipline(long requestId);

        RequestEntity GetLateInEarlyOutDetailToReviewDiscipline(long requestId);

        IList<ChangeShiftDetail> GetChangeShiftDetailToReviewDiscipline(long requestId);

        IList<LeaveDisciplineModel> GetLeaveRequestDetailToIgnoreById(long requestId);

        CommandResult ApproveRequest(int actorId, ProcessRequestParam param);

        CommandResult ResetRequest(int actorId, ProcessRequestParam param);

        CommandResult RejectRequest(int actorId, ProcessRequestParam param);

        CommandResult VoidRequest(int actorId, ProcessRequestParam param);

        void SendMailProcessRequest(ProcessRequestParam param, EmployeeSession actor, RequestCommand command);
    }
}