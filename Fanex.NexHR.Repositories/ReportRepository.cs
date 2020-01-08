namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.ViewModels;

    public partial class ReportRepository : IReportRepository
    {
        public IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(ICriteria criteria)
        {
            var listAttendance = new List<EmployeeAttendanceModel>();
            var currentAttendance = new EmployeeAttendanceModel();

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
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
                    parameterSet: criteria);
            }

            return listAttendance;
        }

        public IList<SupervisorReportModel> GetSupervisorReport(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    IList<SupervisorReportModel> supervisors = multi.Read<SupervisorReportModel, SectionModel, SupervisorReportModel>(
                    (su, section) =>
                    {
                        su.Section = section;

                        return su;
                    },
                    splitOn: "SectionId").ToList();

                    IEnumerable<EmployeeRole> roles = multi.Read<EmployeeRole, SystemRoleModel, EmployeeRole>(
                        (er, r) =>
                        {
                            er.Role = r;

                            return er;
                        },
                        "RoleId");

                    IList<SupervisorModel> employees = multi.Read<SupervisorModel, Employee, SupervisorModel>(
                    (su, e) =>
                    {
                        su.Supervisor = e;

                        return su;
                    },
                    splitOn: "EmpId").ToList();

                    foreach (SupervisorReportModel supervisor in supervisors)
                    {
                        supervisor.Supervisors = employees.Where(x => x.Supervisor.Id == supervisor.Id).ToList();
                        supervisor.Roles = roles.Where(i => i.EmployeeId == supervisor.Id).Select(i => i.Role).ToList();
                    }

                    return supervisors;
                }
            }
        }

        public IList<SectionSupervisorReportModel> GetSectionSupervisor(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    IList<SectionSupervisorReportModel> supervisors = multi.Read<SectionSupervisorReportModel>().ToList();

                    IList<SupervisorModel> employees = multi.Read<SupervisorModel, SectionModel, SupervisorModel>(
                    (su, section) =>
                    {
                        su.Section = section;

                        return su;
                    },
                    splitOn: "SectionId").ToList();

                    foreach (SectionSupervisorReportModel supervisor in supervisors)
                    {
                        supervisor.Supervisors = employees.Where(x => x.Section.SectionId == supervisor.SectionId).ToList();
                    }

                    return supervisors;
                }
            }
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
                TotalMins = t.Time,
                CurrentStatus = t.CurrentStatus,
                KeepInViewTypeId = t.KeepInViewTypeId
            };
            return request;
        }

        private LeaveType FillLeaveType(dynamic t)
        {
            var leaveType = new LeaveType
            {
                Id = t.LeaveTypeId ?? 0,
                Name = t.LeaveType
            };

            return leaveType;
        }
    }
}