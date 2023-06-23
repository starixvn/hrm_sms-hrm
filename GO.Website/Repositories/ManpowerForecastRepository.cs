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
                return await db.QueryAsync<AnnualManpowerForecastRequest, ManpowerPeriod, AnnualManpowerForecastRequest>(
                    (req, period) =>
                    {
                        req.Period = period;

                        return req;
                    },
                    splitOn: "PeriodId",
                    parameterSet: criteria);
            }
        }

        public async Task<AnnualManpowerForecastRequest> GetAnnualRequestByIdAsync(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = await db.QueryMultipleAsync(criteria))
                {
                    var request = multi.Read<AnnualManpowerForecastRequest, ManpowerPeriod, AnnualManpowerForecastRequest>(
                        (req, period) =>
                        {
                            req.Period = period;

                            return req;
                        },
                        splitOn: "PeriodId").FirstOrDefault();

                    if (request != null)
                    {
                        request.Details = multi.Read<AnnualManpowerForecastRequestDetail>().ToList();
                    }

                    return request;
                }
            }
        }
    }
}