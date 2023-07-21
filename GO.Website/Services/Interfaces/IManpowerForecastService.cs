namespace GO.Website.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.Core.Models;
    using GO.Website.Criteria.Manpower;
    using GO.Website.Criteria.Manpower.AnnualRequest;
    using GO.Website.Models.Manpower;
    using GO.Website.Models.Manpower.AnnualRequest;
    using GO.Website.Models.Manpower.Period;

    public interface IManpowerForecastService
    {
        Task<IEnumerable<string>> GetUnitByUserIdAsync(int userId);

        Task<(ManpowerPeriod, IEnumerable<AnnualManpowerForecastRequestDetail>)> GetDataForCreateAnnualRequestAsync(int userId, string unitName);

        Task<IEnumerable<AnnualManpowerForecastRequest>> GetMyAnnualRequestAsync(int userId);

        Task<IEnumerable<AnnualManpowerForecastRequest>> GetMemberAnnualRequestAsync(GetMemberAnnualManpowerForecastRequestCriteria criteria);

        Task<AnnualManpowerForecastRequest> GetAnnualRequestByIdAsync(int requestId);
        Task<IEnumerable<AnnualManpowerForecastRequest>> GetAllAnnualRequestByPeriodIdAsync(GetAllAnnualManpowerForecastRequestByPeriodIdCriteria criteria);

        Task<IEnumerable<AnnualManpowerForecastRequest>> GetAnnualRequestByPeriodIdAsync(GetAnnualManpowerForecastRequestByPeriodIdCriteria criteria);

        Task<CommandResult> CreateAnnualRequestAsync(int userId, CreateAnnualManpowerForecastRequestParam param);
        Task<CommandResult> UpdateAnnualRequestAsync(int userId, UpdateAnnualManpowerForecastRequestParam param);

        Task<CommandResult> UpdateStatusAnnualRequestAsync(UpdateStatusAnnualManpowerForecastRequestCommand command);
    }
}