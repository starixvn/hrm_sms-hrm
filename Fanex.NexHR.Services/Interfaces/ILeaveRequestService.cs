namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.Leave;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;

    public interface ILeaveRequestService
    {
        IList<LeaveModel> GetByDate(GetLeaveRequestByDateCriteria criteria);

        LeaveModel GetById(GetLeaveRequestByIdCriteria criteria);

        IList<LeaveDetailModel> GetLeaveRequestDetailById(GetLeaveRequestDetailByIdCriteria criteria);

        IList<LeaveModel> GetHistoryById(GetLeaveRequestHistoryByIdCriteria criteria);

        EmployeeLeaveBalance GetLeaveBalance(GetLeaveBalanceCriteria criteria);

        void SendMailForInsertSuccessfully(InsertLeaveRequestCriteria criteria);

        void SendMailForUpdateSuccessfully(UpdateLeaveRequestCriteria criteria);

        void SendMailForDeleteSuccessfully(long requestId);
    }
}