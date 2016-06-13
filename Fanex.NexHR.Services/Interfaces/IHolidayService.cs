namespace Fanex.NexHR.Services.Interfaces
{
    public interface IHolidayService
    {
        bool Delete(int holidayId, int modifiedBy);

        Fanex.NexHR.Models.Holiday GetById(int holidayId);

        System.Collections.Generic.IList<Fanex.NexHR.Models.Holiday> GetByYear(int year);

        int Insert(Fanex.NexHR.Models.Holiday holiday);

        bool Update(Fanex.NexHR.Models.Holiday holiday);
    }
}