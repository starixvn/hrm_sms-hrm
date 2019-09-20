namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.ScheduleArrange;

    public interface IChangeShiftRequestRepository
    {
        IEnumerable<DailySchedule> GetWorkingCalendarForRequest(ICriteria criteria);
    }

    public class ChangeShiftRequestRepository : IChangeShiftRequestRepository
    {
        public IEnumerable<DailySchedule> GetWorkingCalendarForRequest(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var dailySchedules = db.Query<DailySchedule, ShiftBase, DailySchedule>(
                    (dailySchedule, shift) =>
                    {
                        dailySchedule.Shift = shift;

                        return dailySchedule;
                    },
                    splitOn: "ShiftId",
                    parameterValues: criteria.GetSpParams());

                return dailySchedules;
            }
        }
    }
}