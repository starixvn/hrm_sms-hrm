namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.ScheduleArrange;
    using Fanex.NexHR.ViewModels;

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

        public IList<WorkingCalendarsViewModel> GetWorkingCalendars(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var list = new List<WorkingCalendarsViewModel>();

                using (var ds = db.ExecuteDataSet(criteria.GetSpParams()))
                {
                    var dttWorkingCalendars = ds?.Tables[0];
                    var totalColumns = dttWorkingCalendars != null ? dttWorkingCalendars.Columns.Count : 0;

                    foreach (DataRow row in dttWorkingCalendars?.Rows)
                    {
                        var empWorkingCalendar = new WorkingCalendarsViewModel
                        {
                            EmpId = int.Parse(row["Empid"].ToString()),
                            FullName = row["fullname"].ToString(),
                            NickName = row["nickname"].ToString(),
                            Team = row["Team"].ToString(),
                            Positions = row["Positions"].ToString(),
                            EmpCode = row["EmpCode"].ToString(),
                            Days = (totalColumns - 6) / 2
                        };
                        var days = new List<string>();
                        var workingSchedule = new List<WorkingScheduleViewModel>();
                        var defaultColumn = 6;
                        for (int i = 1; i <= empWorkingCalendar.Days; i++)
                        {
                            var workingDay = new WorkingScheduleViewModel
                            {
                                Title = row[defaultColumn++].ToString(),
                                Color = row[defaultColumn++].ToString()
                            };
                            days.Add(workingDay.Title);
                            workingSchedule.Add(workingDay);
                        }

                        empWorkingCalendar.WorkingDays = days;
                        empWorkingCalendar.WorkingSchedule = workingSchedule;
                        list.Add(empWorkingCalendar);
                    }

                    return list;
                }
            }
        }

        public IList<EmployeeShiftModel> GetEmployeeShift(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var employeeShifts = db.Query<EmployeeShiftModel, Employee, Team, Shift, EmployeeShiftModel>(
                    (es, e, t, sh) =>
                    {
                        e.Team = t;
                        es.Employee = e;
                        es.Shift = sh;
                        return es;
                    },
                    splitOn: "EmpId, GroupId, ShiftId",
                    parameterValues: criteria.GetSpParams()).ToList();

                return employeeShifts;
            }
        }
    }
}