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
        /// Get Employee Attendance Report.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List Of EmployeeAttendanceModel.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(ICriteria criteria)
        {
            var listAttendance = new List<EmployeeAttendanceModel>();
            EmployeeAttendanceModel currentAttendance = new EmployeeAttendanceModel();
            EmployeeAttendanceRequest request = new EmployeeAttendanceRequest();

            using (ObjectDb objectDb = new ObjectDb(criteria.GetSettingKey()))
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
                    criteria.GetSpParams());
            }

            return listAttendance;
        }

        /// <summary>
        /// Get Request Monitoring.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List Request Monitoring.</returns>
        public IList<RequestsMonitoring> GetRequestMonitoring(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entity = db.Query<RequestsMonitoring, Employee, Team, RequestsMonitoring>(
                        (r, e, t) =>
                        {
                            e.Team = t;
                            r.Requester = e;
                            return r;
                        },
                        splitOn: "Id,GroupId",
                        parameterValues: criteria.GetSpParams());

                return entity.ToList();
            }
        }

        /// <summary>
        /// Get Total Working Hours.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List Total Working Hours.</returns>
        public IList<TotalWorkingHours> GetWorkingHoursList(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entity = db.Query<TotalWorkingHours, Employee, Team, TotalWorkingHours>(
                        (w, e, t) =>
                        {
                            e.Team = t;
                            w.Employee = e;
                            
                            return w;
                        },
                        splitOn: "EmpId,GroupId",
                        parameterValues: criteria.GetSpParams());

                return entity.ToList();
            }
        }

        /// <summary>
        /// Get Total Working Hours Monthly.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List Total Working Hours.</returns>
        public IList<TotalWorkingHourMonthly> GetTotalWorkingHourMonthly(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entityList = new List<TotalWorkingHourMonthly>();

                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
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
        /// Get Total Working Hours Weekly.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List Of Total Working Hour Monthly.</returns>
        public IList<TotalWorkingHourMonthly> GetTotalWorkingHourWeekly(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entityList = new List<TotalWorkingHourMonthly>();

                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
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
