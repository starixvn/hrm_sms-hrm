namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.ScheduleArrange;

    public interface IScheduleArrangeService
    {
        IEnumerable<EntityBase<int>> GetGroups(int actorId);

        IEnumerable<EmployeeSuggestion> GetEmployees(int actorId, string groupIds, string query);

        IEnumerable<WeeklySchedule> GetWeeklySchedule(WeeklyScheduleFilter filter);

        CommandResult EditWeekly(
            int actorId,
            DateTime fromDate,
            DateTime toDate,
            IEnumerable<EditDailySchedule> weeklySchedules,
            IEnumerable<EditDefaultShift> defaultShifts);

        CommandResult EditRemark(int actorId, int employeeId, string remark);

        CommandResult EditDisplayOrder(int actorId, IEnumerable<EditDisplayOrder> displayOrders);

        CommandResult EditGroupOrder(int actorId, IEnumerable<EditGroupOrder> displayOrders);

        CommandResult EditWeeklyDefaultShift(int actorId, WeeklyDefaultShift weeklyDefaultShift, bool isUsedWeeklyDefaultShift);
    }
}