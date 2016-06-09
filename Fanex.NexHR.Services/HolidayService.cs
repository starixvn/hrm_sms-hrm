namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;

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

        public IList<Holiday> GetByYear(int year)
        {
            return this.holidayRepository.GetByYear(year);
        }

        public Holiday GetById(int holidayId)
        {
            if (holidayId > 0)
            {
                return this.holidayRepository.GetById(holidayId);
            }

            return new Holiday();
        }
    }
}