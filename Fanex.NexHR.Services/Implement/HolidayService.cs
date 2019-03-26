namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Services;
    using Criteria.Holidays;
    using Fanex.NexHR.Models;

    public class HolidayService : IHolidayService
    {
        private readonly IBaseService baseService;

        public HolidayService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IList<Holiday> GetByYear(int year)
        {
            var criteria = new GetHolidaysCriteria
            {
                Year = year
            };
            IList<Holiday> holidays = null;

            if (criteria.IsValid())
            {
                holidays = baseService.GetData<Holiday>(criteria).ToList();
            }

            return holidays ?? new List<Holiday>();
        }

        public Holiday GetById(int holidayId)
        {
            var criteria = new GetHolidayByIdCriteria
            {
                Id = holidayId
            };
            Holiday holiday = null;

            if (criteria.IsValid())
            {
                holiday = baseService.GetDataEntity<Holiday>(criteria);
            }

            return holiday ?? new Holiday();
        }
    }
}