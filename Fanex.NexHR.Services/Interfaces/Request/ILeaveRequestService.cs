namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.Leave;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;

    public interface ILeaveRequestService
    {
        IList<LeaveRequest> GetByDate(GetLeaveRequestByDateCriteria criteria);

        LeaveRequest GetById(long requestId);

        IList<LeaveRequestDetail> GetLeaveRequestDetailById(long requestId);

        IList<LeaveRequest> GetHistoryById(long requestId);

        EmployeeLeaveBalance GetLeaveBalance(GetLeaveBalanceCriteria criteria);

        void SendMailForInsertSuccessfully(InsertLeaveRequestCriteria criteria);

        void SendMailForUpdateSuccessfully(UpdateLeaveRequestCriteria criteria);

        void SendMailForDeleteSuccessfully(long requestId);

        DateTime GetFromTimeHalfDay(IList<LeaveRequestDetail> leaveDetails);

        DateTime GetToTimeHalfDay(IList<LeaveRequestDetail> leaveDetails);

        CommandResult Delete(long requestId);
    }
}