namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Request;

    public interface ILeaveRequestRepository
    {
        IList<LeaveRequest> GetByDate(ICriteria criteria);

        LeaveRequest GetById(ICriteria criteria);

        IList<LeaveRequest> GetHistoryById(ICriteria criteria);

        IEnumerable<Models.ScheduleArrange.DailySchedule> GetEmployeeCalendar(ICriteria criteria);
    }
}