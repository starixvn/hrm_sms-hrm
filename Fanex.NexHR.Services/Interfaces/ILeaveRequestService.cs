namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.Leave;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;

    public interface ILeaveRequestService
    {
        IList<LeaveModel> GetByDate(GetLeaveRequestByDateCriteria criteria);

        LeaveModel GetById(long requestId);

        IList<LeaveDetailModel> GetLeaveRequestDetailById(long requestId);

        IList<LeaveModel> GetHistoryById(long requestId);

        EmployeeLeaveBalance GetLeaveBalance(GetLeaveBalanceCriteria criteria);

        void SendMailForInsertSuccessfully(InsertLeaveRequestCriteria criteria);

        void SendMailForUpdateSuccessfully(UpdateLeaveRequestCriteria criteria);

        void SendMailForDeleteSuccessfully(long requestId);

        DateTime GetFromTimeHalfDay(IList<LeaveDetailModel> leaveDetails);

        DateTime GetToTimeHalfDay(IList<LeaveDetailModel> leaveDetails);
    }
}