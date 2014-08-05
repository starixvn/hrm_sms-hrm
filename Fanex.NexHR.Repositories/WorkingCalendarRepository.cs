// --------------------------------------------------------------------------
// <copyright file="WorkingCalendarRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.18">Created</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// WorkingCalendar Repository.
    /// </summary>
    public class WorkingCalendarRepository : IWorkingCalendarRepository
    {
        /// <summary>
        /// Function to get working calendars by date range.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List working calendars.</returns>
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

        /// <summary>
        /// Function to get WorkingCalendars By Date Range, groups and employee.
        /// </summary>
        /// <param name="criteria">Criteria to query DB..</param>
        /// <returns>List WorkingCalendarModel.</returns>
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

        /// <summary>
        /// Method get working schedule.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <param name="date">Report Date.</param>
        /// <returns>A list of working schedule model.</returns>
        public IList<WorkingScheduleModel> GetWorkingSchedule(ICriteria criteria, DateTime date)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var ds = new DataSet();
                ds = db.ExecuteDataSet(criteria.GetSpParams());
                var dttWorkingSchedule = ds != null ? ds.Tables[0] : null;

                var list = new List<WorkingScheduleModel>();
                if (dttWorkingSchedule.Rows.Count != 0)
                {
                    var loopNum = (dttWorkingSchedule.Columns.Count - 5) / 2;
                    for (int i = 1; i <= loopNum; i++)
                    {
                        var workingDay = new WorkingScheduleModel
                        {
                            Title = dttWorkingSchedule.Rows[0][i == 1 ? (i + 4) : ((i * 2) + 3)].ToString(),
                            Start = new DateTime(date.Year, date.Month, i).ToShortDateString(),
                            Color = dttWorkingSchedule.Rows[0][i == 1 ? (i + 5) : ((i * 2) + 4)].ToString(),
                        };
                        list.Add(workingDay);
                    }
                }

                return list;
            }
        }

        /// <summary>
        /// Gets the working calendars.
        /// </summary>
        /// <param name="empId">The employee identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">Query To date.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="jobTitleId">The job title identifier.</param>
        /// <param name="isLocal">Expat or local.</param>
        /// <returns>IList WorkingCalendarsViewModel.</returns>
        public IList<WorkingCalendarsViewModel> GetWorkingCalendars(
            int? empId, 
            DateTime fromDate, 
            DateTime toDate,
            string groupId, 
            string jobTitleId,
            byte? isLocal = null)
        {
            using (var db = new ObjectDb("Report_EmployeeCalendarByWeekly"))
            {
                var list = new List<WorkingCalendarsViewModel>();
                var ds = new DataSet();

                db.AddInParameter("@empId", DbType.Int32, empId);
                db.AddInParameter("@fromtime", DbType.DateTime, fromDate);
                db.AddInParameter("@totime", DbType.DateTime, toDate);
                db.AddInParameter("@groupids", DbType.String, groupId);
                db.AddInParameter("@JobTitleId", DbType.String, jobTitleId);
                db.AddInParameter("@isLocal", DbType.Byte, isLocal);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                ds = db.ExecuteDataSet();
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
                    List<WorkingScheduleModel> workingSchedule = new List<WorkingScheduleModel>();
                    for (int i = 1; i <= empWorkingCalendar.Days; i++)
                    {
                        var workingDay = new WorkingScheduleModel
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
    }
}
