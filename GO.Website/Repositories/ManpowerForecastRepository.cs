namespace GO.Website.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using GO.Website.Models.Manpower;
    using GO.Website.Models.Manpower.Period;

    public class ManpowerForecastRepository : IManpowerForecastRepository
    {
        public async Task<IEnumerable<AnnualManpowerForecastRequest>> GetMyAnnualRequestAsync(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return await db.QueryAsync<AnnualManpowerForecastRequest, ManpowerPeriod, EmployeeInfo, EmployeeInfo, AnnualManpowerForecastRequest>(
                    (req, period, verifier, approver) =>
                    {
                        req.Period = period;
                        req.Verifier = verifier;
                        req.Approver = approver;

                        return req;
                    },
                    splitOn: "PeriodId, VerifierId, ApproverId",
                    parameterSet: criteria);
            }
        }

        public async Task<IEnumerable<AnnualManpowerForecastRequest>> GetMemberAnnualRequestAsync(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return await db.QueryAsync<AnnualManpowerForecastRequest, EmployeeInfo, EmployeeInfo, EmployeeInfo, AnnualManpowerForecastRequest>(
                    (req, creator, verifier, approver) =>
                    {
                        req.Creator = creator;
                        req.Verifier = verifier;
                        req.Approver = approver;

                        return req;
                    },
                    splitOn: "CreatorId, VerifierId, ApproverId",
                    parameterSet: criteria);
            }
        }

        public async Task<AnnualManpowerForecastRequest> GetAnnualRequestByIdAsync(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = await db.QueryMultipleAsync(criteria))
                {
                    var request = multi.Read<AnnualManpowerForecastRequest, ManpowerPeriod, EmployeeInfo, EmployeeInfo, EmployeeInfo, AnnualManpowerForecastRequest>(
                        (req, period, creator, verifier, approver) =>
                        {
                            req.Period = period;
                            req.Creator = creator;
                            req.Verifier = verifier;
                            req.Approver = approver;

                            return req;
                        },
                        splitOn: "PeriodId, CreatorId, VerifierId, ApproverId").FirstOrDefault();

                    if (request != null)
                    {
                        request.Details = multi.Read<AnnualManpowerForecastRequestDetail>().ToList();
                        request.LogDetails = multi.Read<AnnualManpowerForecastRequestDetail, EmployeeInfo, AnnualManpowerForecastRequestDetail>(
                            (log, modifiedBy) =>
                            {
                                log.ModifiedBy = modifiedBy;

                                return log;
                            },
                            splitOn: "ModifiedBy").ToList();
                    }

                    return request;
                }
            }
        }

        public async Task<IEnumerable<AnnualManpowerForecastRequest>> GetMemberAnnualRequestSummaryAsync(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = await db.QueryMultipleAsync(criteria))
                {
                    var requests = multi.Read<AnnualManpowerForecastRequest>().ToList();
                    var details = multi.Read<AnnualManpowerForecastRequestDetail>().ToList();

                    foreach (var request in requests)
                    {
                        request.Details = details.Where(x => x.RequestId == request.RequestId).ToList();
                    }

                    return requests;
                }
            }
        }

        public async Task<IEnumerable<AnnualManpowerForecastRequest>> GetManpowerForecastReportAsync(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = await db.QueryMultipleAsync(criteria))
                {
                    var requests = multi.Read<AnnualManpowerForecastRequest>().ToList();
                    var details = multi.Read<AnnualManpowerForecastRequestDetail>().ToList();

                    foreach (var request in requests)
                    {
                        request.Details = details.Where(x => x.RequestId == request.RequestId || x.UnitName == request.UnitName).ToList();
                    }

                    return requests;
                }
            }
        }

        public async Task<IEnumerable<AnnualManpowerForecastRequest>> GetAnnualRequestByPeriodIdAsync(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return await db.QueryAsync<AnnualManpowerForecastRequest, EmployeeInfo, EmployeeInfo, EmployeeInfo, AnnualManpowerForecastRequest>(
                    (req, creator, verifier, approver) =>
                    {
                        req.Creator = creator;
                        req.Verifier = verifier;
                        req.Approver = approver;

                        return req;
                    },
                    splitOn: "CreatorId, VerifierId, ApproverId",
                    parameterSet: criteria);
            }
        }
    }
}