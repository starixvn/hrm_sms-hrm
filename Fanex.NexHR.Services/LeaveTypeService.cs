namespace Fanex.NexHR.Services
{
    using Core.Services;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.LeaveType;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class LeaveTypeService : BaseService, ILeaveTypeService
    {
        public IList<LeaveType> GetAll(GetLeaveTypeCriteria criteria)
        {
            IList<LeaveType> leaveTypes = GetData<LeaveType>(criteria).ToList();
            leaveTypes.Insert(0, new LeaveType { Id = Constants.GeneralLeave, Name = Labels.GeneralLeave });
            return leaveTypes;
        }
    }
}
