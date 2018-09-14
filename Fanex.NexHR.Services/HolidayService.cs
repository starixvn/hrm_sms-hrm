namespace Fanex.NexHR.Services
{
    using Core.Services;
    using Criteria.Holidays;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class HolidayService : BaseService, IHolidayService
    {
        public IList<Holiday> GetByYear(int year)
        {
            GetHolidaysCriteria criteria = new GetHolidaysCriteria
            {
                Year = year
            };
            IList<Holiday> holidays = null;

            if (criteria.IsValid())
            {
                holidays = GetData<Holiday>(criteria).ToList();
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
                holiday = GetDataEntity<Holiday>(criteria);
            }

            return holiday ?? new Holiday();
        }
    }
}