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
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// WorkingCalendar Repository.
    /// </summary>
    public class WorkingCalendarRepository : IWorkingCalendarRepository
    {
        /// <summary>
        /// Function to get WorkingCalendars By Date Range, groups and employee.
        /// </summary>
        /// <param name="fromDate">From Date.</param>
        /// <param name="toDate">The To Date.</param>
        /// <param name="groupIds">Groups Id.</param>
        /// <param name="employeeId">Employee Id.</param>
        /// <returns>List WorkingCalendarModel.</returns>
        public IList<WorkingCalendarModel> GetWorkingCalendarsByDateRange(DateTime fromDate, DateTime toDate, string groupIds, int? employeeId = null)
        {
            using (var db = new ObjectDb("EmployeeCalendar_GetByDateRange"))
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
                        parameterValues: new { fromDate = fromDate, toDate = toDate, groupIds = groupIds, empId = employeeId });

                return workingCalendarList.ToList();
            }
        }

        /// <summary>
        /// Function to get WorkingCalendars By Date Range, groups and employee.
        /// </summary>
        /// <param name="supervisorId">Supervisor Id.</param>
        /// <param name="fromDate">From Date.</param>
        /// <param name="toDate">The To Date.</param>
        /// <param name="groupIds">Groups Id.</param>
        /// <param name="jobTitles">Job titles.</param>
        /// <param name="employeeId">Employee Id.</param>
        /// <returns>List WorkingCalendarModel.</returns>
        public IList<WorkingCalendarModel> GetWorkingCalendarsForShiftArrange(
            int supervisorId, 
            DateTime fromDate, 
            DateTime toDate, 
            string groupIds,
            string jobTitles, 
            int? employeeId = null)
        {
            using (var db = new ObjectDb("ShiftMng_GetShiftSchedule"))
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
                    parameterValues: new { supervisorId = supervisorId, fromtime = fromDate, totime = toDate, groupIds = groupIds, JobTitleId = jobTitles, empId = employeeId });

                return workingCalendarList.ToList();
            }
        }

        /// <summary>
        /// Function to insert or update working calendars.
        /// </summary>
        /// <param name="scheduleArrangeModel">Schedule Arrange Model.</param>
        /// <param name="errorCode">Error Code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Result of insert/update list working calendars.</returns>
        public bool InsertOrUpdateWorkingCalendars(ScheduleArrangeModel scheduleArrangeModel, out int errorCode, out string message)
        {
            using (var db = new ObjectDb("ShiftMng_Set_ShiftSchedule"))
            {
                db.AddInParameter("@ModifiedDate", DbType.DateTime, scheduleArrangeModel.ModifiedDate);
                db.AddInParameter("@ModifiedBy", DbType.Int32, scheduleArrangeModel.ModifiedBy);
                db.AddInParameter("@ScheduleList", DbType.String, scheduleArrangeModel.ScheduleModelXML);
                db.AddInParameter("@ShiftMultipleList", DbType.String, scheduleArrangeModel.ShiftMultipleXML);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                var result = db.ExecuteNonQuery() != 0;

                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                return result;
            }
        }

        public IList<ShiftStatisticModel> GetShiftStatistic(int supervisorId, DateTime fromDate, DateTime toDate, string groupIds, bool isShowWholeCompany)
        {
            using (var db = new ObjectDb("ShiftMng_RptShiftSchedule"))
            {
                var shiftStatisticModels = db.Query<ShiftStatisticModel>(
                    new { supervisorId, fromtime = fromDate, totime = toDate, groupIds, isShowWholeCompany });

                return shiftStatisticModels.ToList();
            }
        }

        /// <summary>
        /// Method get working schedule.
        /// </summary>
        /// <param name="date">Working schedule's date time.</param>
        /// <param name="id">Employee's id.</param>
        /// <returns>A list of working schedule model.</returns>
        public IList<WorkingScheduleModel> GetWorkingSchedule(DateTime date, int id)
        {
            using (var db = new ObjectDb("Report_EmployeeCalendarByMonth"))
            {
                var ds = new DataSet();

                db.AddInParameter("@reportDate", DbType.DateTime, date);
                db.AddInParameter("@empId", DbType.Int32, id);
                ds = db.ExecuteDataSet();
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
        /// <returns>IList WorkingCalendarsViewModel.</returns>
        public IList<WorkingCalendarsViewModel> GetWorkingCalendars(int? empId, DateTime fromDate, DateTime toDate, string groupId, string jobTitleId)
        {
            using (var db = new ObjectDb("Report_EmployeeCalendarByWeekly"))
            {
                var list = new List<WorkingCalendarsViewModel>();
                var ds = new DataSet();

                db.AddInParameter("@empId", DbType.Int32, null);
                db.AddInParameter("@fromtime", DbType.DateTime, fromDate);
                db.AddInParameter("@totime", DbType.DateTime, toDate);
                db.AddInParameter("@groupids", DbType.String, groupId);
                db.AddInParameter("@JobTitleId", DbType.String, jobTitleId);
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
