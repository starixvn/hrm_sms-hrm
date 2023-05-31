namespace GO.Website.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GO.Core.Services;
    using GO.Website.Criteria.UnitHead;
    using GO.Website.Models.UnitHead;

    public class UnitHeadService : IUnitHeadService
    {
        private readonly IBaseService baseService;

        public UnitHeadService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<UnitHeadWeeklySchedule>> GetWeeklyScheduleAsync(GetUnitHeadWeeklyScheduleCriteria criteria)
        {
            (var infos, var schedules) = await baseService.FetchMultipleAsync<UnitHeadWeeklySchedule, UnitHeadDailySchedule>(criteria);
            MappingData(infos, schedules);

            return infos.ToList();
        }

        private static void MappingData(
            IEnumerable<UnitHeadWeeklySchedule> infos,
            IEnumerable<UnitHeadDailySchedule> schedules)
        {
            foreach (UnitHeadWeeklySchedule info in infos)
            {
                IEnumerable<UnitHeadDailySchedule> dailySchedules = schedules.Where(x => x.EmpId == info.EmpId).ToList();
                foreach (UnitHeadDailySchedule dailySchedule in dailySchedules)
                {
                    switch (dailySchedule.WorkDate.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            info.MondaySchedule = dailySchedule;
                            break;

                        case DayOfWeek.Tuesday:
                            info.TuesdaySchedule = dailySchedule;
                            break;

                        case DayOfWeek.Wednesday:
                            info.WednesdaySchedule = dailySchedule;
                            break;

                        case DayOfWeek.Thursday:
                            info.ThursdaySchedule = dailySchedule;
                            break;

                        case DayOfWeek.Friday:
                            info.FridaySchedule = dailySchedule;
                            break;

                        case DayOfWeek.Saturday:
                            info.SaturdaySchedule = dailySchedule;
                            break;

                        case DayOfWeek.Sunday:
                            info.SundaySchedule = dailySchedule;
                            break;

                        default:
                            // nothing
                            break;
                    }
                }
            }
        }
    }
}