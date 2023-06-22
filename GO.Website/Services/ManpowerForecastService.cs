namespace GO.Website.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GO.Core.Services;
    using GO.Website.Criteria.Manpower;
    using GO.Website.Models.Manpower;
    using GO.Website.Models.Manpower.Period;

    public class ManpowerForecastService : IManpowerForecastService
    {
        private readonly IBaseService baseService;

        public ManpowerForecastService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<(ManpowerPeriod, IEnumerable<AnnualRequestFormDetail>)> GetDataForCreateAnnualRequestAsync(int userId)
        {
            ManpowerPeriod period = null;
            var criteria = new GetDataForCreateAnnualRequestCriteria
            {
                UserId = userId
            };
            (var periods, var details) = await baseService.FetchMultipleAsync<ManpowerPeriod, AnnualRequestFormDetail>(criteria);
            if (periods.Any())
            {
                period = periods.First();
            }

            return (period, details);
        }

        public async Task<AnnualRequestForm> GetAnnualRequestByIdAsync(int requestId)
        {
            AnnualRequestForm request = null;
               var criteria = new GetAnnualRequestByIdCriteria
            {
                RequestId = requestId
            };
            (var requests, var details) = await baseService.FetchMultipleAsync<AnnualRequestForm, AnnualRequestFormDetail>(criteria);
            if (requests.Any())
            {
                request = requests.First();
                request.Details = details.ToList();
            }

            return request;
        }
    }
}