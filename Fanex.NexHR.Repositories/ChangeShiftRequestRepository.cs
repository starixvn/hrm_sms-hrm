namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request.ChangeShift;
    using Fanex.NexHR.Models.ScheduleArrange;

    public interface IChangeShiftRequestRepository
    {
        IEnumerable<DailySchedule> GetWorkingCalendarForRequest(ICriteria criteria);

        IEnumerable<ChangeShiftRequest> GetByEmployee(ICriteria criteria);

        ChangeShiftRequest GetById(ICriteria criteria);

        IEnumerable<ChangeShiftRequest> GetHistoryById(ICriteria criteria);
    }

    public class ChangeShiftRequestRepository : IChangeShiftRequestRepository
    {
        public IEnumerable<DailySchedule> GetWorkingCalendarForRequest(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<DailySchedule, ShiftBase, DailySchedule>(
                    (dailySchedule, shift) =>
                    {
                        dailySchedule.Shift = shift;

                        return dailySchedule;
                    },
                    splitOn: "ShiftId",
                    parameterSet: criteria);
            }
        }

        public IEnumerable<ChangeShiftRequest> GetByEmployee(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<ChangeShiftRequest, Employee, Employee, Employee, Employee, ChangeShiftRequest>(
                    (request, employee, verifier, approver, hr) =>
                    {
                        request.Employee = employee;
                        request.VerifiedBy = verifier;
                        request.ApprovedBy = approver;
                        request.ProcessedBy = hr;

                        return request;
                    },
                    splitOn: "EmpId, VerifierId, ApproverId, HRId",
                    parameterSet: criteria);
            }
        }

        public ChangeShiftRequest GetById(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.QueryEntity<ChangeShiftRequest, Employee, Employee, Employee, ChangeShiftRequest>(
                    (request, employee, verifier, approver) =>
                    {
                        request.Employee = employee;
                        request.VerifiedBy = verifier;
                        request.ApprovedBy = approver;

                        return request;
                    },
                    splitOn: "EmpId, VerifierId, ApproverId",
                    parameterSet: criteria);
            }
        }

        public IEnumerable<ChangeShiftRequest> GetHistoryById(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<ChangeShiftRequest, Employee, Team, Employee, Employee, ChangeShiftRequest>(
                    (request, employee, team, verifier, approver) =>
                    {
                        request.Employee = employee;
                        request.Employee.Team = team;
                        request.VerifiedBy = verifier;
                        request.ApprovedBy = approver;
                        return request;
                    },
                    splitOn: "EmpId, GroupId, VerifierId, ApproverId",
                    parameterSet: criteria);
            }
        }
    }
}