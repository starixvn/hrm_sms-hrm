namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Request;
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
                return db.Query<ChangeShiftRequest, Requester, Requester, Requester, Requester, ChangeShiftRequest>(
                    (request, employee, verifier, approver, hr) =>
                    {
                        request.Requester = employee;
                        request.Verifier = verifier;
                        request.Approver = approver;
                        request.HR = hr;

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
                return db.QueryEntity<ChangeShiftRequest, Requester, Requester, Requester, ChangeShiftRequest>(
                    (request, employee, verifier, approver) =>
                    {
                        request.Requester = employee;
                        request.Verifier = verifier;
                        request.Approver = approver;

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
                return db.Query<ChangeShiftRequest, Requester, Requester, Requester, Requester, Requester, ChangeShiftRequest>(
                    (request, employee, verifier, approver, modifier, creater) =>
                    {
                        request.Requester = employee;
                        request.Verifier = verifier;
                        request.Approver = approver;
                        request.Modifier = modifier;
                        request.Creater = creater;
                        return request;
                    },
                    splitOn: "EmpId, VerifierId, ApproverId, ModifierId, CreaterId",
                    parameterSet: criteria);
            }
        }
    }
}