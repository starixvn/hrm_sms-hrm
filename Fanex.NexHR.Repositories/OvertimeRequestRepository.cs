namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Request;

    public class OvertimeRequestRepository : IOvertimeRequestRepository
    {
        public IEnumerable<MonthlyOvertimeRequest> GetMonthlyRequestByEmployeeId(ICriteria criteria)
        {
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<MonthlyOvertimeRequest, RequestStatus, Requester, Requester, MonthlyOvertimeRequest>(
                    (re, stt, verifier, approver) =>
                    {
                        re.Status = stt;
                        re.Verifier = verifier;
                        re.Approver = approver;
                        return re;
                    },
                    "StatusId, VerifierId, ApproverId",
                    criteria.GetSpParams());
            }
        }

        public MonthlyOvertimeRequest GetMonthlyRequestDetailsByEmployeeId(ICriteria criteria)
        {
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria))
                {
                    var monthlyRequest = multi.Read<MonthlyOvertimeRequest, RequestStatus, Requester, Requester, Requester, MonthlyOvertimeRequest>(
                    (re, stt, requester, verifier, approver) =>
                    {
                        re.Requester = requester;
                        re.Status = stt;
                        re.Verifier = verifier;
                        re.Approver = approver;
                        return re;
                    },
                    "StatusId, EmployeeId, VerifierId, ApproverId").FirstOrDefault();

                    if (monthlyRequest != null)
                    {
                        monthlyRequest.DailyOvertimes = multi.Read<DailyOvertimeRequest>();
                    }

                    return monthlyRequest;
                }
            }
        }

        public IEnumerable<MonthlyOvertimeRequest> GetMemberMonthlyRequest(ICriteria criteria)
        {
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<MonthlyOvertimeRequest, RequestStatus, Requester, EntityBase<int>, Requester, Requester, MonthlyOvertimeRequest>(
                    (req, stt, requester, section, verifier, approver) =>
                    {
                        requester.Section = section;
                        req.Status = stt;
                        req.Requester = requester;
                        req.Verifier = verifier;
                        req.Approver = approver;
                        return req;
                    },
                    "StatusId, EmployeeId, GroupId, VerifierId, ApproverId",
                    criteria.GetSpParams());
            }
        }
    }
}