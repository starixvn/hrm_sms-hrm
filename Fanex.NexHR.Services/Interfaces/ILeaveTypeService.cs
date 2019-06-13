namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public interface ILeaveTypeService
    {
        IList<LeaveType> GetAll(bool excludeNotShown = true);
    }
}