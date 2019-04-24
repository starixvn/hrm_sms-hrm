namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Core.XMLExtensions;
    using Fanex.NexHR.Criteria.ScheduleArrange;
    using Fanex.NexHR.Models.ScheduleArrange;
    using Fanex.NexHR.Repositories;

    public class ScheduleArrangeService : IScheduleArrangeService
    {
        private readonly IScheduleArrangeRepository scheduleArrangeRepository;
        private readonly IBaseService baseService;

        public ScheduleArrangeService(
            IScheduleArrangeRepository scheduleArrangeRepository,
            IBaseService baseService)
        {
            this.scheduleArrangeRepository = scheduleArrangeRepository;
            this.baseService = baseService;
        }

        public IEnumerable<WeeklySchedule> GetWeeklySchedule(WeeklyScheduleFilter filter)
        {
            var criteria = new GetWeeklyScheduleCriteria
            {
                ActorId = filter.ActorId,
                FromTime = filter.FromDate,
                ToTime = filter.ToDate,
                EmpId = filter.EmployeeId,
                GroupIds = filter.GroupIds
            };

            return scheduleArrangeRepository.GetWeeklySchedule(criteria);
        }

        public CommandResult EditWeekly(int actorId, IEnumerable<EditDailySchedule> weeklySchedules, IEnumerable<EditDefaultShift> defaultShifts)
        {
            var criteria = new EditWeeklyScheduleCriteria
            {
                ActorId = actorId,
                WeeklyScheduleXml = weeklySchedules.ToXML().ToString(),
                DefaultShiftXml = defaultShifts.ToXML().ToString()
            };

            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult EditRemark(int actorId, int employeeId, string remark)
        {
            var criteria = new EditRemarkCriteria
            {
                ActorId = actorId,
                EmployeeId = employeeId,
                Remark = remark
            };

            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult EditDisplayOrder(int actorId, IEnumerable<EditDisplayOrder> displayOrders)
        {
            var criteria = new EditDisplayOrderCriteria
            {
                ActorId = actorId,
                DisplayOrderXml = displayOrders.ToXML().ToString()
            };

            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult EditGroupOrder(int actorId, IEnumerable<EditGroupOrder> displayOrders)
        {
            var criteria = new EditGroupOrderCriteria
            {
                ActorId = actorId,
                DisplayOrderXml = displayOrders.ToXML().ToString()
            };

            return baseService.ExecuteNonQuery(criteria);
        }
    }
}