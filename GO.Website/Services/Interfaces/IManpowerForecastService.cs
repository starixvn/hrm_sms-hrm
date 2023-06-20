namespace GO.Website.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.Website.Models.Manpower;
    using GO.Website.Models.Manpower.Period;

    public interface IManpowerForecastService
    {
        Task<(ManpowerPeriod, IEnumerable<AnnualRequestFormDetail>)> GetDataForCreateAnnualRequestAsync(int userId);
    }
}