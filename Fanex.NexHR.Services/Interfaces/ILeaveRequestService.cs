namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.Leave;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using System.Collections.Generic;

    public interface ILeaveRequestService
    {
        IList<LeaveDetailModel> GetLeaveRequestDetailById(GetLeaveRequestDetailByIdCriteria criteria);

        EmployeeLeaveBalance GetLeaveBalance(GetLeaveBalanceCriteria criteria);

        void SendMailForInsertSuccessfully(InsertLeaveRequestCriteria criteria);

        void SendMailForUpdateSuccessfully(UpdateLeaveRequestCriteria criteria);
    }
}
