namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;

    public class KeepInViewRequestRepository: IKeepInViewRequestRepository
    {
        public IList<KeepInViewRequest> GetByDate(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entity = db.Query<KeepInViewRequest, Employee, Employee, Employee, Employee, KeepInViewRequest>(
                        (request, employee, verifier, approver, hr) =>
                        {
                            request.Employee = employee;
                            request.VerifiedBy = verifier;
                            request.ApprovedBy = approver;
                            request.ProcessedBy = hr;
                            return request;
                        },
                        splitOn: "EmpId, VerifierId, ApproverId, HRId",
                        parameterValues: criteria.GetSpParams());

                return entity.ToList();
            }
        }

        public KeepInViewRequest GetById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entity = db.QueryEntity<KeepInViewRequest, Employee, Employee, Employee, Employee, KeepInViewRequest>(
                        (request, employee, verifier, approver, hr) =>
                        {
                            request.Employee = employee;
                            request.VerifiedBy = verifier;
                            request.ApprovedBy = approver;
                            request.ProcessedBy = hr;
                            return request;
                        },
                        splitOn: "EmpId, VerifierId, ApproverId, HRId",
                        parameters: criteria.GetSpParams());

                return entity;
            }
        }

        public IList<KeepInViewRequest> GetHistoryById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entity = db.Query<KeepInViewRequest, Employee, Team, Employee, Employee, Employee, KeepInViewRequest>(
                        (request, employee, team, verifier, approver, hr) =>
                        {
                            request.Employee = employee;
                            request.Employee.Team = team;
                            request.VerifiedBy = verifier;
                            request.ApprovedBy = approver;
                            request.ProcessedBy = hr;
                            return request;
                        },
                        splitOn: "EmpId, GroupId, VerifierId, ApproverId, HRId",
                        parameterValues: criteria.GetSpParams());

                return entity.ToList();
            }
        }
    }
}
