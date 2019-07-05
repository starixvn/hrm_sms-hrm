namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Core.XMLExtensions;
    using Fanex.NexHR.Criteria.Holidays;
    using Fanex.NexHR.Models;

    public class HolidayService : IHolidayService
    {
        private readonly IBaseService baseService;
        private readonly ChineseLunisolarCalendar chinaCalendar = new ChineseLunisolarCalendar();

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
            IList<Holiday> holidays = baseService.GetData<Holiday>(criteria).ToList();

            return holidays ?? new List<Holiday>();
        }

        public Holiday GetById(int holidayId)
        {
            var criteria = new GetHolidayByIdCriteria
            {
                Id = holidayId
            };
            Holiday holiday = baseService.GetDataEntity<Holiday>(criteria);

            return holiday ?? new Holiday();
        }

        public CommandResult Create(int actorId, Holiday model)
        {
            var criteria = new CreateHolidayCriteria
            {
                HolidayName = model.HolidayName,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                ModifiedBy = actorId,
                ModifiedDate = DateTime.Now
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Edit(int actorId, Holiday model)
        {
            var criteria = new EditHolidayCriteria
            {
                Id = model.Id,
                HolidayName = model.HolidayName,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                ModifiedBy = actorId,
                ModifiedDate = DateTime.Now
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Delete(int actorId, int id)
        {
            var criteria = new DeleteHolidayCriteria
            {
                Id = id,
                ModifiedBy = actorId,
                ModifiedDate = DateTime.Now
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        #region Auto detect holidays and insert to DB

        private DateTime ConvertLunarToSolarCalendar(int year, int month, int day)
        {
            var solarDate = chinaCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
            return solarDate;
        }

        private IEnumerable<Holiday> SolarHolidays(int year)
        {
            var solarHolidays = new List<Holiday>();
            var newYear = new Holiday
            {
                HolidayName = $"New Year Day {year}",
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 1, 1)
            };

            var vitoryDay = new Holiday
            {
                HolidayName = "Victory Day",
                FromDate = new DateTime(year, 4, 30),
                ToDate = new DateTime(year, 4, 30)
            };

            var labourDay = new Holiday
            {
                HolidayName = "Labour Day",
                FromDate = new DateTime(year, 5, 1),
                ToDate = new DateTime(year, 5, 1)
            };

            var independenceDay = new Holiday
            {
                HolidayName = "Independence Day",
                FromDate = new DateTime(year, 9, 2),
                ToDate = new DateTime(year, 9, 2)
            };

            solarHolidays.Add(newYear);
            solarHolidays.Add(vitoryDay);
            solarHolidays.Add(labourDay);
            solarHolidays.Add(independenceDay);

            return solarHolidays;
        }

        private IEnumerable<Holiday> LunarHolidays(int year)
        {
            var lunarHolidays = new List<Holiday>();
            var newYear1stOnSolarDay = ConvertLunarToSolarCalendar(year, 1, 1);

            for (int i = -1; i < 4; i++)
            {
                var tetHoliday = newYear1stOnSolarDay.AddDays(i);
                var holiday = new Holiday
                {
                    HolidayName = "Traditional New Year",
                    FromDate = tetHoliday,
                    ToDate = tetHoliday
                };
                lunarHolidays.Add(holiday);
            }

            var kingAnniSolar = ConvertLunarToSolarCalendar(year, 3, 10);
            var kingAnniHoliday = new Holiday
            {
                HolidayName = "King Hung's Anniversary",
                FromDate = kingAnniSolar,
                ToDate = kingAnniSolar
            };
            lunarHolidays.Add(kingAnniHoliday);
            return lunarHolidays;
        }

        public CommandResult AutoInsertHolidayOnYear(int year)
        {
            var holidays = new List<Holiday>();
            holidays.AddRange(SolarHolidays(year));
            holidays.AddRange(LunarHolidays(year));
            holidays = holidays.OrderBy(h => h.FromDate).ToList();

            var criteria = new AutoGenerateHoliDayCriteria
            {
                Holidays = holidays.ToXML().ToString()
            };

            var result = baseService.ExecuteNonQuery(criteria);

            return result;
        }

        #endregion Auto detect holidays and insert to DB
    }
}