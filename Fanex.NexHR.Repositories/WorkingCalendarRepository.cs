namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Models.ScheduleArrange;

    public class WorkingCalendarRepository : IWorkingCalendarRepository
    {
        public IEnumerable<DailySchedule> GetWorkingSchedule(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<DailySchedule, ShiftBase, LeaveRequest, LeaveRequest, DailySchedule>(
                    (dailySchedule, shift, leave, leave2) =>
                    {
                        dailySchedule.Shift = shift;
                        dailySchedule.LeaveType = leave;
                        dailySchedule.LeaveType2 = leave2;

                        return dailySchedule;
                    },
                    splitOn: "ShiftId, LeaveTypeId, LeaveTypeId2",
                    parameterValues: criteria.GetSpParams());
            }
        }

        public IEnumerable<WorkingCalendar> GetWorkingCalendars(ICriteria criteria)
        {
            using (IObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<WorkingCalendar> calendars = multi.Read<WorkingCalendar>();

                    IEnumerable<DailySchedule> dailySchedules = multi.Read<DailySchedule, ShiftBase, LeaveRequest, LeaveRequest, DailySchedule>(
                        (dailySchedule, shift, leave, leave2) =>
                        {
                            dailySchedule.Shift = shift;
                            dailySchedule.LeaveType = leave;
                            dailySchedule.LeaveType2 = leave2;

                            return dailySchedule;
                        },
                        splitOn: "ShiftId, LeaveTypeId, LeaveTypeId2");

                    MappingData(calendars, dailySchedules);

                    return calendars;
                }
            }
        }

        private static void MappingData(
            IEnumerable<WorkingCalendar> calendars,
            IEnumerable<DailySchedule> dailySchedules)
        {
            foreach (WorkingCalendar calendar in calendars)
            {
                IEnumerable<DailySchedule> memberDailySchedules = dailySchedules.Where(x => x.EmployeeId == calendar.EmployeeId);
                foreach (DailySchedule dailySchedule in memberDailySchedules)
                {
                    switch (dailySchedule.WorkDate.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            calendar.MondaySchedule = dailySchedule;
                            break;

                        case DayOfWeek.Tuesday:
                            calendar.TuesdaySchedule = dailySchedule;
                            break;

                        case DayOfWeek.Wednesday:
                            calendar.WednesdaySchedule = dailySchedule;
                            break;

                        case DayOfWeek.Thursday:
                            calendar.ThursdaySchedule = dailySchedule;
                            break;

                        case DayOfWeek.Friday:
                            calendar.FridaySchedule = dailySchedule;
                            break;

                        case DayOfWeek.Saturday:
                            calendar.SaturdaySchedule = dailySchedule;
                            break;

                        case DayOfWeek.Sunday:
                            calendar.SundaySchedule = dailySchedule;
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