namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.ChangeShift;
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Criteria.Request.Leave;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Request.ChangeShift;

    public interface IRequestService
    {
        RequestEntity GetRequestById(GetRequestByIdCriteria criteria);

        IList<RequestEntity> GetRequests(GetRequestCriteria criteria);

        decimal GetNumberRequestInMonth(GetNumberRequestInMonthCriteria criteria);

        VerifierAndApproverForRequest GetVerifierAndApprover(int employeeId, RequestType requestType);

        RequestEntity GetCheckInOutRequestToIgnoreById(GetCheckInOutRequestToIgnoreByIdCriteria criteria);

        RequestEntity GetlateInEarlyOutRequestToIgnoreById(GetLateInEarlyOutRequestToIgnoreByIdCriteria criteria);

        IList<ChangeShiftDetail> GetChangeShiftDetailToIgnoreById(GetChangeShiftDetailToIgnoreByIdCriteria criteria);

        IList<LeaveDisciplineModel> GetLeaveRequestDetailToIgnoreById(GetLeaveRequestDetailToIgnoreByIdCriteria criteria);

        CommandResult Delete(long requestId);
    }
}