namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.Leave;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;

    public interface ILeaveRequestService
    {
        IList<LeaveDetailModel> GetLeaveRequestDetailById(GetLeaveRequestDetailByIdCriteria criteria);

        EmployeeLeaveBalance GetLeaveBalance(GetLeaveBalanceCriteria criteria);

        void SendMailForInsertSuccessfully(InsertLeaveRequestCriteria criteria);

        void SendMailForUpdateSuccessfully(UpdateLeaveRequestCriteria criteria);
    }
}