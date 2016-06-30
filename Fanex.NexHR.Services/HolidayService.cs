namespace Fanex.NexHR.Services
{
    using Criteria.Holidays;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class HolidayService : BaseService, IHolidayService
    {
        private readonly IHolidayRepository holidayRepository;

        public HolidayService(IHolidayRepository holidayRepository = null)
        {
            this.holidayRepository = holidayRepository ?? new HolidayRepository();
        }

        public int Insert(Holiday holiday)
        {
            int result = 0;
            if (holiday != null)
            {
                result = this.holidayRepository.Insert(holiday);
            }

            return result;
        }

        public bool Delete(int holidayId, int modifiedBy)
        {
            bool result = false;
            if (holidayId > 0)
            {
                result = this.holidayRepository.Delete(holidayId, modifiedBy);
            }

            return result;
        }

        public bool Update(Holiday holiday)
        {
            bool result = false;
            if (holiday != null)
            {
                result = this.holidayRepository.Update(holiday);
            }

            return result;
        }

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