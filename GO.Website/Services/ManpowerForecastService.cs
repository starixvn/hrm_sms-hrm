namespace GO.Website.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GO.Core.Extensions.XML;
    using GO.Core.Models;
    using GO.Core.Services;
    using GO.Website.Criteria.Manpower;
    using GO.Website.Models.Manpower;
    using GO.Website.Models.Manpower.Period;
    using GO.Website.Repositories;

    public class ManpowerForecastService : IManpowerForecastService
    {
        private readonly IManpowerForecastRepository manpowerForecastRepository;
        private readonly IBaseService baseService;

        public ManpowerForecastService(
            IManpowerForecastRepository manpowerForecastRepository,
            IBaseService baseService)
        {
            this.manpowerForecastRepository = manpowerForecastRepository;
            this.baseService = baseService;
        }

        public async Task<IEnumerable<string>> GetUnitByUserIdAsync(int userId)
        {
            var criteria = new GetUnitByUserIdCriteria
            {
                UserId = userId
            };
            return await baseService.GetDataAsync<string>(criteria);
        }

        public async Task<(ManpowerPeriod, IEnumerable<AnnualManpowerForecastRequestDetail>)> GetDataForCreateAnnualRequestAsync(int userId, string unitName)
        {
            ManpowerPeriod period = null;
            var criteria = new GetDataForCreateAnnualManpowerForecastRequestCriteria
            {
                UserId = userId,
                UnitName = unitName
            };
            (var periods, var details) = await baseService.FetchMultipleAsync<ManpowerPeriod, AnnualManpowerForecastRequestDetail>(criteria);
            if (periods.Any())
            {
                period = periods.First();
            }

            return (period, details);
        }

        public async Task<IEnumerable<AnnualManpowerForecastRequest>> GetMyAnnualRequestAsync(int userId)
        {
            var criteria = new GetMyAnnualManpowerForecastRequestCriteria
            {
                UserId = userId
            };
            return await manpowerForecastRepository.GetMyAnnualRequestAsync(criteria);
        }

        public async Task<IEnumerable<MemberAnnualManpowerForecastRequest>> GetMemberAnnualRequestAsync(GetMemberAnnualManpowerForecastRequestCriteria criteria)
        {
            return await manpowerForecastRepository.GetMemberAnnualRequestAsync(criteria);
        }

        public async Task<AnnualManpowerForecastRequest> GetAnnualRequestByIdAsync(int requestId)
        {
            var criteria = new GetAnnualManpowerForecastRequestByIdCriteria
            {
                RequestId = requestId
            };
            AnnualManpowerForecastRequest request = await manpowerForecastRepository.GetAnnualRequestByIdAsync(criteria);

            return request;
        }

        public async Task<IEnumerable<AnnualManpowerForecastRequest>> GetAnnualRequestByPeriodIdAsync(
            GetAnnualManpowerForecastRequestByPeriodIdCriteria criteria)
        {
            return await manpowerForecastRepository.GetAnnualRequestByPeriodIdAsync(criteria);
        }

        public async Task<CommandResult> CreateAnnualRequestAsync(int userId, CreateAnnualManpowerForecastRequestParam param)
        {
            var command = new CreateAnnualManpowerForecastRequestCommand
            {
                PeriodId = param.PeriodId,
                UnitName = param.UnitName,
                DetailXml = param.Details != null && param.Details.Any() ? param.Details.ToXML().ToString() : string.Empty,
                CreatedBy = userId
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> UpdateStatusAnnualRequestAsync(UpdateStatusAnnualManpowerForecastRequestCommand command)
        {
            return await baseService.ExecuteNonQueryAsync(command);
        }
    }
}