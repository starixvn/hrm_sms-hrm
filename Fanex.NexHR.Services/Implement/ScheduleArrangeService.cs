namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Core.XMLExtensions;
    using Fanex.NexHR.Criteria.ScheduleArrange;
    using Fanex.NexHR.Models;
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

        public IEnumerable<EntityBase<int>> GetGroups(int actorId)
        {
            var criteria = new GetGroupCriteria
            {
                ActorId = actorId
            };

            return baseService.GetData<EntityBase<int>>(criteria);
        }

        public IEnumerable<EmployeeSuggestion> GetEmployees(int actorId, string groupIds, string query)
        {
            var criteria = new GetEmployeeCriteria
            {
                ActorId = actorId,
                GroupIds = groupIds,
                Query = query
            };

            return baseService.GetData<EmployeeSuggestion>(criteria);
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

            IEnumerable<WeeklySchedule> weeklySchedules = scheduleArrangeRepository.GetWeeklySchedule(criteria);
            MappingDefaultShift(weeklySchedules);

            return weeklySchedules;
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

        private static void MappingDefaultShift(IEnumerable<WeeklySchedule> weeklySchedules)
        {
            foreach (WeeklySchedule weeklySchedule in weeklySchedules)
            {
                MappingDailyDefaultShift(weeklySchedule.MondaySchedule, weeklySchedule.DefaultShift);
                MappingDailyDefaultShift(weeklySchedule.TuesdaySchedule, weeklySchedule.DefaultShift);
                MappingDailyDefaultShift(weeklySchedule.WednesdaySchedule, weeklySchedule.DefaultShift);
                MappingDailyDefaultShift(weeklySchedule.ThursdaySchedule, weeklySchedule.DefaultShift);
                MappingDailyDefaultShift(weeklySchedule.FridaySchedule, weeklySchedule.DefaultShift);
                MappingDailyDefaultShift(weeklySchedule.SaturdaySchedule, weeklySchedule.DefaultShift);
                MappingDailyDefaultShift(weeklySchedule.SundaySchedule, weeklySchedule.DefaultShift);
            }
        }

        private static void MappingDailyDefaultShift(DailySchedule dailySchedule, EntityBase<int> defaultShift)
        {
            if (!(dailySchedule.IsHoliday || dailySchedule.IsOffDay || dailySchedule.IsKeepInViewDay || dailySchedule.IsChangeShift))
            {
                if (dailySchedule.IsLeaveDay)
                {
                    const decimal HalfDay = 0.5M;
                    if (dailySchedule.LeaveDay == HalfDay)
                    {
                        dailySchedule.Shift = dailySchedule.Shift ?? defaultShift;
                    }
                }
                else
                {
                    dailySchedule.Shift = dailySchedule.Shift ?? defaultShift;
                }
            }
        }
    }
}