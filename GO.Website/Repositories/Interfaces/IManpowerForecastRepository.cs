namespace GO.Website.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using GO.Website.Models.Manpower;

    public interface IManpowerForecastRepository
    {
        Task<IEnumerable<AnnualManpowerForecastRequest>> GetMyAnnualRequestAsync(ICriteria criteria);

        Task<IEnumerable<MemberAnnualManpowerForecastRequest>> GetMemberAnnualRequestAsync(ICriteria criteria);

        Task<AnnualManpowerForecastRequest> GetAnnualRequestByIdAsync(ICriteria criteria);
    }
}