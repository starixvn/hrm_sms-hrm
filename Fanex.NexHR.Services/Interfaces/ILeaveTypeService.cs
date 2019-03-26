namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.LeaveType;
    using Fanex.NexHR.Models;

    public interface ILeaveTypeService
    {
        IList<LeaveType> GetAll(GetLeaveTypeCriteria criteria);
    }
}