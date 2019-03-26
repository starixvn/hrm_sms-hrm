namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.ChangeShift;
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Models.Request;

    public interface IRequestService
    {
        RequestEntity GetRequestById(GetRequestByIdCriteria criteria);

        IList<RequestEntity> GetRequests(GetRequestCriteria criteria);

        IList<RequestEntity> GetHistoryRequest(GetHistoryRequestCriteria criteria);

        decimal GetNumberRequestInMonth(GetNumberRequestInMonthCriteria criteria);

        VerifierAndApproverForRequest GetVerifierAndApprover(GetVerifierAndApproverCriteria criteria);

        RequestEntity GetCheckInOutRequestToIgnoreById(GetCheckInOutRequestToIgnoreByIdCriteria criteria);

        RequestEntity GetlateInEarlyOutRequestToIgnoreById(GetLateInEarlyOutRequestToIgnoreByIdCriteria criteria);

        IList<ChangeShiftDetailModel> GetChangeShiftDetailToIgnoreById(GetChangeShiftDetailToIgnoreByIdCriteria criteria);

        IList<LeaveDetailModel> GetLeaveRequestDetailToIgnoreById(GetLeaveRequestDetailToIgnoreByIdCriteria criteria);
    }
}