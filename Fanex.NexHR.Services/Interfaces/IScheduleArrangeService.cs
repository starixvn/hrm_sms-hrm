namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models.ScheduleArrange;

    public interface IScheduleArrangeService
    {
        IEnumerable<WeeklySchedule> GetWeeklySchedule(WeeklyScheduleFilter filter);

        CommandResult EditWeekly(int actorId, IEnumerable<EditDailySchedule> weeklySchedules, IEnumerable<EditDefaultShift> defaultShifts);

        CommandResult EditRemark(int actorId, int employeeId, string remark);

        CommandResult EditDisplayOrder(int actorId, IEnumerable<EditDisplayOrder> displayOrders);
    }
}