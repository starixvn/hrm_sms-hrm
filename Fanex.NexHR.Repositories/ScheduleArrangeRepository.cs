namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.ScheduleArrange;

    public class ScheduleArrangeRepository : IScheduleArrangeRepository
    {
        public IEnumerable<WeeklySchedule> GetWeeklySchedule(ICriteria criteria)
        {
            using (IObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<WeeklySchedule> weeklySchedules = multi.Read<WeeklySchedule, EntityBase<int>, EntityBase<int>, WeeklySchedule>(
                        (weeklySchedule, team, defaultShift) =>
                        {
                            weeklySchedule.Team = team;
                            weeklySchedule.DefaultShift = defaultShift;

                            return weeklySchedule;
                        },
                        splitOn: "GroupId, DefaultShiftId");

                    IEnumerable<DailySchedule> dailySchedules = multi.Read<DailySchedule, EntityBase<int>, EntityBase<int>, EntityBase<int>, DailySchedule>(
                        (dailySchedule, shift, leave, leave2) =>
                        {
                            dailySchedule.Shift = shift;
                            dailySchedule.LeaveType = leave;
                            dailySchedule.LeaveType2 = leave2;

                            return dailySchedule;
                        },
                        splitOn: "ShiftId, LeaveTypeId, LeaveTypeId2");

                    IEnumerable<WeeklyDefaultShift> weeklyDefaultShifts = multi.Read<WeeklyDefaultShift>();

                    MappingData(weeklySchedules, dailySchedules, weeklyDefaultShifts);

                    return weeklySchedules;
                }
            }
        }

        private static void MappingData(
            IEnumerable<WeeklySchedule> weeklySchedules,
            IEnumerable<DailySchedule> dailySchedules,
            IEnumerable<WeeklyDefaultShift> weeklyDefaultShifts)
        {
            foreach (WeeklySchedule weeklySchedule in weeklySchedules)
            {
                weeklySchedule.WeeklyDefaultShift = weeklyDefaultShifts.FirstOrDefault(x => x.EmployeeId == weeklySchedule.EmployeeId);
                IEnumerable<DailySchedule> memberDailySchedules = dailySchedules.Where(x => x.EmployeeId == weeklySchedule.EmployeeId);

                foreach (DailySchedule dailySchedule in memberDailySchedules)
                {
                    switch (dailySchedule.Weekday - 1)
                    {
                        case (int)DayOfWeek.Monday:
                            weeklySchedule.MondaySchedule = dailySchedule;
                            break;

                        case (int)DayOfWeek.Tuesday:
                            weeklySchedule.TuesdaySchedule = dailySchedule;
                            break;

                        case (int)DayOfWeek.Wednesday:
                            weeklySchedule.WednesdaySchedule = dailySchedule;
                            break;

                        case (int)DayOfWeek.Thursday:
                            weeklySchedule.ThursdaySchedule = dailySchedule;
                            break;

                        case (int)DayOfWeek.Friday:
                            weeklySchedule.FridaySchedule = dailySchedule;
                            break;

                        case (int)DayOfWeek.Saturday:
                            weeklySchedule.SaturdaySchedule = dailySchedule;
                            break;

                        case (int)DayOfWeek.Sunday:
                            weeklySchedule.SundaySchedule = dailySchedule;
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