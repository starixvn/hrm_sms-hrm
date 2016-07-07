namespace Fanex.NexHR.Repositories
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.ViewModels.Calendar;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public class WorkingCalendarRepository : IWorkingCalendarRepository
    {
        public IList<WorkingCalendarModel> GetWorkingCalendarByDateRange(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var workingCalendarList = db.Query<WorkingCalendarModel, Employee, Shift, LeaveType, WorkingCalendarModel>(
                    (w, e, s, lv) =>
                    {
                        w.Employee = e;
                        w.Shift = s;
                        w.LeaveType = lv;
                        return w;
                    },
                    splitOn: "EmpId, ShiftId, LeaveTypeId",
                    parameterValues: criteria.GetSpParams());

                return workingCalendarList.ToList();
            }
        }

        public IList<WorkingCalendarModel> GetWorkingCalendarsForShiftArrange(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var workingCalendarList = db.Query<WorkingCalendarModel, Employee, Team, Shift, LeaveType, LeaveType, WorkingCalendarModel>(
                    (w, e, t, s, lv, lv2) =>
                    {
                        e.Team = t;
                        w.Employee = e;
                        w.Shift = s;
                        w.LeaveType = lv;
                        w.LeaveType2 = lv2;
                        return w;
                    },
                    splitOn: "EmpId, GroupId, ShiftId, LeaveTypeId, LeaveTypeId2",
                    parameterValues: criteria.GetSpParams());

                return workingCalendarList.ToList();
            }
        }

        public IList<WorkingScheduleViewModel> GetWorkingSchedule(ICriteria criteria, DateTime workingDate)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var ds = new DataSet();
                ds = db.ExecuteDataSet(criteria.GetSpParams());
                var dttWorkingSchedule = ds != null ? ds.Tables[0] : null;

                var list = new List<WorkingScheduleViewModel>();
                if (dttWorkingSchedule.Rows.Count != 0)
                {
                    var totalEmployeeInfoColumn = 5;
                    var loopNum = (dttWorkingSchedule.Columns.Count - totalEmployeeInfoColumn) / 2;
                    var index = totalEmployeeInfoColumn;

                    for (int i = 1; i <= loopNum; i++)
                    {
                        var workingDay = new WorkingScheduleViewModel
                        {
                            Title = dttWorkingSchedule.Rows[0][index++].ToString(),
                            Start = new DateTime(workingDate.Year, workingDate.Month, i).ToShortDateString(),
                            Color = dttWorkingSchedule.Rows[0][index++].ToString(),
                        };

                        list.Add(workingDay);
                    }
                }

                return list;
            }
        }

        public IList<WorkingCalendarsViewModel> GetWorkingCalendars(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var list = new List<WorkingCalendarsViewModel>();
                var ds = new DataSet();

                ds = db.ExecuteDataSet(criteria.GetSpParams());
                var dttWorkingCalendars = ds != null ? ds.Tables[0] : null;
                var totalColumns = dttWorkingCalendars.Columns.Count;

                foreach (System.Data.DataRow row in dttWorkingCalendars.Rows)
                {
                    var empWorkingCalendar = new WorkingCalendarsViewModel();
                    empWorkingCalendar.EmpId = int.Parse(row["Empid"].ToString());
                    empWorkingCalendar.FullName = row["fullname"].ToString();
                    empWorkingCalendar.NickName = row["nickname"].ToString();
                    empWorkingCalendar.Team = row["Team"].ToString();
                    empWorkingCalendar.Positions = row["Positions"].ToString();
                    empWorkingCalendar.EmpCode = row["EmpCode"].ToString();
                    empWorkingCalendar.Days = (totalColumns - 6) / 2;
                    List<string> days = new List<string>();
                    List<WorkingScheduleViewModel> workingSchedule = new List<WorkingScheduleViewModel>();
                    for (int i = 1; i <= empWorkingCalendar.Days; i++)
                    {
                        var workingDay = new WorkingScheduleViewModel
                        {
                            Title = row[i == 1 ? (i + 5) : ((i * 2) + 4)].ToString(),
                            Color = row[i == 1 ? (i + 6) : ((i * 2) + 5)].ToString()
                        };
                        days.Add(row[i == 1 ? (i + 5) : ((i * 2) + 4)].ToString());
                        workingSchedule.Add(workingDay);
                    }

                    empWorkingCalendar.WorkingDays = days;
                    empWorkingCalendar.WorkingSchedule = workingSchedule;
                    list.Add(empWorkingCalendar);
                }

                return list;
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
