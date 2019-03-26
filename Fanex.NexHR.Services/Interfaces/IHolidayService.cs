namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public interface IHolidayService
    {
        Holiday GetById(int holidayId);

        IList<Holiday> GetByYear(int year);
    }
}