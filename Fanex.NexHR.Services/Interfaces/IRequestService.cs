namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Overtime;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.ChangeShift;
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using System.Collections.Generic;

    public interface IRequestService
    {
        CommandResult Delete(DeleteRequestCriteria criteria);

        CommandResult Process(ProcessRequestCriteria criteria);

        RequestEntity GetRequestById(GetRequestByIdCriteria criteria);

        IList<RequestEntity> GetRequests(GetRequestCriteria criteria);

        IList<RequestEntity> GetHistoryRequest(GetHistoryRequestCriteria criteria);

        CommandResult ProcessOvertime(ProcessSingleOvertimeCriteria criteria);

        CommandResult ProcessOvertime(ProcessMultipleOvertimeCriteria criteria);

        decimal GetNumberRequestInMonth(GetNumberRequestInMonthCriteria criteria);

        CommandResult ApproveRequest(ApproveRequestCriteria criteria);

        string CheckRoleAccordingToRequestType(RequestViewType requestViewType);

        VerifierAndApproverForRequest GetVerifierAndApprover(GetVerifierAndApproverCriteria criteria);

        RequestEntity GetCheckInOutRequestToIgnoreById(GetCheckInOutRequestToIgnoreByIdCriteria criteria);

        RequestEntity GetlateInEarlyOutRequestToIgnoreById(GetLateInEarlyOutRequestToIgnoreByIdCriteria criteria);

        IList<ChangeShiftDetailModel> GetChangeShiftDetailToIgnoreById(GetChangeShiftDetailToIgnoreByIdCriteria criteria);

        IList<LeaveDetailModel> GetLeaveRequestDetailToIgnoreById(GetLeaveRequestDetailToIgnoreByIdCriteria criteria);
    }
}