namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Services;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Criteria.LeaveType;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;

    public class LeaveTypeService : ILeaveTypeService
    {
        private readonly IBaseService baseService;

        public LeaveTypeService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IList<LeaveType> GetAll(GetLeaveTypeCriteria criteria)
        {
            IList<LeaveType> leaveTypes = baseService.GetData<LeaveType>(criteria).ToList();
            leaveTypes.Insert(0, new LeaveType { Id = Constants.GeneralLeave, Name = Labels.GeneralLeave });

            return leaveTypes;
        }
    }
}