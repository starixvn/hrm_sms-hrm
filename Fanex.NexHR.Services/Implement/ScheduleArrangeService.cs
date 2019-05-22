namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
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

        public CommandResult EditWeeklyDefaultShift(int actorId, WeeklyDefaultShift weeklyDefaultShift, bool isUsedWeeklyDefaultShift)
        {
            var criteria = new EditWeeklyDefaultShiftCriteria
            {
                ActorId = actorId,
                EmployeeId = weeklyDefaultShift.EmployeeId,
                FromDate = DateTime.Now.Date,
                MonShiftId = GetShiftId(weeklyDefaultShift.MonShiftId),
                TueShiftId = GetShiftId(weeklyDefaultShift.TueShiftId),
                WedShiftId = GetShiftId(weeklyDefaultShift.WedShiftId),
                ThuShiftId = GetShiftId(weeklyDefaultShift.ThuShiftId),
                FriShiftId = GetShiftId(weeklyDefaultShift.FriShiftId),
                SatShiftId = GetShiftId(weeklyDefaultShift.SatShiftId),
                SunShiftId = GetShiftId(weeklyDefaultShift.SunShiftId),
                IsUsedWeeklyDefaultShift = isUsedWeeklyDefaultShift
            };

            return baseService.ExecuteNonQuery(criteria);
        }

        private static int? GetShiftId(int? shiftId)
        {
            return shiftId.HasValue && shiftId.Value != Constants.OffDayShiftId ? shiftId.Value : (int?)null;
        }

        private static void MappingDefaultShift(IEnumerable<WeeklySchedule> weeklySchedules)
        {
            foreach (WeeklySchedule weeklySchedule in weeklySchedules)
            {
                WeeklyDefaultShift weeklyDefaultShift = weeklySchedule.WeeklyDefaultShift;
                EntityBase<int> defaultShift = weeklySchedule.DefaultShift;

                MappingDailyDefaultShift(
                    weeklySchedule.MondaySchedule,
                    weeklySchedule.IsUsedWeeklyDefaultShift ? GetDefaultShift(weeklyDefaultShift, weeklySchedule.MondaySchedule.Weekday) : defaultShift);
                MappingDailyDefaultShift(weeklySchedule.TuesdaySchedule,
                    weeklySchedule.IsUsedWeeklyDefaultShift ? GetDefaultShift(weeklyDefaultShift, weeklySchedule.TuesdaySchedule.Weekday) : defaultShift);
                MappingDailyDefaultShift(weeklySchedule.WednesdaySchedule,
                    weeklySchedule.IsUsedWeeklyDefaultShift ? GetDefaultShift(weeklyDefaultShift, weeklySchedule.WednesdaySchedule.Weekday) : defaultShift);
                MappingDailyDefaultShift(weeklySchedule.ThursdaySchedule,
                    weeklySchedule.IsUsedWeeklyDefaultShift ? GetDefaultShift(weeklyDefaultShift, weeklySchedule.ThursdaySchedule.Weekday) : defaultShift);
                MappingDailyDefaultShift(weeklySchedule.FridaySchedule,
                    weeklySchedule.IsUsedWeeklyDefaultShift ? GetDefaultShift(weeklyDefaultShift, weeklySchedule.FridaySchedule.Weekday) : defaultShift);
                MappingDailyDefaultShift(weeklySchedule.SaturdaySchedule,
                    weeklySchedule.IsUsedWeeklyDefaultShift ? GetDefaultShift(weeklyDefaultShift, weeklySchedule.SaturdaySchedule.Weekday) : defaultShift);
                MappingDailyDefaultShift(weeklySchedule.SundaySchedule,
                    weeklySchedule.IsUsedWeeklyDefaultShift ? GetDefaultShift(weeklyDefaultShift, weeklySchedule.SundaySchedule.Weekday) : defaultShift);
            }
        }

        private static EntityBase<int> GetDefaultShift(WeeklyDefaultShift weeklyDefaultShift, int weekday)
        {
            EntityBase<int> shift = null;
            if (weeklyDefaultShift != null)
            {
                switch (weekday - 1)
                {
                    case (int)DayOfWeek.Monday:
                        shift = new EntityBase<int> { Id = weeklyDefaultShift.MonShiftId, Code = weeklyDefaultShift.MonShiftCode };
                        break;

                    case (int)DayOfWeek.Tuesday:
                        shift = new EntityBase<int> { Id = weeklyDefaultShift.TueShiftId, Code = weeklyDefaultShift.TueShiftCode };
                        break;

                    case (int)DayOfWeek.Wednesday:
                        shift = new EntityBase<int> { Id = weeklyDefaultShift.WedShiftId, Code = weeklyDefaultShift.WedShiftCode };
                        break;

                    case (int)DayOfWeek.Thursday:
                        shift = new EntityBase<int> { Id = weeklyDefaultShift.ThuShiftId, Code = weeklyDefaultShift.ThuShiftCode };
                        break;

                    case (int)DayOfWeek.Friday:
                        shift = new EntityBase<int> { Id = weeklyDefaultShift.FriShiftId, Code = weeklyDefaultShift.FriShiftCode };
                        break;

                    case (int)DayOfWeek.Saturday:
                        shift = new EntityBase<int> { Id = weeklyDefaultShift.SatShiftId, Code = weeklyDefaultShift.SatShiftCode };
                        break;

                    case (int)DayOfWeek.Sunday:
                        shift = new EntityBase<int> { Id = weeklyDefaultShift.SunShiftId, Code = weeklyDefaultShift.SunShiftCode };
                        break;

                    default:
                        // nothing
                        break;
                }
            }

            return shift;
        }

        private static void MappingDailyDefaultShift(DailySchedule dailySchedule, EntityBase<int> defaultShift)
        {
            if (!(dailySchedule.IsHoliday || dailySchedule.IsOffDay || dailySchedule.IsKeepInViewDay || dailySchedule.IsChangeShift))
            {
                MappingDefaultShift(dailySchedule, defaultShift);
            }
        }

        private static void MappingDefaultShift(DailySchedule dailySchedule, EntityBase<int> defaultShift)
        {
            if (dailySchedule.Shift == null && defaultShift != null)
            {
                if (defaultShift.Id == Constants.OffDayShiftId)
                {
                    dailySchedule.IsOffDay = true;
                }
                else
                {
                    dailySchedule.Shift = defaultShift;
                }
            }
        }
    }
}