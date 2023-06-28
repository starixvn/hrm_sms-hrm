namespace GO.Website.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.Core.Models;
    using GO.Website.Criteria.Manpower;
    using GO.Website.Models.Manpower;
    using GO.Website.Models.Manpower.Period;

    public interface IManpowerForecastService
    {
        Task<(ManpowerPeriod, IEnumerable<AnnualManpowerForecastRequestDetail>)> GetDataForCreateAnnualRequestAsync(int userId);

        Task<IEnumerable<AnnualManpowerForecastRequest>> GetMyAnnualRequestAsync(int userId);

        Task<IEnumerable<MemberAnnualManpowerForecastRequest>> GetMemberAnnualRequestAsync(GetMemberAnnualManpowerForecastRequestCriteria criteria);

        Task<AnnualManpowerForecastRequest> GetAnnualRequestByIdAsync(int requestId);

        Task<CommandResult> CreateAnnualRequestAsync(int userId, CreateAnnualManpowerForecastRequestParam param);

        Task<CommandResult> UpdateStatusAnnualRequestAsync(UpdateStatusAnnualManpowerForecastRequestCommand command);
    }
}