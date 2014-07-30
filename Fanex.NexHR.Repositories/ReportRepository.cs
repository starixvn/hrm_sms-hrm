// --------------------------------------------------------------------------
// <copyright file="ReportRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Report Repository.
    /// </summary>
    public class ReportRepository : IReportRepository
    {
        /// <summary>
        /// Method get Employee Attendance report.
        /// </summary>
        /// <param name="fromTime">Get from time.</param>
        /// <param name="toTime">Get to time.</param>
        /// <param name="type">Type to get.</param>
        /// <param name="empId">Employee's id.</param>
        /// <param name="groupIds">Group's id.</param>
        /// <param name="excludeDeletedUsers">Exclude deleted users.</param>
        /// <param name="isLocal">Expat or Local.</param>
        /// <returns>List of EmployeeAttendanceModel.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(
            DateTime fromTime,
            DateTime toTime,
            short type,
            int? empId = null,
            string groupIds = null,
            int? excludeDeletedUsers = 1,
            byte? isLocal = null)
        {
            var listAttendance = new List<EmployeeAttendanceModel>();
            EmployeeAttendanceModel currentAttendance = new EmployeeAttendanceModel();
            EmployeeAttendanceRequest request = new EmployeeAttendanceRequest();

            using (ObjectDb objectDb = new ObjectDb("EmployeeAttendance_GetByType"))
            {
                objectDb.Query<EmployeeAttendanceModel, dynamic, EmployeeAttendanceModel>(
                    (attendance, t) =>
                    {
                        if (attendance.EmpId != currentAttendance.EmpId
                            || (attendance.EmpId == currentAttendance.EmpId && attendance.AttendanceDate != currentAttendance.AttendanceDate))
                        {
                            listAttendance.Add(attendance);
                            currentAttendance = attendance;
                            currentAttendance.RequestList = new List<EmployeeAttendanceRequest>();
                        }

                        if (t != null && t.RequestType != null)
                        {
                            request = new EmployeeAttendanceRequest
                            {
                                RequestType = new RequestTypeModel { Id = Convert.ToInt16(t.RequestType) },
                                FromDate = t.FromDate,
                                ToDate = t.ToDate,
                                FromTime = t.FromTime,
                                ToTime = t.ToTime,
                                CreatedDate = t.CreatedDate,
                                Remark = t.Remark,
                                TotalDays = Convert.ToDecimal(t.TotalDays),
                                LeaveType = new LeaveType
                                {
                                    Id = t.LeaveTypeId == null ? 0 : t.LeaveTypeId,
                                    Name = t.LeaveType
                                },
                                FromShift = t.FromShift,
                                ToShift = t.ToShift,
                                TotalMins = t.Time
                            };
                            currentAttendance.RequestList.Add(request);
                        }

                        return currentAttendance;
                    },
                    "RequestType",
                    new { fromTime, toTime, type, empId, groupIds, excludeUserDelete = excludeDeletedUsers, isLocal });
            }

            return listAttendance;
        }

        /// <summary>
        ///  Method Get Request Monitoring.
        /// </summary>
        /// <param name="fromDate">Query from date.</param>
        /// <param name="toDate">Query to date.</param>
        /// <param name="maxRequest">Max request.</param>
        /// <param name="role">Current user role.</param>
        /// <param name="employeeId">Employee id.</param>
        /// <param name="loginUserId">Current user id.</param>
        /// <param name="groupIds">Filter groups.</param>
        /// <param name="isLocal">Expat or local.</param>
        /// <returns>List of Request Monitoring Entity.</returns>
        public IList<RequestsMonitoring> GetRequestMonitoring(
            DateTime fromDate,
            DateTime toDate,
            int maxRequest,
            int role,
            int? employeeId,
            int loginUserId,
            string groupIds,
            byte? isLocal = null)
        {
            using (var db = new ObjectDb("RequestMonitoring"))
            {
                var entityList = new List<RequestsMonitoring>();

                using (IDataReader reader = db.ExecuteReader(new { fromDate, toDate, maxRequest, role, employeeId, loginUserId, groupIds, isLocal }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestsMonitoring
                            {
                                Requester = new Employee
                                {
                                    FullName = reader.ToString("FullName"),
                                    NickName = reader.ToString("NickName"),
                                    Team = new Team { Name = reader.ToString("Team"), Id = reader.ToInt("TeamId") },
                                    Id = reader.ToInt("EmpID")
                                },
                                TotalCheckInOut = reader.ToInt("TotalCheckInOutRequest"),
                                TotalLateIn = reader.ToInt("TotalLateInRequest"),
                                TotalEarlyOut = reader.ToInt("TotalEarlyOutRequest"),
                                TotalChangeShift = reader.ToInt("TotalChangeShiftRequest"),
                                TotalLeave = reader.ToInt("TotalLeaveRequest"),
                                TotalUnpaidLeave = reader.ToInt("TotalUnpaidLeaveRequest"),
                                TotalOvertime = reader.ToInt("TotalOvertimeRequest"),
                                KeepInViewRequest = reader.ToInt("KeepInViewRequest")
                            };

                            entityList.Add(entity);
                        }
                    }
                }

                return entityList;
            }
        }

        /// <summary>
        /// Method get working hours.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">Get to date.</param>
        /// <param name="empId">Employee's id.</param>
        /// <param name="groupIds">Group's id.</param>
        /// <param name="isLocal">Is expat or local.</param>
        /// <returns>List of TotalWorkingHours.</returns>
        public IList<TotalWorkingHours> GetWorkingHoursList(DateTime fromDate, DateTime toDate, int? empId, string groupIds, byte? isLocal = null)
        {
            using (var db = new ObjectDb("Report_WorkingHoursSummary"))
            {
                var entityList = new List<TotalWorkingHours>();

                using (IDataReader reader = db.ExecuteReader(new { fromDate, toDate, empId, groupIds, isLocal }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new TotalWorkingHours
                            {
                                Employee = new Employee
                                {
                                    Code = reader.ToString("Code"),
                                    FullName = reader.ToString("FullName"),
                                    NickName = reader.ToString("NickName"),
                                    Team = new Team { Name = reader.To<string>("Team") },
                                    Id = reader.ToInt("empid")
                                },
                                WorkingDay = reader.ToInt("WorkingDays"),
                                ActualWorkingDay = reader.ToFloat("ActualWorkingDays"),
                                LeaveTaken = reader.ToFloat("LeaveTaken"),
                                UnpaidLeave = reader.ToFloat("UnpaidLeave"),
                                LateInMins = reader.ToInt("LateIn"),
                                EarlyOutMins = reader.ToInt("EarlyOut"),
                                TotalHours = reader.ToFloat("TotalWorkHours"),
                                AverageHours = reader.ToFloat("AverageHours")
                            };

                            entityList.Add(entity);
                        }
                    }
                }

                return entityList;
            }
        }

        /// <summary>
        /// Gets the total working hour monthly.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <param name="empId">The employee identifier.</param>
        /// <param name="groupIds">The group ids.</param>
        /// <param name="isLocal">Expat or Local.</param>
        /// <returns>The list of TotalWorkingHourMonthly.</returns>
        public IList<TotalWorkingHourMonthly> GetTotalWorkingHourMonthly(
           DateTime month,
           int? empId,
           string groupIds,
           byte? isLocal = null)
        {
            using (var db = new ObjectDb("Report_EmployeeTotalWorkingHourByMonth"))
            {
                var entityList = new List<TotalWorkingHourMonthly>();

                using (IDataReader reader = db.ExecuteReader(new { reportMonth = month, empId, groupIds, isLocal }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new TotalWorkingHourMonthly
                            {
                                StaffId = Convert.ToString(reader.GetValue(0)),
                                Section = Convert.ToString(reader.GetValue(1)),
                                FullNameNick = Convert.ToString(reader.GetValue(2)),
                                Nick = Convert.ToString(reader.GetValue(3)),
                                CountWorkDay = Convert.ToDecimal(reader.GetValue(4)),
                            };

                            entity.WorkingHours = new List<string>();
                            for (int i = 5; i < reader.FieldCount; i++)
                            {
                                var workingHour = reader.GetValue(i);
                                entity.WorkingHours.Add(Convert.ToString(workingHour));
                            }

                            entityList.Add(entity);
                        }
                    }
                }

                return entityList;
            }
        }

        /// <summary>
        /// Get Total Working Hour Weekly.
        /// </summary>
        /// <param name="empId">Employee Id.</param>
        /// <param name="fromDate">From Date.</param>
        /// <param name="toDate">Query to date.</param>
        /// <param name="groupIds">Group Ids.</param>
        /// <param name="isLocal">Local or expat.</param>
        /// <returns>TotalWorkingHourMonthly Model.</returns>
        public IList<TotalWorkingHourMonthly> GetTotalWorkingHourWeekly(
            int? empId,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            byte? isLocal = null)
        {
            using (var db = new ObjectDb("Report_EmployeeTotalWorkingHourByWeek"))
            {
                var entityList = new List<TotalWorkingHourMonthly>();

                using (IDataReader reader = db.ExecuteReader(new { empId, fromDate, toDate, groupIds, isLocal }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new TotalWorkingHourMonthly
                            {
                                StaffId = Convert.ToString(reader.GetValue(0)),
                                Section = Convert.ToString(reader.GetValue(1)),
                                FullNameNick = Convert.ToString(reader.GetValue(2)),
                                Nick = Convert.ToString(reader.GetValue(3)),
                                CountWorkDay = Convert.ToDecimal(reader.GetValue(4)),
                            };

                            entity.WorkingHours = new List<string>();
                            for (int i = 5; i < reader.FieldCount; i++)
                            {
                                var workingHour = reader.GetValue(i);
                                entity.WorkingHours.Add(Convert.ToString(workingHour));
                            }

                            entityList.Add(entity);
                        }
                    }
                }

                return entityList;
            }
        }

        /// <summary>
        /// Method get annual leave balance.
        /// </summary>
        /// <param name="year">Year to get.</param>
        /// <param name="empId">Employee's id.</param>
        /// <param name="groupIds">Group's id.</param>
        /// <param name="isLocal">Employee Local or Expat.</param>
        /// <returns>List of LeaveSummary.</returns>
        public IList<LeaveSummary> GetAnnualLeaveBalanceReport(int year, int? empId, string groupIds, byte? isLocal)
        {
            using (var db = new ObjectDb("Report_LeaveSummary"))
            {
                var entityList = new List<LeaveSummary>();

                using (IDataReader reader = db.ExecuteReader(new { empId, groupIds, year, isLocal }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new LeaveSummary
                            {
                                Employee = new Employee
                                {
                                    FullName = reader.ToString("FullName"),
                                    NickName = reader.ToString("NickName"),
                                    Team = new Team { Name = reader.To<string>("Team") },
                                    Id = reader.ToInt("empid"),
                                    JoinedDate = reader.ToDateTime("JoinedDate"),
                                    ConfirmedDate = reader.ToDateTime("ConfirmedDate")
                                },
                                AnnualLeaveLastYear = reader.ToFloat("AnnualLeavelastYear"),
                                AnnualLeaveCurrentYear = reader.ToFloat("AnnualLeaveCurrentYear"),
                                Jan = reader.ToFloat("Jan"),
                                Feb = reader.ToFloat("Feb"),
                                Mar = reader.ToFloat("Mar"),
                                Apr = reader.ToFloat("Apr"),
                                May = reader.ToFloat("May"),
                                Jun = reader.ToFloat("Jun"),
                                Jul = reader.ToFloat("Jul"),
                                Aug = reader.ToFloat("Aug"),
                                Sep = reader.ToFloat("Sep"),
                                Oct = reader.ToFloat("Oct"),
                                Nov = reader.ToFloat("Nov"),
                                Dec = reader.ToFloat("Dec"),
                                LeaveBalance = reader.ToFloat("LeaveBalance"),
                                Remark = reader.ToString("Remark")
                            };

                            entityList.Add(entity);
                        }
                    }
                }

                return entityList;
            }
        }

        /// <summary>
        /// Function to get time records.
        /// </summary>
        /// <param name="employeeId">Employee Id.</param>
        /// <param name="groupIds">Group Ids.</param>
        /// <param name="fromDate">From Date.</param>
        /// <param name="toDate">The To Date.</param>
        /// <param name="isLocal">Expat or Local.</param>
        /// <returns>List of time records.</returns>
        public IList<TimeRecordModel> GetTimeRecords(int? employeeId, string groupIds, DateTime fromDate, DateTime toDate, byte? isLocal)
        {
            IList<TimeRecordModel> timeRecords = null;

            using (var db = new ObjectDb("TimeRecord_Get"))
            {
                timeRecords = db.Query<TimeRecordModel, Employee, Team, TimeRecordModel>(
                    (t, e, g) =>
                    {
                        t.Employee = e;
                        e.Team = g;
                        return t;
                    },
                       splitOn: "EmpId, GroupId",
                       parameterValues: new { EmpId = employeeId, groupIds = groupIds, fromDate = fromDate, toDate = toDate, isLocal }).ToList();
            }

            return timeRecords;
        }

        public IList<WorkingCalendarSummary> CalendarSummaryList(ICriteria criteria)
        {
            IList<WorkingCalendarSummary> calendars = null;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                calendars = db.Query<WorkingCalendarSummary>(criteria.GetSpParams()).ToList();
            }

            return calendars;
        }

        /// <summary>
        /// Method Get Supervisor Report.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of Supervisor Group Model.</returns>
        public List<SupervisorReportGroupModel> GetSupervisorReport(ICriteria criteria)
        {
            List<SupervisorReportModel> supervisors = null;
            List<SupervisorReportGroupModel> groupingSupervisor = null;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                supervisors = db.Query<SupervisorReportModel>(criteria.GetSpParams()).ToList();
            }

            groupingSupervisor = supervisors.GroupBy(x => x.EmpId)
                .Select(x => new SupervisorReportGroupModel { EmployeeId = x.Key, SupervisorAction = x.ToList() }).ToList();

            return groupingSupervisor;
        }

        /// <summary>
        /// Method Get All Team Supervisors.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List Of Team Supervisor Report Model.</returns>
        public List<TeamSupervisorReportModel> GetAllTeamSupervisor(ICriteria criteria)
        {
            List<TeamSupervisorModel> entities = null;
            List<TeamSupervisorReportModel> supervisors = null;

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                entities = db.Query<TeamSupervisorModel>(criteria.GetSpParams()).ToList();
            }

            supervisors = entities.GroupBy(x => x.TeamId)
                .Select(x => new TeamSupervisorReportModel { GroupId = x.Key, SupervisorAction = x.ToList() }).ToList();

            return supervisors;
        }
    }
}
