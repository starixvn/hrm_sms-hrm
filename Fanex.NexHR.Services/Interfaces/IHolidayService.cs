namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Holidays;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public interface IHolidayService
    {
        bool Delete(int holidayId, int modifiedBy);

        Holiday GetById(GetHolidayByIdCriteria holidayId);

        IList<Holiday> GetByYear(GetHolidaysCriteria criteria);

        int Insert(Holiday holiday);

        bool Update(Holiday holiday);
    }
}