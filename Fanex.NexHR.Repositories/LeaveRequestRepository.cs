namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;

    public class LeaveRequestRepository: ILeaveRequestRepository
    {
        public IList<LeaveRequest> GetByDate(ICriteria criteria)
        {
            using (IObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
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

                    IEnumerable<LeaveRequestDetail> leaveDetails = multi.Read<LeaveRequestDetail>();
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
            using (IObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var leave = multi.Read<LeaveRequest, LeaveType, Employee, Employee, Employee, Employee, LeaveRequest> (
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

        public IList<LeaveRequest> GetHistoryById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entity = db.Query<LeaveRequest, LeaveType, Employee, Team, Employee, Employee, Employee, LeaveRequest>(
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
                        splitOn: "LeaveTypeId, EmpId, GroupId, VerifierId, ApproverId, HRId",
                        parameterValues: criteria.GetSpParams());

                return entity.ToList();
            }
        }
    }
}
