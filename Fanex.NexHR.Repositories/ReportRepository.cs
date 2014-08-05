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
    using System.Linq;
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
        public IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(ICriteria criteria)
        {
            using (ObjectDb objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var entity = objectDb.Query<EmployeeAttendanceModel, dynamic, EmployeeAttendanceModel>(
                    (attendance, t) =>
                    {
                        attendance.RequestList = new List<EmployeeAttendanceRequest>();
                        if (t != null && t.RequestType != null)
                        {
                            EmployeeAttendanceRequest request = FillAttendanceRequest(t);

                            attendance.RequestList.Add(request);
                        }

                        return attendance;
                    },
                    splitOn: "RequestType",
                    parameterValues: criteria.GetSpParams());

                return entity.ToList();
            }
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
        /// Get Annual Leave Balance Report.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List Leave Summary.</returns>
        public IList<LeaveSummary> GetAnnualLeaveBalanceReport(ICriteria criteria)
        {
            IList<LeaveSummary> entities = null;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                entities = db.Query<LeaveSummary, Employee, Team, LeaveSummary>(
                    (leave, emp, team) =>
                    {
                        leave.Employee = emp;
                        emp.Team = team;
                        return leave;
                    },
                       splitOn: "EmpId, GroupId",
                       parameterValues: criteria.GetSpParams()).ToList();
            }

            return entities;
        }

        /// <summary>
        /// Function to get time records.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of time records.</returns>
        public IList<TimeRecordModel> GetTimeRecords(ICriteria criteria)
        {
            IList<TimeRecordModel> timeRecords = null;

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                timeRecords = db.Query<TimeRecordModel, Employee, Team, TimeRecordModel>(
                    (t, e, g) =>
                    {
                        t.Employee = e;
                        e.Team = g;
                        return t;
                    },
                       splitOn: "EmpId, GroupId",
                       parameterValues: criteria.GetSpParams()).ToList();
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

        /// <summary>
        /// Function to fill employee attendance request.
        /// </summary>
        /// <param name="t">dynamic object.</param>
        /// <returns>EmployeeAttendance Request.</returns>
        private EmployeeAttendanceRequest FillAttendanceRequest(dynamic t)
        {
            var request = new EmployeeAttendanceRequest
            {
                RequestType = new RequestTypeModel { Id = t.RequestType },
                FromDate = t.FromDate,
                ToDate = t.ToDate,
                FromTime = t.FromTime,
                ToTime = t.ToTime,
                CreatedDate = t.CreatedDate,
                Remark = t.Remark,
                TotalDays = t.TotalDays,
                LeaveType = FillLeaveType(t),
                FromShift = t.FromShift,
                ToShift = t.ToShift,
                TotalMins = t.Time
            };
            return request;
        }

        private LeaveType FillLeaveType(dynamic t)
        {
            var leaveType = new LeaveType
            {
                Id = t.LeaveTypeId == null ? 0 : t.LeaveTypeId,
                Name = t.LeaveType
            };

            return leaveType;
        }
    }
}
