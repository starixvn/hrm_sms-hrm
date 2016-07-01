namespace Fanex.NexHR.Services
{
    using Criteria.Holidays;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class HolidayService : BaseService, IHolidayService
    {
        public IList<Holiday> GetByYear(GetHolidaysCriteria criteria)
        {
            IList<Holiday> holidays = null;

            if (criteria.IsValid())
            {
                holidays = GetData<Holiday>(criteria).ToList();
            }

            return holidays ?? new List<Holiday>();
        }

        public Holiday GetById(GetHolidayByIdCriteria criteria)
        {
            Holiday holiday = null;

            if (criteria.IsValid())
            {
                holiday = GetDataEntity<Holiday>(criteria);
            }

            return holiday ?? new Holiday();
        }
    }
}