namespace GO.Website.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using GO.Website.Models.Manpower;

    public interface IManpowerForecastRepository
    {
        Task<IEnumerable<AnnualManpowerForecastRequest>> GetMyAnnualRequestAsync(ICriteria criteria);

        Task<IEnumerable<AnnualManpowerForecastRequest>> GetMemberAnnualRequestAsync(ICriteria criteria);

        Task<AnnualManpowerForecastRequest> GetAnnualRequestByIdAsync(ICriteria criteria);

        Task<IEnumerable<AnnualManpowerForecastRequest>> GetAnnualRequestByPeriodIdAsync(ICriteria criteria);
        Task<ValueTuple<IEnumerable<AnnualManpowerForecastRequest>, IEnumerable<AnnualManpowerForecastRequestDetail>>> GetAllAnnualRequestByPeriodIdAsync(ICriteria criteria);
    }
}