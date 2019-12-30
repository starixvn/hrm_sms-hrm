namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;

    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        public IList<LeaveRequest> GetByDate(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    var leaves = multi.Read<LeaveRequest, LeaveType, Employee, Employee, Employee, Employee, LeaveRequest>(
                        (request, leaveType, employee, verifier, approver, hr) =>
                        {
                            request.LeaveType = leaveType;
                            request.Employee = employee;
                            request.VerifiedBy = verifier;
                            request.ApprovedBy = approver;
                            request.ProcessedBy = hr;
                            return request;
                        },
                        splitOn: "LeaveTypeId, EmpId, VerifierId, ApproverId, HRId").ToList();

                    var leaveDetails = multi.Read<LeaveRequestDetail>();
                    return MappingLeaveDetail(leaves, leaveDetails);
                }
            }
        }

        private static IList<LeaveRequest> MappingLeaveDetail(IList<LeaveRequest> leaves, IEnumerable<LeaveRequestDetail> leaveDetails)
        {
            foreach (var leave in leaves)
            {
                leave.LeaveDetail = leaveDetails.Where(t => t.RequestId == leave.RequestId).ToList();
            }

            return leaves;
        }

        public LeaveRequest GetById(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    var leave = multi.Read<LeaveRequest, LeaveType, Employee, Employee, Employee, Employee, LeaveRequest>(
                        (request, leaveType, employee, verifier, approver, hr) =>
                        {
                            request.LeaveType = leaveType;
                            request.Employee = employee;
                            request.VerifiedBy = verifier;
                            request.ApprovedBy = approver;
                            request.ProcessedBy = hr;
                            return request;
                        },
                        splitOn: "LeaveTypeId, EmpId, VerifierId, ApproverId, HRId").FirstOrDefault();

                    IEnumerable<LeaveRequestDetail> leaveDetails = multi.Read<LeaveRequestDetail>();
                    leave.LeaveDetail = leaveDetails.ToList();

                    return leave;
                }
            }
        }

        public IEnumerable<LeaveRequest> GetHistoryById(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    var leaves = multi.Read<LeaveRequest, LeaveType, Employee, Team, Employee, Employee, Employee, LeaveRequest>(
                        (request, leaveType, employee, team, verifier, approver, hr) =>
                        {
                            request.LeaveType = leaveType;
                            request.Employee = employee;
                            request.Employee.Team = team;
                            request.VerifiedBy = verifier;
                            request.ApprovedBy = approver;
                            request.ProcessedBy = hr;
                            return request;
                        },
                        splitOn: "LeaveTypeId, EmpId, GroupId, VerifierId, ApproverId, HRId").ToList();
                    var leaveDetails = multi.Read<LeaveRequestDetail>();

                    foreach (var leave in leaves)
                    {
                        leave.LeaveDetail = leaveDetails.Where(x => x.HistoryId == leave.HistoryId && x.RequestId == leave.RequestId).ToList();
                    }

                    return leaves;
                }
            }
        }

        public IEnumerable<Models.ScheduleArrange.DailySchedule> GetEmployeeCalendar(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<Models.ScheduleArrange.DailySchedule,
                    Models.ScheduleArrange.ShiftBase,
                    Models.ScheduleArrange.LeaveRequest,
                    Models.ScheduleArrange.LeaveRequest,
                    Models.ScheduleArrange.DailySchedule>(
                    (dailySchedule, shift, leave, leave2) =>
                    {
                        dailySchedule.Shift = shift;
                        dailySchedule.LeaveType = leave;
                        dailySchedule.LeaveType2 = leave2;

                        return dailySchedule;
                    },
                    splitOn: "ShiftId, LeaveTypeId, LeaveTypeId2",
                    parameterSet: criteria);
            }
        }
    }
}