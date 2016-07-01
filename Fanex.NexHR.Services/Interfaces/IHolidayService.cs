namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Holidays;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public interface IHolidayService
    {
        Holiday GetById(GetHolidayByIdCriteria holidayId);

        IList<Holiday> GetByYear(GetHolidaysCriteria criteria);
    }
}