namespace GO.Website.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.Website.Criteria.UnitHead;
    using GO.Website.Models.UnitHead;

    public interface IUnitHeadService
    {
        Task<IEnumerable<UnitHeadWeeklySchedule>> GetWeeklyScheduleAsync(GetUnitHeadWeeklyScheduleCriteria criteria);
    }
}