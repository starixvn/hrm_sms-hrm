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
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.ViewModels.Report;

    public class ReportRepository : IReportRepository
    {
        public IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(ICriteria criteria)
        {
            var listAttendance = new List<EmployeeAttendanceModel>();
            var currentAttendance = new EmployeeAttendanceModel();

            using (ObjectDb objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objectDb.Query<EmployeeAttendanceModel, dynamic, EmployeeAttendanceModel>(
                    (attendance, t) =>
                    {
                        if (currentAttendance == null
                            || currentAttendance.EmpId != attendance.EmpId
                            || currentAttendance.AttendanceDate.Value.Date != attendance.AttendanceDate.Value.Date)
                        {
                            listAttendance.Add(attendance);
                            currentAttendance = attendance;
                            currentAttendance.RequestList = new List<EmployeeAttendanceRequest>();
                        }

                        if (t != null && t.RequestType != null)
                        {
                            EmployeeAttendanceRequest request = FillAttendanceRequest(t);

                            currentAttendance.RequestList.Add(request);
                        }

                        return currentAttendance;
                    },
                    splitOn: "RequestType",
                    parameterValues: criteria.GetSpParams());
            }

            return listAttendance;
        }

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

        public IList<LeaveSummary> GetUnpaidLeave(ICriteria criteria)
        {
            IList<LeaveSummary> unpaidLeaves = null;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                unpaidLeaves = db.Query<LeaveSummary, Employee, Team, LeaveSummary>(
                    (leave, emp, team) =>
                    {
                        leave.Employee = emp;
                        emp.Team = team;
                        return leave;
                    },
                       splitOn: "EmpId, GroupId",
                       parameterValues: criteria.GetSpParams()).ToList();
            }

            return unpaidLeaves;
        }

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

        public IList<SupervisorReportModel> GetSupervisorReport(ICriteria criteria)
        {
            IList<SupervisorReportModel> supervisors = null;

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                supervisors = db.Query<SupervisorReportModel, Employee, Team, SupervisorReportModel>(
                    (su, e, g) =>
                    {
                        su.Employee = e;
                        e.Team = g;
                        return su;
                    },
                    splitOn: "EmpId, GroupId",
                    parameterValues: criteria.GetSpParams()).ToList();
            }

            return supervisors;
        }

        public IList<TeamSupervisorReportModel> GetAllTeamSupervisor(ICriteria criteria)
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

        private EmployeeAttendanceRequest FillAttendanceRequest(dynamic t)
        {
            var request = new EmployeeAttendanceRequest
            {
                RequestType = new RequestTypeModel { Id = (short)t.RequestType },
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